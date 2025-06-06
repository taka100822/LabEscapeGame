using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; //Photonサーバーの情報を使用するため
using System;

public class GameManager : MonoBehaviourPunCallbacks //Photon viewやPunを使用するため
{
    public static GameManager Instance { get; private set; }
    public float MouseSensitivity { get; private set; } = 1.8f;   // PlayerControllerのMouseSensitivity用
    // イベント定義
    public event Action<float> OnMouseSensitivityChanged;

    [SerializeField] GameObject playerPrefab_I;
    [SerializeField] GameObject playerPrefab_T;
    [SerializeField] Text GameStepText;
    [SerializeField] CheckUIManager checkUIManager;

    private int GameStep = 1;
    private float cooldown = 1f; // 隠しコマンドクールタイム
    private float nextAvailableTime = 0f;
    private bool isUIActive = false;

    void Awake()
    {
        // シングルトンの設定
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) //サーバーに接続していたら
        {
            if (playerPrefab_I != null)
            {
                Vector3 spawnPosition = new Vector3(0.6f, 1.2f, -4.9f);
                PhotonNetwork.Instantiate(playerPrefab_I.name, spawnPosition, Quaternion.identity); //Photonを介した生成
            }

            // 自分以外のAudioListenerを無効化する
            if (!photonView.IsMine)
            {
                // 自分のオブジェクトでなければAudioListenerを無効化
                GetComponentInChildren<AudioListener>().enabled = false;
                // 必要ならカメラも無効に
                GetComponentInChildren<Camera>().enabled = false;
            }

            // SituationTextManager.Instance.ShowLongMessage("卒論を提出しないと......");
            SituationTextManager.Instance.ShowLongMessage("thesis_submit_hint");
        }

        GameStep = 1;
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // SituationTextManager.Instance.ShowLongMessage("卒論を提出しないと......");
            SituationTextManager.Instance.ShowLongMessage("thesis_submit_hint");

            // 各クライアントが自分のプレイヤを生成する
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        if (playerPrefab_T != null && PhotonNetwork.IsConnected)
        {
            Vector3 spawnPosition = new Vector3(0f, 1.2f, -4.5f);
            PhotonNetwork.Instantiate(playerPrefab_T.name, spawnPosition, Quaternion.identity);
        }
    }

    public void GoNextStep()
    {
        GameStep++;
        Debug.Log("GameStepは " + GameStep.ToString() + " に進みました. ");
    }

    public int GetGameStep()
    {
        return GameStep;
    }

    public bool CheckUIActive()
    {
        isUIActive = checkUIManager.CheckUIActive();
        return isUIActive;

    }

    void Update()
    {
        if (Input.GetKey("g"))
        {
            if (Time.time >= nextAvailableTime)
            {
                nextAvailableTime = Time.time + cooldown;
                GoNextStep();
                Debug.Log("gが押されました．GameStepを進めました");
                // SituationTextManager.Instance.ShowMessage("隠しコマンドでGameStepが進んだ");
                SituationTextManager.Instance.ShowMessage("game_step_advance");
            }
        }

        GameStepText.text = "Game Progress: " + GameStep.ToString();
    }

    public void OnMouseSensitivitySliderChanged(float value)
    {
        MouseSensitivity = value;
        OnMouseSensitivityChanged?.Invoke(value); // 通知
    }

    public static void ResetInstance()
    {
        Instance = null;
    }
}