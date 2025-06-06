using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LauncherManager : MonoBehaviourPunCallbacks
{
    public GameObject LoginPanel;
    public InputField playerNameInput;

    public GameObject ConnectingPanel;
    public GameObject LobbyPanel;

    private string SceneName;

    #region Unity Methods

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        LoginPanel.SetActive(true);
        ConnectingPanel.SetActive(false);
        LobbyPanel.SetActive(false);
    } 
    #endregion

    #region Public Methods

    public void ConnectToPhotonServer() //LoginButtonで呼ぶ
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);

        if (!PhotonNetwork.IsConnected) //サーバーに接続していたら
        {
            string playerName = playerNameInput.text;
            if (!string.IsNullOrEmpty(playerName))
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
                ConnectingPanel.SetActive(true);
                LoginPanel.SetActive(false);
            }
        }
        else { }
    }

    public void JoinRandomRoom() //StatButtonで呼ぶ
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);
        PhotonNetwork.JoinRandomRoom(); 
    }
    #endregion
    

    #region Photon Callbacks

    public override void OnConnectedToMaster() //ログインしたら呼ばれる
    {
        Debug.Log(PhotonNetwork.NickName+ "Connected to Photon server");
        LobbyPanel.SetActive(true);
        ConnectingPanel.SetActive(false);

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateAndJoinRoom(); //ルームがなければ自ら作って入る
    }

    public override void OnJoinedRoom() //ルームに入ったら呼ばれる
    {
        Debug.Log(PhotonNetwork.NickName+ "joined to"+ PhotonNetwork.CurrentRoom.Name);
        
        PhotonNetwork.LoadLevel(SceneName); //シーンをロード
    }

    public void SetName_Main()
    {
        SceneName = "LabScene";
    }

    public void SetName_Tani()
    {
        SceneName = "TaniScene";
    }

    public void SetName_Watanabe()
    {
        SceneName = "WatanabeScene";
    }

    public void SetName_Sasaki()
    {
        SceneName = "SasakiScene";
    }

    public void SetName_Ishikawa()
    {
        SceneName = "IshikawaScene";
    }

        public void SetName_Funaki()
    {
        SceneName = "FunakiScene";
    }

    #endregion

    #region Private Methods
    void CreateAndJoinRoom()
    {
        //自動で作られるルームの設定
        string roomName = "Room" + Random.Range(0, 10000); //ルーム名
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;//ルームの定員
        PhotonNetwork.CreateRoom(roomName,roomOptions);
    }
    #endregion
}