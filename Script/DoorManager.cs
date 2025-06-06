using UnityEngine;
using Photon.Pun;

public class DoorManager : MonoBehaviourPun
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject goalCanvas; // 表示したいクリア画面
    private int activateStep = 12;
    private bool isDoorOpened = false;
    private bool isEndTriggered = false;


    void OnMouseDown()
    {
        if (GameManager.Instance.GetGameStep() != activateStep || isEndTriggered) return;

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_ToggleDoor", RpcTarget.AllBuffered);
        }
        else
        {
            ToggleDoor();
        }
    }

    [PunRPC]
    private void RPC_ToggleDoor()
    {
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        if (isDoorOpened) return;

        isDoorOpened = true;
        SoundManager.Instance.PlaySE(SESoundData.SE.OpenDoor);

        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpenOutdoor", isDoorOpened);
        }

        isEndTriggered = true;
        FadeManager.Instance.FadeOut(() =>
        {
            // カーソル表示＆ロック解除
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            goalCanvas.SetActive(true);
            FadeManager.Instance.FadeIn();
            
            SoundManager.Instance.PlaySE(SESoundData.SE.ShibuyaVoice);
        });
    }
}
