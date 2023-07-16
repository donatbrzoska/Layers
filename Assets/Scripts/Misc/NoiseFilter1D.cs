using UnityEngine;

public class NoiseFilter1D
{
    private float Frequency;
    private float Amplitude; // describes how much can the value differ in each direction
    private WorldSpacePerlinNoise PerlinNoise;

    public NoiseFilter1D(float frequency, float amplitude)
    {
        Frequency = frequency;
        Amplitude = amplitude;

        PerlinNoise = new WorldSpacePerlinNoise(new Vector2(Frequency, 1));
    }

    public float Filter(float value, float x)
    {
        float noise = Amplitude * 2 * (PerlinNoise.ValueAt(x, 1) - 0.5f);
        return value + noise;
    }
}