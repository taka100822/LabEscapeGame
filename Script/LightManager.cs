using UnityEngine;
using Photon.Pun;

public class LightManager : MonoBehaviourPun
{
    [SerializeField] GameObject lightsParent;
    private GameManager gameManager;
    private int activateStep = 1;      // このコードが有効になるGameStep
    private Light[] lights;
    private bool isLight = false;
    [PunRPC] bool isButtonAPressed = false;
    [PunRPC] bool isButtonBPressed = false;
    [PunRPC] bool lightsActivated = false;

    void Start()
    {
        lights = lightsParent.GetComponentsInChildren<Light>();
        SetLights(false);
    }

    public void SetButtonAState(bool state)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            photonView.RPC(nameof(UpdateButtonAState), RpcTarget.AllBuffered, state);
    }

    public void SetButtonBState(bool state)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            photonView.RPC(nameof(UpdateButtonBState), RpcTarget.AllBuffered, state);
    }

    [PunRPC]
    void UpdateButtonAState(bool state)
    {
        isButtonAPressed = state;
        TryActivateLights();
    }

    [PunRPC]
    void UpdateButtonBState(bool state)
    {
        isButtonBPressed = state;
        TryActivateLights();
    }

    void TryActivateLights()
    {
        if (lightsActivated) return;

        if (isButtonAPressed && isButtonBPressed)
        {
            lightsActivated = true;
            SetLights(true);
            // SituationTextManager.Instance.ShowMessage("電気がついた");
            SituationTextManager.Instance.ShowMessage("lights_on");
            Debug.Log("両方のボタンが押されました．ライトをONにしました");
            if(!isLight)
            {
                GameManager.Instance.GoNextStep();
                isLight = true;
            }
        }
    }

    void Update()
    {
        if (GameManager.Instance == null) return;  // nullチェック追加

        if (GameManager.Instance.GetGameStep() == activateStep) // GameStepが1なら
        {
            if (Input.GetKey("l"))
            {
                SetLights(true);
                Debug.Log("lが押されました．ライトをONにしました");
                // SituationTextManager.Instance.ShowMessage("隠しコマンドで電気がついた");
                SituationTextManager.Instance.ShowMessage("lights_on");
                if (!isLight)
                {
                    GameManager.Instance.GoNextStep();
                    isLight = true;
                }
            }
        }
    }

    void SetLights(bool state)
    {
        foreach (var light in lights)
        {
            light.enabled = state;
        }
    }
}
