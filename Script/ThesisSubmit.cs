using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ThesisSubmit : MonoBehaviourPun
{
    [SerializeField] Text itemDisplayText;
    [Header("製本済み論文のアイテム画像オブジェクト")][SerializeField] GameObject boundThesisImageObject;

    private int activateStep = 9;

    void OnMouseDown()
    {
        if (GameManager.Instance.GetGameStep() == activateStep && boundThesisImageObject.activeInHierarchy) // GameStepが9なら
        {
            Debug.Log("Mr.shibuyaに製本済み論文を渡した");
            boundThesisImageObject.SetActive(false);

            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                photonView.RPC("RPC_BoundThesisSubmit", RpcTarget.AllBuffered);
            }

            else
            {
                RPC_BoundThesisSubmit();
            }
        }
    }

    [PunRPC]
    public void RPC_BoundThesisSubmit()
    {
        // SituationTextManager.Instance.ShowMessage("Mr.shibuyaに製本済み論文を渡した");
        SituationTextManager.Instance.ShowMessage("thesis_submit");

        Debug.Log("GameStepは" + GameManager.Instance.GetGameStep() + "に進みました");
        GameManager.Instance.GoNextStep();
    }
}
