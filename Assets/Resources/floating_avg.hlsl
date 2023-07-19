// always returns new avg value
float floating_avg(RWStructuredBuffer<float> avg_ringbuffer, uint stroke_begin, float new_value)
{
    uint OFFSET = 3; // actual buffer starts at index 3

    if (stroke_begin == 1)
    {
        float current_avg = new_value;
        uint pointer = 0;
        uint size = (uint)avg_ringbuffer[2];

        avg_ringbuffer[0] = current_avg;
        avg_ringbuffer[1] = (float)pointer;
        // init buffer with all same values
        for (uint i=0; i<size; i++)
        {
            avg_ringbuffer[OFFSET+i] = current_avg;
        }

        return new_value;
    }
    else
    {
        float current_avg = avg_ringbuffer[0];
        uint pointer = (uint)avg_ringbuffer[1];
        uint size = (uint)avg_ringbuffer[2];

        float removed = avg_ringbuffer[OFFSET+pointer];
        float new_avg = current_avg - removed/(float)size + new_value/(float)size;

        avg_ringbuffer[0] = new_avg;
        avg_ringbuffer[1] = (float)((pointer+1) % size);
        avg_ringbuffer[OFFSET+pointer] = new_value;

        return new_avg;
    }
}