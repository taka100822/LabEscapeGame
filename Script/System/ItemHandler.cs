using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ItemHandler : MonoBehaviourPun
{
    void Update()
    {
        // マウス左クリック
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // 取得可能アイテムに当たった場合
                ItemCollectable item = hit.collider.GetComponent<ItemCollectable>();

                if (item != null)
                {
                    PhotonView targetView = item.GetComponent<PhotonView>();

                    if (targetView != null)
                    {
                        // 取得したアイテムを右上に表示
                        SoundManager.Instance.PlaySE(SESoundData.SE.GetItem);

                        item.itemImage.SetActive(true);
                        // SituationTextManager.Instance.ShowMessage(item.transform.name + "を入手");
                        string itemName = item.transform.name;
                        SituationTextManager.Instance.ShowMessageFormatted("get_item", itemName);

                        if (item.transform.name == "Thesis")
                        {
                            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                            {
                                photonView.RPC("RPC_GetThesis", RpcTarget.AllBuffered);
                            }
                            else
                            {
                                GameManager.Instance.GoNextStep();
                            }
                        }

                        // オフラインでもオブジェクトが消えるように
                        hit.collider.gameObject.SetActive(false);

                        // アイテムの取得を同期
                        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                            photonView.RPC("RPC_ItemCollected", RpcTarget.All, targetView.ViewID);
                    }
                }
            }
        }
    }

    // アイテムの取得を他のプレイヤーにも通知
    [PunRPC]
    public void RPC_ItemCollected(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);

        if (targetView != null)
        {
            targetView.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void RPC_GetThesis()
    {
        GameManager.Instance.GoNextStep();
    }
}