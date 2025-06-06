using UnityEngine;
using Photon.Pun;

public class TemperatureQuestion : MonoBehaviourPun
{
    // [SerializeField] GameObject ShibuyaTalkBox;
    [SerializeField] GameObject AnswerButtons;
    [SerializeField] GameObject Thermometer;
    private int activateStep = 10;
    private double coatThreshold = 12.0f; // コートを着る基準温度
    private double now_temperature;
    private bool hasSeenButtons = false;
    private bool isCorrect;


    void Update()
    {
        if (GameManager.Instance == null) return;  // nullチェック追加

        if (GameManager.Instance.GetGameStep() == activateStep)
        {
            now_temperature = Thermometer.GetComponent<GetWeather>().Temperature;      // GetWeather.csから気温をもらう

            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                photonView.RPC("RPC_DisplayButtons", RpcTarget.AllBuffered);
            }
            else
            {
                RPC_DisplayButtons();
            }
        }
    }

    /// <summary>
    /// Yesのボタンを押したとき
    /// </summary>
    public void AnswerYes()
    {
        if (now_temperature < coatThreshold)
        {
            Debug.Log("now temperature is" + now_temperature.ToString() + " ○");
            isCorrect = true;
        }
        else
        {
            Debug.Log("now temperature is" + now_temperature.ToString() + " ×");
            isCorrect = false;
        }

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_BackGameMode", RpcTarget.AllBuffered, isCorrect);
        }
        else
        {
            RPC_BackGameMode(isCorrect);
        }
    }

    /// <summary>
    /// Noのボタンを押したとき
    /// </summary>
    public void AnswerNo()
    {
        if (now_temperature >= coatThreshold)
        {
            Debug.Log("now temperature is" + now_temperature.ToString() + " ○");
            isCorrect = true;
        }
        else
        {
            Debug.Log("now temperature is" + now_temperature.ToString() + " ×");
            isCorrect = false;
        }

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_BackGameMode", RpcTarget.AllBuffered, isCorrect);
        }
        else
        {
            RPC_BackGameMode(isCorrect);
        }
    }

    /// <summary>
    /// ボタン&カーソル表示の同期関数
    /// </summary>
    [PunRPC]
    public void RPC_DisplayButtons()
    {
        if (!hasSeenButtons)    // RPCの同期処理の順序都合上、ここでフラグの処理しないとうまく動作しない
        {
            AnswerButtons.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            hasSeenButtons = true;
        }
    }

    /// <summary>
    /// ボタン&カーソル非表示 + GameStepをあげる同期関数
    /// </summary>
    [PunRPC]
    private void RPC_BackGameMode(bool amswercorrect)
    {
        if (amswercorrect)
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.ShibuyaCorrect);
        }
        else
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.ShibuyaMistake);
        }
        
        if (GameManager.Instance.GetGameStep() == activateStep)
        {
            GameManager.Instance.GoNextStep();
        }

        AnswerButtons.SetActive(false);
        // カーソルをゲーム内へ
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
