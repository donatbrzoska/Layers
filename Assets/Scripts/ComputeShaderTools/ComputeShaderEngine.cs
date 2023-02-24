using System.Collections.Generic;

public class ComputeShaderEngine
{
    private Queue<ComputeShaderTask> ComputeShaderTasks;

    public ComputeShaderEngine()
    {
        ComputeShaderTasks = new Queue<ComputeShaderTask>();
    }

    public void EnqueueOrRun(ComputeShaderTask cst)
    {
        if (ComputeShaderTasks != null)
        {
            ComputeShaderTasks.Enqueue(cst);
        }
        else
        {
            cst.Run();
        }
    }

    public void ProcessTasks(int n = int.MaxValue)
    {
        if (ComputeShaderTasks != null)
        {
            while (ComputeShaderTasks.Count > 0 && n-- >= 0)
            {
                ComputeShaderTask cst = ComputeShaderTasks.Dequeue();
                cst.Run();
            }
        }
    }
}
