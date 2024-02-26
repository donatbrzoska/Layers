using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public abstract class ToggleController : MonoBehaviour
{
    protected Toggle Toggle;

    public void Awake()
    {
        Toggle = GetComponent<Toggle>();
        Toggle.onValueChanged.AddListener(OnValueChanged);
    }

    public abstract void OnValueChanged(bool locked);
}
