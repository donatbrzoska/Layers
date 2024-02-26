public interface PositionXSource
{
    float PositionX { get; }
}

public interface PositionYSource
{
    float PositionY { get; }
}

public interface PositionBaseZSource
{
    float PositionBaseZ { get; }
}

public interface PressureSource
{
    float Pressure { get; }
}

public interface RotationSource
{
    float Rotation { get; }
}

public interface TiltSource
{
    float Tilt { get; }
}

public interface StrokeStateSource
{
    bool StrokeBegin { get; }
    bool InStroke { get; }
}