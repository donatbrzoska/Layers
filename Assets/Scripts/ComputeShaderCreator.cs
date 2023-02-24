public abstract class ComputeShaderCreator
{
    protected ShaderRegionFactory ShaderRegionFactory;
    protected ComputeShaderEngine ComputeShaderEngine;

    public ComputeShaderCreator(ShaderRegionFactory shaderRegionFactory, ComputeShaderEngine computeShaderEngine)
    {
        ShaderRegionFactory = shaderRegionFactory;
        ComputeShaderEngine = computeShaderEngine;
    }
}
