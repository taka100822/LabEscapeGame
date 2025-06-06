using UnityEngine;
using Photon.Pun;
using System.Collections;

public class EndManager : MonoBehaviourPun
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Animator shibuchanAnimator;
    [SerializeField] private Transform shibuchanTransform;

    private int triggerStep = 11;
    private bool hasStartedSequence = false;

    void Update()
    {
        if (!hasStartedSequence && GameManager.Instance.GetGameStep() == triggerStep && PhotonNetwork.IsMasterClient)   // ホストのみが実行しあとは同期させる
        {
            hasStartedSequence = true;

            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                photonView.RPC("RPC_StartDoorSequence", RpcTarget.AllBuffered);
            }
            else
            {
                StartLeaveLabSequence();
            }
        }
    }

    [PunRPC]
    private void RPC_StartDoorSequence()
    {
        StartLeaveLabSequence();
    }

    /// <summary>
    /// 渋谷先生が研究室から出ていくアニメーションを再生する
    /// </summary>
    private void StartLeaveLabSequence()
    {
        StartCoroutine(LeaveLabSequenceRoutine());
    }

    /// <summary>
    /// 渋谷先生が研究室から出ていくアニメーションを再生する
    /// </summary>
    private IEnumerator LeaveLabSequenceRoutine()
    {
        // モデルの向きを -90 に設定
        shibuchanTransform.rotation = Quaternion.Euler(0f, -90f, 0f);
        yield return null;

        // ドア開くアニメーション再生
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("OpenIndoor");
        }

        // 少し待ってから歩き出す（アニメタイミングを調整）
        yield return new WaitForSeconds(1f);

        // 歩くアニメーション再生
        shibuchanAnimator.SetTrigger("Walk");

        // 数秒後にドアを閉じる
        yield return new WaitForSeconds(3f);
        CloseDoor();

        GameManager.Instance.GoNextStep();
    }

    /// <summary>
    /// ドアを閉じるアニメーションを再生する
    /// </summary>
    private void CloseDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("CloseIndoor");

            shibuchanTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
            shibuchanAnimator.SetTrigger("Walk");

        }
    }
}
