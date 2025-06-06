using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RestartButtonHandler : MonoBehaviour
{
    private void OnEnable()
    {
        RestartManager manager = FindFirstObjectByType<RestartManager>();

        if (manager == null)
        {
            Debug.LogError("RestartManager が見つかりません。");
            return;
        }

        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => manager.RestartFromLogin());

        Debug.Log($"{gameObject.name} に RestartManager をバインドしました。");
    }
}
