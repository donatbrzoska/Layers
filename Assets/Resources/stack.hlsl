float PAINT_UNIT();
bool floats_equal(float a, float b);

struct StackInfo
{
    uint size;
    uint max_size;

    // This has a different meaning depending on the usage.
    // Therefore you cannot use a stack with both, push and raw_push
    uint write_index;

    float volume;
};

bool stack_is_full(StackInfo stack)
{
    return stack.write_index == stack.max_size;
}

// it is assumed, that src and dst are same size, since this is only used for reservoir duplication
void stack_2D_copy(
    RWStructuredBuffer<StackInfo> src_2D_info, RWStructuredBuffer<Paint> src_2D_content, uint2 src_2D_size, uint2 src_pos,
    RWStructuredBuffer<StackInfo> dst_2D_info, RWStructuredBuffer<Paint> dst_2D_content, uint2 dst_2D_size, uint2 dst_pos)
{
    dst_2D_info[XY(dst_pos.x, dst_pos.y, dst_2D_size.x)] = src_2D_info[XY(src_pos.x, src_pos.y, src_2D_size.x)];

    for (uint z = 0; z < src_2D_info[XY(src_pos.x, src_pos.y, src_2D_size.x)].size; z++)
    {
        dst_2D_content[XYZ(dst_pos.x, dst_pos.y, z, dst_2D_size)] = src_2D_content[XYZ(src_pos.x, src_pos.y, z, src_2D_size)];
    }
}

void stack_2D_push(
    RWStructuredBuffer<StackInfo> stack_2D_info, RWStructuredBuffer<Paint> stack_2D_content, uint2 stack_2D_size,
    uint2 pos, Paint element)
{
    Paint left = element;
    while (!is_empty(left) && !stack_is_full(stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)]))
    {
        uint z = stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].write_index;
        Paint top = stack_2D_content[XYZ(pos.x, pos.y, z, stack_2D_size)];

        // try fill up top
        float fits_into_top = PAINT_UNIT() - top.volume;
        Paint element_part;
        element_part.color = left.color;
        element_part.volume = min(fits_into_top, left.volume);

        Paint updated_top = mix(top, element_part);

        stack_2D_content[XYZ(pos.x, pos.y, z, stack_2D_size)] = updated_top;
        stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].volume += element_part.volume;
        left.volume -= element_part.volume;

        // update stack info
        bool top_empty_before = is_empty(top);
        if (top_empty_before)
        {
            stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].size++;
        }
        bool top_was_filled = floats_equal(updated_top.volume, PAINT_UNIT());
        if (top_was_filled)
        {
            stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].write_index++;
        }
    }
}

// void stack_2D_push_(
//     RWStructuredBuffer<StackInfo> stack_2D_info, RWStructuredBuffer<Paint> stack_2D_content, uint2 stack_2D_size,
//     uint2 pos, Paint element)
// {
//     if (!stack_is_full(stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)]) && !is_empty(element))
//     {
//         uint z = stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].write_index;
//         Paint top = stack_2D_content[XYZ(pos.x, pos.y, z, stack_2D_size)];

//         // try fill up top element
//         float fits_into_top = PAINT_UNIT() - top.volume;
//         Paint element_part_1;
//         element_part_1.color = element.color;
//         element_part_1.volume = min(fits_into_top, element.volume);

//         Paint updated_top = mix(top, element_part_1);

//         stack_2D_content[XYZ(pos.x, pos.y, z, stack_2D_size)] = updated_top;
//         stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].volume += element_part_1.volume;

//         // update stack info
//         bool top_empty_before = is_empty(top);
//         if (top_empty_before)
//         {
//             stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].size++;
//         }
//         bool top_was_filled = floats_equal(updated_top.volume, PAINT_UNIT());
//         if (top_was_filled)
//         {
//             stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].write_index++;
//         }

//         // add rest as new element
//         float remaining = max(element.volume - fits_into_top, 0);
//         if (remaining > 0 && !stack_is_full(stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)]))
//         {
//             if (top_was_filled)
//             {
//                 stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].size++;
//             }
//             Paint element_part_2;
//             element_part_2.color = element.color;
//             element_part_2.volume = remaining;

//             z = stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].write_index;
//             stack_2D_content[XYZ(pos.x, pos.y, z, stack_2D_size)] = element_part_2;
//             stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].volume += element_part_2.volume;
//         }
//     }
// }

void stack_2D_raw_push(
    RWStructuredBuffer<StackInfo> stack_2D_info, RWStructuredBuffer<Paint> stack_2D_content, uint2 stack_2D_size,
    uint2 pos, Paint element)
{
    if (!stack_is_full(stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)]) && !is_empty(element))
    {
        uint z = stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].size;
        stack_2D_content[XYZ(pos.x, pos.y, z, stack_2D_size)] = element;

        stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].size++;
        stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].write_index = stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].size;
        stack_2D_info[XY(pos.x, pos.y, stack_2D_size.x)].volume += element.volume;
    }
}

void stack_2D_reverse_transfer(
    RWStructuredBuffer<StackInfo> src_2D_info, RWStructuredBuffer<Paint> src_2D_content, uint2 src_2D_size, uint2 src_pos,
    RWStructuredBuffer<StackInfo> dst_2D_info, RWStructuredBuffer<Paint> dst_2D_content, uint2 dst_2D_size, uint2 dst_pos)
{
    for (uint z = 0; z < src_2D_info[XY(src_pos.x, src_pos.y, src_2D_size.x)].size; z++)
    {
        Paint element = src_2D_content[XYZ(src_pos.x, src_pos.y, z, src_2D_size)];
        stack_2D_push(dst_2D_info, dst_2D_content, dst_2D_size, dst_pos, element);
    }
}

// it is also assumed, that cells are only emptied from top to bottom (write_index update)
void stack_2D_delete(
    RWStructuredBuffer<StackInfo> stack_2D_info, RWStructuredBuffer<Paint> stack_2D_content, uint2 stack_2D_size,
    uint3 delete_pos, float delete_volume)
{
    Paint available = stack_2D_content[XYZ(delete_pos.x, delete_pos.y, delete_pos.z, stack_2D_size)];
    float to_be_deleted = min(delete_volume, available.volume);
    stack_2D_info[XY(delete_pos.x, delete_pos.y, stack_2D_size.x)].volume -= to_be_deleted;
    stack_2D_content[XYZ(delete_pos.x, delete_pos.y, delete_pos.z, stack_2D_size)].volume -= to_be_deleted;

    bool cell_emptied = floats_equal(available.volume, to_be_deleted);
    if (to_be_deleted > 0 && cell_emptied)
    {
        stack_2D_info[XY(delete_pos.x, delete_pos.y, stack_2D_size.x)].size--;
        stack_2D_info[XY(delete_pos.x, delete_pos.y, stack_2D_size.x)].write_index = delete_pos.z;
    }
}

Paint stack_2D_get(RWStructuredBuffer<StackInfo> stack_2D_info, RWStructuredBuffer<Paint> stack_2D_content, uint2 stack_2D_size, uint3 get_pos)
{
    return stack_2D_content[XYZ(get_pos.x, get_pos.y, get_pos.z, stack_2D_size)];
}