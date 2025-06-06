using Photon.Pun;
using UnityEngine;

public class DrawerManager : MonoBehaviourPun
{
    public Animator drawerAnimator;
    private GameManager gameManager;
    private int activateStep = 3;               // このコードが有効になるGameStep
    private bool isDrawerPressed = false;       // Drawerがクリックされているか
    private bool checkDrawerPressed = false;    // Drawerが同時にクリックされているか
    private bool isDrawerOpened = false;        // Drawerが開いているか
    private bool isDrawerEverOpened = false;    // Drawerが開いたことがあるか

    void OnMouseDown()
    {
        if (isDrawerEverOpened == false) // 一度も開いていないなら
        {
            if (GameManager.Instance.GetGameStep() >= activateStep) // GameStepが3以上なら
            {
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                {
                    isDrawerPressed = true; // 全員に押していることを通知
                    photonView.RPC("RPC_DrawerPressed", RpcTarget.AllBuffered, isDrawerPressed);
                }
            }
        }
        else
        {
            // 開いたことがあるならすぐに開閉
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                photonView.RPC("RPC_DrawerUpdate", RpcTarget.AllBuffered);
        }

    }

    void OnMouseUp()
    {
        if (isDrawerEverOpened == false) // 一度も開いていないなら
        {
            if(GameManager.Instance.GetGameStep() >= activateStep) // GameStepが3以上なら
            {
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                {
                    isDrawerPressed = false; // 全員に押していないことを通知
                    photonView.RPC("RPC_DrawerPressed", RpcTarget.AllBuffered, isDrawerPressed);
                }
            }
        }
    }

    [PunRPC]
    public void RPC_DrawerPressed(bool isDrawerPressed)
    {
        if (checkDrawerPressed && isDrawerPressed) // 押している通知2回で入る
        {
            photonView.RPC("RPC_DrawerUpdate", RpcTarget.AllBuffered);

            if (isDrawerEverOpened == false)
            {
                isDrawerEverOpened = true;
                GameManager.Instance.GoNextStep();
            }
        }
        else
        {
            checkDrawerPressed = isDrawerPressed; // 現在の状態を更新
        }
    }

    [PunRPC]
    public void RPC_DrawerUpdate()
    {
        isDrawerOpened = !isDrawerOpened; 
        drawerAnimator.SetBool("isOpen", isDrawerOpened);

        SoundManager.Instance.PlaySE(SESoundData.SE.Withdraw);

        if (isDrawerOpened)
        {
            // SituationTextManager.Instance.ShowMessage("引き出しを開けた");
            SituationTextManager.Instance.ShowMessage("drawer_opened");
        }
        else
        {
            // SituationTextManager.Instance.ShowMessage("引き出しを閉めた");
            SituationTextManager.Instance.ShowMessage("drawer_closed");
        }
    }

    void Update()
    {
        if (GameManager.Instance == null) return;  // nullチェック追加

        if (GameManager.Instance.GetGameStep() >= activateStep) // GameStepが3以上なら
        {
            if (Input.GetKey("o"))
            {
                if (isDrawerEverOpened == false) // 一度も開いていないなら
                {
                    isDrawerOpened = !isDrawerOpened;
                    drawerAnimator.SetBool("isOpen", isDrawerOpened);
                    isDrawerEverOpened = true;

                    // SituationTextManager.Instance.ShowMessage("引き出しを開けた");
                    SituationTextManager.Instance.ShowMessage("drawer_opened");
                    Debug.Log("oボタンが押されました．Drawerを開きました");
                    GameManager.Instance.GoNextStep();
                }
            }
        }
    }
}