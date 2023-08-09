float PAINT_UNIT();
float FLOAT_PRECISION();
bool floats_equal(float a, float b);

struct ColumnInfo
{
    uint size;

    // This has a different meaning depending on the usage.
    // Therefore you cannot use a column with both, fill and push
    uint write_index;

    float volume;
};

ColumnInfo column_info_create_empty()
{
    ColumnInfo c;
    c.size = 0;
    c.write_index = 0;
    c.volume = 0;

    return c;
}

bool column_is_full(ColumnInfo c, uint3 pg_size)
{
    return c.write_index == pg_size.z;
}

// it is assumed, that src and dst are same size, since this is only used for reservoir duplication
void paint_grid_copy(
    RWStructuredBuffer<ColumnInfo> src_pg_info, RWStructuredBuffer<Paint> src_pg_content, uint3 src_pg_size, uint2 src_pos,
    RWStructuredBuffer<ColumnInfo> dst_pg_info, RWStructuredBuffer<Paint> dst_pg_content, uint3 dst_pg_size, uint2 dst_pos)
{
    dst_pg_info[XY(dst_pos.x, dst_pos.y, dst_pg_size.xy)] = src_pg_info[XY(src_pos.x, src_pos.y, src_pg_size.xy)];

    for (uint z = 0; z < src_pg_info[XY(src_pos.x, src_pos.y, src_pg_size.xy)].size; z++)
    {
        dst_pg_content[XYZ(dst_pos.x, dst_pos.y, z, dst_pg_size)] = src_pg_content[XYZ(src_pos.x, src_pos.y, z, src_pg_size)];
    }
}

void paint_grid_fill(
    RWStructuredBuffer<ColumnInfo> pg_info, RWStructuredBuffer<Paint> pg_content, uint3 pg_size,
    float pg_cell_volume, uint pg_diffuse_depth, float pg_diffuse_ratio,
    uint2 pos, Paint p)
{
    uint initial_write_index = pg_info[XY(pos.x, pos.y, pg_size.xy)].write_index;

    // fill paint into column
    Paint left = p;
    while (!is_empty(left) && !column_is_full(pg_info[XY(pos.x, pos.y, pg_size.xy)], pg_size))
    {
        uint z = pg_info[XY(pos.x, pos.y, pg_size.xy)].write_index;
        Paint top = pg_content[XYZ(pos.x, pos.y, z, pg_size)];

        // try fill up top
        float fits_into_top = pg_cell_volume - top.volume;
        Paint element_part = paint_create(left.color, min(fits_into_top, left.volume));

        Paint updated_top = mix(top, element_part);

        pg_content[XYZ(pos.x, pos.y, z, pg_size)] = updated_top;
        pg_info[XY(pos.x, pos.y, pg_size.xy)].volume += element_part.volume;
        left.volume -= element_part.volume;

        // update column info
        bool top_empty_before = is_empty(top);
        if (top_empty_before)
        {
            pg_info[XY(pos.x, pos.y, pg_size.xy)].size++;
        }
        bool top_was_filled = floats_equal(updated_top.volume, pg_cell_volume);
        if (top_was_filled)
        {
            pg_info[XY(pos.x, pos.y, pg_size.xy)].write_index++;
        }
    }

    // diffuse at border (only to bottom though)
    int diffuse_steps_left = pg_diffuse_depth;
    int diffuse_index = initial_write_index;
    while (diffuse_steps_left > 0 && diffuse_index > 0)
    {
        Paint cell = pg_content[XYZ(pos.x, pos.y, diffuse_index, pg_size)];
        Paint below = pg_content[XYZ(pos.x, pos.y, diffuse_index-1, pg_size)];

        Paint cell_diffuse_part = paint_create(cell.color, cell.volume * pg_diffuse_ratio);
        Paint cell_staying_part = paint_create(cell.color, cell.volume - cell_diffuse_part.volume);

        Paint below_diffuse_part = paint_create(below.color, cell_diffuse_part.volume);
        // TODO
        // What if below - below_diffuse is < 0? -> currently handled by mix function
        // Should never happen though, because below would have been filled before anyways
        Paint below_staying_part = paint_create(below.color, below.volume - below_diffuse_part.volume);

        Paint new_cell = mix(cell_staying_part, below_diffuse_part);
        Paint new_below = mix(below_staying_part, cell_diffuse_part);

        pg_content[XYZ(pos.x, pos.y, diffuse_index, pg_size)] = new_cell;
        pg_content[XYZ(pos.x, pos.y, diffuse_index-1, pg_size)] = new_below;

        diffuse_steps_left--;
        diffuse_index--;
    }
}

