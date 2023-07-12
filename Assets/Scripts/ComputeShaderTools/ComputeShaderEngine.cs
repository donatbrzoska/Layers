using System.Collections.Generic;

public class ComputeShaderEngine
{
    private Queue<ComputeShaderTask> ComputeShaderTasks;

    public ComputeShaderEngine(bool delayedExecution)
    {
        if (delayedExecution)
        {
            ComputeShaderTasks = new Queue<ComputeShaderTask>();
        }
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

    public void ProcessTasks(int n)
    {
        if (ComputeShaderTasks != null)
        {
            while (n-- >= 0 && ComputeShaderTasks.Count > 0)
            {
                ComputeShaderTask cst = ComputeShaderTasks.Dequeue();
                cst.Run();
            }
        }
    }
}