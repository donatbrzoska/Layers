using UnityEngine;

public class RakelTiltController : InputFieldController
{
    public GameObject _InputManager;
    public GameObject _TiltSink;

    private InputManager InputManager;
    private TiltSink TiltSink;

    new void Awake()
    {
        base.Awake();
        InputManager = _InputManager.GetComponent<InputManager>();
        TiltSink = _TiltSink.GetComponent<TiltSink>();
    }

    public void Start()
    {
        InputField.SetTextWithoutNotify("" + InputManager.Tilt);
    }

    public void Update()
    {
        if (!InputManager.UsingScriptTilt)
        {
            InputField.SetTextWithoutNotify("" + InputManager.Tilt);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        TiltSink.Tilt = value;
    }
}