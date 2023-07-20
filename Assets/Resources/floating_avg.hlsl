uint XYZ(uint x, uint y, uint z, uint3 dimensions);

// always returns new avg value
float floating_avg_2D(RWStructuredBuffer<float> avg_ringbuffer, uint3 avg_ringbuffer_size, uint stroke_begin, uint2 pos, float new_value)
{
    uint OFFSET = 2; // actual buffer starts at index 2

    if (stroke_begin == 1)
    {
        float current_avg = new_value;
        uint pointer = 0;

        avg_ringbuffer[XYZ(pos.x, pos.y, 0, avg_ringbuffer_size)] = current_avg;
        avg_ringbuffer[XYZ(pos.x, pos.y, 1, avg_ringbuffer_size)] = (float)pointer;
        // init buffer with all same values
        for (uint i=0; i<avg_ringbuffer_size.z; i++)
        {
            avg_ringbuffer[XYZ(pos.x, pos.y, OFFSET+i, avg_ringbuffer_size)] = current_avg;
        }

        return new_value;
    }
    else
    {
        float current_avg = avg_ringbuffer[XYZ(pos.x, pos.y, 0, avg_ringbuffer_size)];
        uint pointer = (uint)avg_ringbuffer[XYZ(pos.x, pos.y, 1, avg_ringbuffer_size)];

        float removed = avg_ringbuffer[XYZ(pos.x, pos.y, OFFSET+pointer, avg_ringbuffer_size)];
        float new_avg = current_avg - removed/(float)avg_ringbuffer_size.z + new_value/(float)avg_ringbuffer_size.z;

        avg_ringbuffer[XYZ(pos.x, pos.y, 0, avg_ringbuffer_size)] = new_avg;
        avg_ringbuffer[XYZ(pos.x, pos.y, 1, avg_ringbuffer_size)] = (float)((pointer+1) % avg_ringbuffer_size.z);
        avg_ringbuffer[XYZ(pos.x, pos.y, OFFSET+pointer, avg_ringbuffer_size)] = new_value;

        return new_avg;
    }
}

// always returns new avg value
float floating_avg(RWStructuredBuffer<float> avg_ringbuffer, uint avg_ringbuffer_size, uint stroke_begin, float new_value)
{
    return floating_avg_2D(avg_ringbuffer, uint3(1, 1, avg_ringbuffer_size), stroke_begin, uint2(0,0), new_value);
}