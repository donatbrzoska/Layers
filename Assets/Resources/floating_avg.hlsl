// always returns new avg value
float floating_avg(RWStructuredBuffer<float> avg_ringbuffer, uint avg_ringbuffer_size, uint stroke_begin, float new_value)
{
    uint OFFSET = 2; // actual buffer starts at index 2

    if (stroke_begin == 1)
    {
        float current_avg = new_value;
        uint pointer = 0;

        avg_ringbuffer[0] = current_avg;
        avg_ringbuffer[1] = (float)pointer;
        // init buffer with all same values
        for (uint i=0; i<avg_ringbuffer_size; i++)
        {
            avg_ringbuffer[OFFSET+i] = current_avg;
        }

        return new_value;
    }
    else
    {
        float current_avg = avg_ringbuffer[0];
        uint pointer = (uint)avg_ringbuffer[1];

        float removed = avg_ringbuffer[OFFSET+pointer];
        float new_avg = current_avg - removed/(float)avg_ringbuffer_size + new_value/(float)avg_ringbuffer_size;

        avg_ringbuffer[0] = new_avg;
        avg_ringbuffer[1] = (float)((pointer+1) % avg_ringbuffer_size);
        avg_ringbuffer[OFFSET+pointer] = new_value;

        return new_avg;
    }
}