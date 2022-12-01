using UnityEngine;

public interface IRakel
{
    public void Apply(Vector3 rakelPosition, float rakelRotation, float rakelTilt, RenderTexture target);
}
