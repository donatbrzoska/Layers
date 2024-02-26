using UnityEngine;

public abstract class ConfigToggleController : ToggleController
{
    protected OilPaintEngine OilPaintEngine;

    new void Awake()
    {
        base.Awake();
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();
    }

    protected void MakeNonInteractableInEvaluateMode()
    {
        if (OilPaintEngine.EVALUATE)
        {
            Toggle.interactable = false;
        }
    }
}
