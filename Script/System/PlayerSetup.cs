using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerSetUp : MonoBehaviourPunCallbacks
{
    
    [SerializeField] GameObject FPSCamera;
    [SerializeField] Text playerNameText;

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine) //このオブジェクトが自分がPhotonを介して生成したものならば
        {
            transform.GetComponent<PlayerMoveController>().enabled = true; //MovementController.csを有効にする
            FPSCamera.GetComponent<Camera>().enabled = true; //FPSCameraのCameraコンポーネントを有効にする
        }
        else
        {
            transform.GetComponent<PlayerMoveController>().enabled = false;
            FPSCamera.GetComponent<Camera>().enabled = false;
        }
        
        if(playerNameText!=null) //Textオブジェクトが空でなければ
        {
            playerNameText.text = photonView.Owner.NickName; //ログインした名前を代入
        }
    }
}