using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public enum ButtonType { A, B }
    [SerializeField] ButtonType type;
    [SerializeField] GameObject lightManager;

    private LightManager manager;
    private bool isPressed = false;

    void Start()
    {
        manager = lightManager.GetComponent<LightManager>();
    }

    void OnMouseDown()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Push);

        isPressed = true;
        SendState();
    }

    void OnMouseUp()
    {
        isPressed = false;
        SendState();
    }

    void SendState()
    {
        if (manager == null) return;

        if (type == ButtonType.A)
            manager.SetButtonAState(isPressed);
        else
            manager.SetButtonBState(isPressed);
    }
}
