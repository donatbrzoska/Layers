using UnityEngine;

public class RakelPositionXController : InputFieldController
{
    public GameObject _InputManager;
    public GameObject _PositionXSink;

    private InputManager InputManager;
    private PositionXSink PositionXSink;

    new void Awake()
    {
        base.Awake();
        InputManager = _InputManager.GetComponent<InputManager>();
        PositionXSink = _PositionXSink.GetComponent<PositionXSink>();
    }

    public void Start()
    {
        InputField.SetTextWithoutNotify("" + InputManager.PositionX);
    }

    public void Update()
    {
        if (!InputManager.UsingScriptPositionX)
        {
            InputField.SetTextWithoutNotify("" + InputManager.PositionX);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        PositionXSink.PositionX = value;
    }
}