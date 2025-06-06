using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Threading.Tasks;

public class LanguageSwitchButton : MonoBehaviour
{
    [SerializeField] private Button switchButton;
    [SerializeField] private Text buttonLabel;

    private bool isInitialized = false;

    private async void Start()
    {
        await LocalizationSettings.InitializationOperation.Task;

        if (switchButton != null)
        {
            switchButton.onClick.AddListener(OnLanguageSwitchClicked);
        }

        UpdateButtonLabel();
        isInitialized = true;
    }

    private void OnLanguageSwitchClicked()
    {
        if (!isInitialized) return;

        SoundManager.Instance.PlaySE(SESoundData.SE.Click);
        
        var currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        string nextLocaleCode = currentCode == "ja" ? "en" : "ja";

        ChangeLocale(nextLocaleCode);
    }

    private async void ChangeLocale(string localeCode)
    {
        await LocalizationSettings.InitializationOperation.Task;
        LocalizationSettings.SelectedLocale = Locale.CreateLocale(localeCode);
        UpdateButtonLabel(); // 切り替え後にラベルを更新
    }

    private void UpdateButtonLabel()
    {
        var currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;

        // ボタンには「次に切り替える言語」を表示
        buttonLabel.text = currentCode == "ja" ? "English" : "日本語";
    }
}
