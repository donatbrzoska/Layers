using UnityEngine;

public class RakelPressureController : InputFieldController
{
    public GameObject _InputManager;
    public GameObject _PressureSink;

    private InputManager InputManager;
    private PressureSink PressureSink;

    new void Awake()
    {
        base.Awake();
        InputManager = _InputManager.GetComponent<InputManager>();
        PressureSink = _PressureSink.GetComponent<PressureSink>();
    }

    public void Start()
    {
        InputField.SetTextWithoutNotify("" + InputManager.Pressure);
    }

    public void Update()
    {
        if (!InputManager.UsingScriptPressure)
        {
            InputField.SetTextWithoutNotify("" + InputManager.Pressure);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        PressureSink.Pressure = value;
    }
}