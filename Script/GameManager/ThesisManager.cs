using UnityEngine;
using Photon.Pun;

public class ThesisManager : MonoBehaviourPun
{
    [SerializeField] private GameObject thesis;
    private int activateStep = 7;

    void Update()
    {
        if (!thesis.activeInHierarchy && GameManager.Instance.GetGameStep() == activateStep)
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                photonView.RPC("RPC_NextStep", RpcTarget.AllBuffered);
            }
            else
            {
                thesis.SetActive(true);
            }
        }
    }

    [PunRPC]
    public void RPC_NextStep()
    {
        thesis.SetActive(true);
    }
}
