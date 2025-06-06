using UnityEngine;
using Photon.Pun;

public class StaplerManager : MonoBehaviourPun
{
    [SerializeField] GameObject thesisImageObject;
    [SerializeField] GameObject boundThesisImageObject;

    void OnMouseDown()
    {
        if (thesisImageObject.activeInHierarchy)
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.GetItem);

            thesisImageObject.SetActive(false);
            boundThesisImageObject.SetActive(true);

            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                photonView.RPC("RPC_Binding", RpcTarget.AllBuffered);
            }
            else
            {
                // SituationTextManager.Instance.ShowMessage("製本して製本済みの卒論を入手");
                SituationTextManager.Instance.ShowMessage("binding");
                GameManager.Instance.GoNextStep();
            }
        }
    }

    [PunRPC]
    public void RPC_Binding()
    {
        // SituationTextManager.Instance.ShowMessage("製本して製本済みの卒論を入手");
        SituationTextManager.Instance.ShowMessage("binding");

        GameManager.Instance.GoNextStep();
    }
}
