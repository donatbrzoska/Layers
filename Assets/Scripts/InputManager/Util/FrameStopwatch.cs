
public class FrameStopwatch
{
    private System.Diagnostics.Stopwatch sw;
    public float SecondsSinceLastFrame;

    public FrameStopwatch()
    {
        sw = new System.Diagnostics.Stopwatch();
        sw.Start();
    }

    public void Update()
    {
        sw.Stop();
        SecondsSinceLastFrame = (float)sw.Elapsed.Milliseconds / 1000;
        sw = new System.Diagnostics.Stopwatch();
        sw.Start();
    }
}
