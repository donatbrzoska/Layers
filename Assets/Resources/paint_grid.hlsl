float PAINT_UNIT();
bool floats_equal(float a, float b);

struct ColumnInfo
{
    uint size;
    uint max_size;

    // This has a different meaning depending on the usage.
    // Therefore you cannot use a column with both, fill and push
    uint write_index;

    float volume;
};

bool column_is_full(ColumnInfo c)
{
    return c.write_index == c.max_size;
}

// it is assumed, that src and dst are same size, since this is only used for reservoir duplication
void paint_grid_copy(
    RWStructuredBuffer<ColumnInfo> src_pg_info, RWStructuredBuffer<Paint> src_pg_content, uint2 src_pg_size, uint2 src_pos,
    RWStructuredBuffer<ColumnInfo> dst_pg_info, RWStructuredBuffer<Paint> dst_pg_content, uint2 dst_pg_size, uint2 dst_pos)
{
    dst_pg_info[XY(dst_pos.x, dst_pos.y, dst_pg_size.x)] = src_pg_info[XY(src_pos.x, src_pos.y, src_pg_size.x)];

    for (uint z = 0; z < src_pg_info[XY(src_pos.x, src_pos.y, src_pg_size.x)].size; z++)
    {
        dst_pg_content[XYZ(dst_pos.x, dst_pos.y, z, dst_pg_size)] = src_pg_content[XYZ(src_pos.x, src_pos.y, z, src_pg_size)];
    }
}

void paint_grid_fill(
    RWStructuredBuffer<ColumnInfo> pg_info, RWStructuredBuffer<Paint> pg_content, uint2 pg_size,
    uint2 pos, Paint p)
{
    Paint left = p;
    while (!is_empty(left) && !column_is_full(pg_info[XY(pos.x, pos.y, pg_size.x)]))
    {
        uint z = pg_info[XY(pos.x, pos.y, pg_size.x)].write_index;
        Paint top = pg_content[XYZ(pos.x, pos.y, z, pg_size)];

        // try fill up top
        float fits_into_top = PAINT_UNIT() - top.volume;
        Paint element_part;
        element_part.color = left.color;
        element_part.volume = min(fits_into_top, left.volume);

        Paint updated_top = mix(top, element_part);

        pg_content[XYZ(pos.x, pos.y, z, pg_size)] = updated_top;
        pg_info[XY(pos.x, pos.y, pg_size.x)].volume += element_part.volume;
        left.volume -= element_part.volume;

        // update column info
        bool top_empty_before = is_empty(top);
        if (top_empty_before)
        {
            pg_info[XY(pos.x, pos.y, pg_size.x)].size++;
        }
        bool top_was_filled = floats_equal(updated_top.volume, PAINT_UNIT());
        if (top_was_filled)
        {
            pg_info[XY(pos.x, pos.y, pg_size.x)].write_index++;
        }
    }
}

void paint_grid_push(
    RWStructuredBuffer<ColumnInfo> pg_info, RWStructuredBuffer<Paint> pg_content, uint2 pg_size,
    uint2 pos, Paint element)
{
    if (!column_is_full(pg_info[XY(pos.x, pos.y, pg_size.x)]) && !is_empty(element))
    {
        uint z = pg_info[XY(pos.x, pos.y, pg_size.x)].size;
        pg_content[XYZ(pos.x, pos.y, z, pg_size)] = element;

        pg_info[XY(pos.x, pos.y, pg_size.x)].size++;
        pg_info[XY(pos.x, pos.y, pg_size.x)].write_index = pg_info[XY(pos.x, pos.y, pg_size.x)].size;
        pg_info[XY(pos.x, pos.y, pg_size.x)].volume += element.volume;
    }
}

void paint_grid_reverse_transfer(
    RWStructuredBuffer<ColumnInfo> src_pg_info, RWStructuredBuffer<Paint> src_pg_content, uint2 src_pg_size, uint2 src_pos,
    RWStructuredBuffer<ColumnInfo> dst_pg_info, RWStructuredBuffer<Paint> dst_pg_content, uint2 dst_pg_size, uint2 dst_pos)
{
    for (uint z = 0; z < src_pg_info[XY(src_pos.x, src_pos.y, src_pg_size.x)].size; z++)
    {
        Paint p = src_pg_content[XYZ(src_pos.x, src_pos.y, z, src_pg_size)];
        paint_grid_fill(dst_pg_info, dst_pg_content, dst_pg_size, dst_pos, p);
    }
}

// it is also assumed, that cells are only emptied from top to bottom (write_index update)
void paint_grid_delete(
    RWStructuredBuffer<ColumnInfo> pg_info, RWStructuredBuffer<Paint> pg_content, uint2 pg_size,
    uint3 delete_pos, float delete_volume)
{
    Paint available = pg_content[XYZ(delete_pos.x, delete_pos.y, delete_pos.z, pg_size)];
    float to_be_deleted = min(delete_volume, available.volume);
    pg_info[XY(delete_pos.x, delete_pos.y, pg_size.x)].volume -= to_be_deleted;
    pg_content[XYZ(delete_pos.x, delete_pos.y, delete_pos.z, pg_size)].volume -= to_be_deleted;

    bool cell_emptied = floats_equal(available.volume, to_be_deleted);
    if (to_be_deleted > 0 && cell_emptied)
    {
        pg_info[XY(delete_pos.x, delete_pos.y, pg_size.x)].size--;
        pg_info[XY(delete_pos.x, delete_pos.y, pg_size.x)].write_index = delete_pos.z;
    }
}

Paint paint_grid_get(RWStructuredBuffer<ColumnInfo> pg_info, RWStructuredBuffer<Paint> pg_content, uint2 pg_size, uint3 get_pos)
{
    return pg_content[XYZ(get_pos.x, get_pos.y, get_pos.z, pg_size)];
}