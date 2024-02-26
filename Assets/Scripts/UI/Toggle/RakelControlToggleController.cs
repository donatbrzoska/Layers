using UnityEngine;

public abstract class RakelControlToggleController : ToggleController
{
    public GameObject _InputManager;
    protected InputManager InputManager;

    new void Awake()
    {
        base.Awake();
        InputManager = _InputManager.GetComponent<InputManager>();
    }
}
