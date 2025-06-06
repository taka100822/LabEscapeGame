using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class StaplerBoxBoxManager : MonoBehaviourPun
{
    [Header("UI・オブジェクト参照")]
    [SerializeField] GameObject staplerBoxWindow;
    [SerializeField] StaplerBoxPassButton[] passwordButtons = default;
    [SerializeField] int[] correctNumbers = default;
    [SerializeField] GameObject largeStapler;

    private bool isWindowOpen = false;
    private bool hasSeenHintPaper = false;

    void Update()
    {
        HandleMouseClick();
        UpdateCursorState();
    }

    /// <summary>
    /// マウスクリック時の処理
    /// </summary>
    private void HandleMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (isWindowOpen)
        {
            // UIのどこかをクリックした場合は閉じない
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // UIの外（=ゲーム空間）をクリックした場合は閉じる
            CloseWindow();
            return;
        }

        // UIが開いていない状態で，オブジェクトをクリックしたら開く
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.name == this.transform.name)
            {
                TryOpenWindow();
            }
        }
    }

    /// <summary>
    /// カーソルの表示状態を更新
    /// </summary>
    private void UpdateCursorState()
    {
        if (isWindowOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// ウィンドウを開こうとする処理
    /// </summary>
    private void TryOpenWindow()
    {
        if (isWindowOpen) return;

        if (GameManager.Instance.GetGameStep() <= 1)
        {
            // SituationTextManager.Instance.ShowMessage("暗くて見えない");
            SituationTextManager.Instance.ShowMessage("dark");
            return;
        }

        staplerBoxWindow.SetActive(true);
        isWindowOpen = true;

        if (!hasSeenHintPaper)
        {
            hasSeenHintPaper = true;
        }
    }

    /// <summary>
    /// ウィンドウを閉じてカーソルを隠す
    /// </summary>
    private void CloseWindow()
    {
        staplerBoxWindow.SetActive(false);
        isWindowOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// パスワードが正解ならアイテムを出す
    /// </summary>
    public void CheckClear()
    {
        if (!IsPasswordCorrect()) return;

        CloseWindow();

        SoundManager.Instance.PlaySE(SESoundData.SE.Correct);

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_Stapler", RpcTarget.AllBuffered);
        }
        else
        {
            // SituationTextManager.Instance.ShowMessage("大型ホッチキスが出てきた");
            SituationTextManager.Instance.ShowMessage("large_stapler_appeared");
            gameObject.SetActive(false);    // 箱を消す
            largeStapler.SetActive(true);   // 大型ホッチキスを出現
        }

    }

    /// <summary>
    /// 入力されたパスワードが正解か判定
    /// </summary>
    private bool IsPasswordCorrect()
    {
        for (int i = 0; i < correctNumbers.Length; i++)
        {
            if (passwordButtons[i].number != correctNumbers[i])
            {
                SoundManager.Instance.PlaySE(SESoundData.SE.Mistake);

                // SituationTextManager.Instance.ShowMessage("不正解のようだ...");
                SituationTextManager.Instance.ShowMessage("incorrect_password");
                return false;
            }
        }
        return true;
    }

    [PunRPC]
    public void RPC_Stapler()
    {
        // SituationTextManager.Instance.ShowMessage("大型ホッチキスが出てきた");
        SituationTextManager.Instance.ShowMessage("large_stapler_appeared");
        gameObject.SetActive(false);    // 箱を消す
        largeStapler.SetActive(true);   // 大型ホッチキスを出現
    }
}
