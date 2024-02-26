using UnityEngine;

public class RakelPositionYController : InputFieldController
{
    public GameObject _InputManager;
    public GameObject _PositionYSink;

    private InputManager InputManager;
    private PositionYSink PositionYSink;

    new void Awake()
    {
        base.Awake();
        InputManager = _InputManager.GetComponent<InputManager>();
        PositionYSink = _PositionYSink.GetComponent<PositionYSink>();
    }

    public void Start()
    {
        InputField.SetTextWithoutNotify("" + InputManager.PositionY);
    }

    public void Update()
    {
        if (!InputManager.UsingScriptPositionY)
        {
            InputField.SetTextWithoutNotify("" + InputManager.PositionY);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        PositionYSink.PositionY = value;
    }
}