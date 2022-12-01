using System.Collections.Concurrent;
using UnityEngine;

public class RakelDrawer
{
    //private IRakel Rakel;
    //private RenderTexture DrawingTarget;
    //private BlockingCollection<Node> NodeSource;

    //public RakelDrawer(IRakel rakel, RenderTexture drawingTarget, BlockingCollection<Node> nodeSource)
    //{
    //    Rakel = rakel;
    //    DrawingTarget = drawingTarget;
    //    NodeSource = nodeSource;
    //}

    //public void Start()
    //{
    //    foreach (var result in NodeSource.GetConsumingEnumerable())
    //    {
    //        //Debug.Log(string.Format("Applying with {0} {1} {2}" + result.Position, result.Rotation, result.Tilt).ToString());
    //        Rakel.Apply(result.Position, result.Rotation, result.Tilt, DrawingTarget);
    //    }
    //}

    //public void Stop()
    //{
    //    NodeSource.CompleteAdding();
    //}
}
