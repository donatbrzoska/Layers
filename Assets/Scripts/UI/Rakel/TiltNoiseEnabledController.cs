
public class TiltNoiseEnabledController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Config.RakelConfig.TiltNoiseEnabled);
    }

    override public void OnValueChanged(bool enabled)
    {
        OilPaintEngine.UpdateRakelTiltNoiseEnabled(enabled);
    }
}
