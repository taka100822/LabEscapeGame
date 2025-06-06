using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PostItManager : MonoBehaviourPun
{
    [SerializeField] GameObject postItWindow;

    private bool isWindowOpen = false;
    private int activateStep = 2;
    // private bool isSeePostIt = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // ウィンドウが開いている場合
        if (isWindowOpen)
        {
            // UIをクリックしていれば閉じない
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // ゲーム空間をクリックしたときは閉じる
            if (!Physics.Raycast(ray, out RaycastHit _))
            {
                CloseWindow();
                return;
            }

            CloseWindow();
            return;
        }

        // ウィンドウが閉じているとき，対象をクリックすれば開く
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TryOpenPostIt(hit);
        }
    }

    void TryOpenPostIt(RaycastHit hit)
    {
        if (hit.transform.name != this.transform.name) return;

        if (GameManager.Instance.GetGameStep() <= 1)
        {
            // SituationTextManager.Instance.ShowMessage("暗くて見えない");
            SituationTextManager.Instance.ShowMessage("dark");

            return;
        }

        OpenPostIt();

        if (activateStep == GameManager.Instance.GetGameStep())
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                photonView.RPC("RPC_SeePostIt", RpcTarget.AllBuffered);
            }
            else
            {
                RPC_SeePostIt();
            }
        }
    }

    void OpenPostIt()
    {
        postItWindow.SetActive(true);
        isWindowOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// ウィンドウを閉じてカーソルを隠す
    /// </summary>
    private void CloseWindow()
    {
        postItWindow.SetActive(false);
        isWindowOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [PunRPC]
    public void RPC_SeePostIt()
    {
        GameManager.Instance.GoNextStep();
    }
}
