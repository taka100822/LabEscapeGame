using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;


public class SituationTextManager : MonoBehaviour
{
    public static SituationTextManager Instance { get; private set; }
    public float displayDuration = 3f;
    public float longDisplayDuration = 6f;

    private Text situationText;
    private Coroutine currentCoroutine;
    [SerializeField] private string tableName = "SituationTexts";

    void Awake()
    {
        // シングルトンの設定
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        situationText = GetComponent<Text>();
        if (situationText == null)
        {
            Debug.Log("StiationTextがうまく設定されていません");
        }
    }

    public void ShowMessage(string key)
    {
        StartCoroutine(ShowLocalizedMessage(key, displayDuration));
    }

    public void ShowLongMessage(string key)
    {
        StartCoroutine(ShowLocalizedMessage(key, longDisplayDuration));
    }

    private IEnumerator ShowLocalizedMessage(string key, float duration)
    {
        var tableLoading = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return tableLoading;

        var table = tableLoading.Result;
        if (table == null)
        {
            Debug.LogError("ローカライズテーブルが見つかりません: " + tableName);
            yield break;
        }

        var entry = table.GetEntry(key);
        if (entry == null)
        {
            Debug.LogWarning($"ローカライズキー '{key}' が見つかりませんでした");
            situationText.text = key; // フォールバックでキーを表示
        }
        else
        {
            situationText.text = entry.GetLocalizedString();
        }

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(HideMessageAfterDelay(duration));
    }

    // 動的にフォーマットされたメッセージを表示するためのメソッド
    public void ShowMessageFormatted(string key, params object[] args)
    {
        StartCoroutine(ShowLocalizedMessageFormatted(key, displayDuration, args));
    }

    public void ShowLongMessageFormatted(string key, params object[] args)
    {
        StartCoroutine(ShowLocalizedMessageFormatted(key, longDisplayDuration, args));
    }

    private IEnumerator ShowLocalizedMessageFormatted(string key, float duration, params object[] args)
    {
        var tableLoading = LocalizationSettings.StringDatabase.GetTableAsync(tableName);
        yield return tableLoading;

        var table = tableLoading.Result;
        if (table == null)
        {
            Debug.LogError("ローカライズテーブルが見つかりません: " + tableName);
            yield break;
        }

        var entry = table.GetEntry(key);
        if (entry == null)
        {
            Debug.LogWarning($"ローカライズキー '{key}' が見つかりませんでした");
            situationText.text = key; // フォールバックでキーを表示
        }
        else
        {
            situationText.text = entry.GetLocalizedString(args);
        }

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(HideMessageAfterDelay(duration));
    }

    private IEnumerator HideMessageAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        situationText.text = "";
        currentCoroutine = null;
    }
}
