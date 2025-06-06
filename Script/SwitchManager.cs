using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SwitchManager : MonoBehaviourPun
{
    [SerializeField] Text itemDisplayText;
    [Header("スマブラのアイテム画像オブジェクト")][SerializeField] GameObject SmashBroCase;

    private int activateStep = 4;

    void OnMouseDown()
    {
        if (GameManager.Instance.GetGameStep() == activateStep && SmashBroCase.activeInHierarchy) // GameStepが4なら
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.Set);

            SmashBroCase.SetActive(false);

            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                photonView.RPC("RPC_SwitchItemUse", RpcTarget.AllBuffered);
            }
            else
            {
                RPC_SwitchItemUse();
            }
        }
    }

    [PunRPC]
    public void RPC_SwitchItemUse()
    {
        // SituationTextManager.Instance.ShowMessage("Switchにスマブラを入れた");
        SituationTextManager.Instance.ShowMessage("switch_item_use");
        Debug.Log("GameStepは" + GameManager.Instance.GetGameStep() + "に進みました");
        GameManager.Instance.GoNextStep();
    }
}
