using UnityEngine;

public class RakelRotationController : InputFieldController
{
    public GameObject _InputManager;
    public GameObject _RotationSink;

    private InputManager InputManager;
    private RotationSink RotationSink;

    new void Awake()
    {
        base.Awake();
        InputManager = _InputManager.GetComponent<InputManager>();
        RotationSink = _RotationSink.GetComponent<RotationSink>();
    }

    public void Start()
    {
        InputField.SetTextWithoutNotify("" + InputManager.Rotation);
    }

    public void Update()
    {
        if (!InputManager.UsingScriptRotation)
        {
            InputField.SetTextWithoutNotify("" + InputManager.Rotation);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        RotationSink.Rotation = value;
    }
}