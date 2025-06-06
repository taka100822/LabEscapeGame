using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class RestartManager : MonoBehaviour
{
    public static RestartManager Instance { get; private set; }

    [SerializeField] private string emptySceneName = "TempEmptyScene";
    [SerializeField] private string launcherSceneName = "LauncherScene";

    private void Awake()
    {
        // Singleton設定
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void RestartFromLogin()
    {
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        // 1. ルーム退出
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            while (PhotonNetwork.InRoom)
                yield return null;
        }

        // 2. サーバー切断
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
                yield return null;
        }

        // 5. 空シーン → 1フレーム待機
        yield return SceneManager.LoadSceneAsync(emptySceneName);
        yield return null;

        // 3. GameManager削除（シーンに存在しているか確認）
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
            GameManager.ResetInstance(); // 下記で定義
        }

        // 4. DontDestroyOnLoadのMessageCanvasを探して破棄
        GameObject messageCanvas = GameObject.Find("MessageCanvas");
        if (messageCanvas != null)
        {
            Destroy(messageCanvas);
        }

        // 6. タイトルシーンへ
        yield return SceneManager.LoadSceneAsync(launcherSceneName);
    }
}
