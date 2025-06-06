using UnityEngine;

public class CheckUIManager : MonoBehaviour
{
    [SerializeField] Canvas settingCanvas;
    [SerializeField] Canvas popupCanvas;
    
    public bool CheckUIActive()
    {
        if (HasAnyActiveChild(settingCanvas) || HasAnyActiveChild(popupCanvas))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private bool HasAnyActiveChild(Canvas parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}
