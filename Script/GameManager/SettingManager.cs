using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;  // 設定メニューのUI
    [SerializeField] private Text SettingText;  // 設定メニューのUI
    [SerializeField] private GameObject openSettingsText;  // 設定メニューのUI
    [SerializeField] private GameObject closeSettingsText;  // 設定メニューのUI



    private bool isActiveWindow = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.DiplaySetting);

            // 表示状態を反転させる
            isActiveWindow = !isActiveWindow;
            settingsMenu.SetActive(isActiveWindow);

            // カーソルの表示・ロック状態を切り替え
            if (isActiveWindow)
            {
                Cursor.lockState = CursorLockMode.None;  // ロック解除
                Cursor.visible = true;                  // カーソル表示
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;  // ロック
                Cursor.visible = false;                    // カーソル非表示
            }
        }

        // テキスト更新
        if (isActiveWindow)
        {
            // SettingText.text = "Eキーで設定を閉じる";
            openSettingsText.SetActive(false);
            closeSettingsText.SetActive(true);
        }
        else
        {
            // SettingText.text = "Eキーで設定を開く";
            openSettingsText.SetActive(true);
            closeSettingsText.SetActive(false);
        }
    }
}