void paint_grid_push(
    RWStructuredBuffer<ColumnInfo> pg_info, RWStructuredBuffer<Paint> pg_content, uint3 pg_size,
    uint2 pos, Paint element)
{
    if (!column_is_full(pg_info[XY(pos.x, pos.y, pg_size.xy)], pg_size) && !is_empty(element))
    {
        uint z = pg_info[XY(pos.x, pos.y, pg_size.xy)].size;
        pg_content[XYZ(pos.x, pos.y, z, pg_size)] = element;

        pg_info[XY(pos.x, pos.y, pg_size.xy)].size++;
        pg_info[XY(pos.x, pos.y, pg_size.xy)].write_index = pg_info[XY(pos.x, pos.y, pg_size.xy)].size;
        pg_info[XY(pos.x, pos.y, pg_size.xy)].volume += element.volume;
    }
}

void paint_grid_reverse_transfer(
    RWStructuredBuffer<ColumnInfo> src_pg_info, RWStructuredBuffer<Paint> src_pg_content, uint3 src_pg_size, uint2 src_pos,
    RWStructuredBuffer<ColumnInfo> dst_pg_info, RWStructuredBuffer<Paint> dst_pg_content, uint3 dst_pg_size, uint2 dst_pos,
    float dst_pg_cell_volume, uint dst_pg_diffuse_depth, float dst_pg_diffuse_ratio)
{
    for (uint z = 0; z < src_pg_info[XY(src_pos.x, src_pos.y, src_pg_size.xy)].size; z++)
    {
        Paint p = src_pg_content[XYZ(src_pos.x, src_pos.y, z, src_pg_size)];
        paint_grid_fill(dst_pg_info, dst_pg_content, dst_pg_size, dst_pg_cell_volume, dst_pg_diffuse_depth, dst_pg_diffuse_ratio, dst_pos, p);
    }
}

void paint_grid_clear(RWStructuredBuffer<ColumnInfo> pg_info, RWStructuredBuffer<Paint> pg_content, uint3 pg_size, uint2 clear_pos)
{
    for (uint z = 0; z < pg_info[XY(clear_pos.x, clear_pos.y, pg_size.xy)].size; z++)
    {
        pg_content[XYZ(clear_pos.x, clear_pos.y, z, pg_size)] = paint_create_empty();
    }

    pg_info[XY(clear_pos.x, clear_pos.y, pg_size.xy)] = column_info_create_empty();
}

// it is assumed, that cells are only emptied from top to bottom (write_index update)
void paint_grid_delete(
    RWStructuredBuffer<ColumnInfo> pg_info, RWStructuredBuffer<Paint> pg_content, uint3 pg_size,
    uint3 delete_pos, float delete_volume)
{
    Paint available = pg_content[XYZ(delete_pos.x, delete_pos.y, delete_pos.z, pg_size)];
    // we include the volume from content as well as from info to prevent negative values due to float precision errors (scenario is hard to test and therefore not tested)
    // Q: is it possible for total info volume to be empty, preventing the deletion from content?
    // A: probably not, because total info volume can't get fully emptied once filled (MIN_VOLUME_TO_STAY)
    // A2: maybe yes, because there is MIN_VOLUME_TO_STAY for rakel reservoir
    float to_be_deleted = min(min(delete_volume, available.volume), pg_info[XY(delete_pos.x, delete_pos.y, pg_size.xy)].volume);
    pg_info[XY(delete_pos.x, delete_pos.y, pg_size.xy)].volume -= to_be_deleted;
    Paint updated = paint_create(available.color, available.volume - to_be_deleted);
    if (is_empty(updated))
    {
        updated = paint_create_empty();
    }
    pg_content[XYZ(delete_pos.x, delete_pos.y, delete_pos.z, pg_size)] = updated;
}

void paint_grid_update_size(
    RWStructuredBuffer<ColumnInfo> pg_info, RWStructuredBuffer<Paint> pg_content, uint3 pg_size, float pg_cell_volume,
    uint2 pos)
{
    int z = 0;
    Paint top = pg_content[XYZ(pos.x, pos.y, (uint)z, pg_size)];

    bool write_index_found = false;
    uint write_index = 0;
    if (top.volume < pg_cell_volume)
    {
        write_index_found = true;
    }

    while (!is_empty(top) && z < (int)pg_size.z)
    {
        z++;
        top = pg_content[XYZ(pos.x, pos.y, (uint)z, pg_size)];

        if (!write_index_found && top.volume < pg_cell_volume)
        {
            write_index = z;
            write_index_found = true;
        }
    }

    pg_info[XY(pos.x, pos.y, pg_size.xy)].write_index = write_index;

    // now z is the index at which there is no paint anymore
    pg_info[XY(pos.x, pos.y, pg_size.xy)].size = (uint)z;
}

Paint paint_grid_get(RWStructuredBuffer<ColumnInfo> pg_info, RWStructuredBuffer<Paint> pg_content, uint3 pg_size, uint3 get_pos)
{
    return pg_content[XYZ(get_pos.x, get_pos.y, get_pos.z, pg_size)];
}