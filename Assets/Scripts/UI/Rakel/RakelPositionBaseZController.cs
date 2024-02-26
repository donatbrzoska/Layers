using UnityEngine;

public class RakelPositionBaseZController : InputFieldController
{
    public GameObject _InputManager;
    public GameObject _PositionBaseZSink;

    private InputManager InputManager;
    private PositionBaseZSink PositionBaseZSink;

    new void Awake()
    {
        base.Awake();
        InputManager = _InputManager.GetComponent<InputManager>();
        PositionBaseZSink = _PositionBaseZSink.GetComponent<PositionBaseZSink>();
    }

    public void Start()
    {
        InputField.SetTextWithoutNotify("" + InputManager.PositionBaseZ);
    }

    public void Update()
    {
        if (!InputManager.UsingScriptPositionBaseZ)
        {
            InputField.SetTextWithoutNotify("" + InputManager.PositionBaseZ);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        PositionBaseZSink.PositionBaseZ = value;
    }
}