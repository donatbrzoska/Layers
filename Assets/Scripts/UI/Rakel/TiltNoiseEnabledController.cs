
public class TiltNoiseEnabledController : ConfigToggleController
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
