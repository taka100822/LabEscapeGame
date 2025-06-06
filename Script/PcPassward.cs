using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PcPassward : MonoBehaviourPun
{
    [SerializeField] GameObject selectFunakiPc;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject thesis;

    private bool isPcUsed = false; // PCが使用されているか
    public string correctPassword = "601";
    public GameObject feedbackText;
    private float displayDuration = 3f;
    private int activateStep = 6;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //左クリックが押された場合
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == transform.name)
                {
                    // PCが使われていないなら開く
                    if (!isPcUsed)
                    {
                        OpenTab();
                    }
                    else
                    {
                        // PCがクリックされていてもUIを触っていないなら閉じる
                        if (EventSystem.current.IsPointerOverGameObject()) return;
                        CloseTab();
                    }
                }
                else
                {
                    if (isPcUsed)
                    {
                        // UIのどこかをクリックした場合は閉じない
                        if (EventSystem.current.IsPointerOverGameObject()) return;
                        CloseTab();
                    }
                }
            }
        }
        //Escキーで閉じる
        // if (Input.GetKeyDown(KeyCode.Escape)) CloseTab();

        //Enterキーで決定
        if (Input.GetKeyDown(KeyCode.Return)) CheckPassword();
    }

    public void CheckPassword()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            if (inputField.text == correctPassword)
            {
                SoundManager.Instance.PlaySE(SESoundData.SE.Correct);

                inputField.text = "";
                StartCoroutine(MessageCoroutine(displayDuration));
            }
            else
            {
                SoundManager.Instance.PlaySE(SESoundData.SE.Mistake);
                inputField.text = "";
                // SituationTextManager.Instance.ShowMessage("不正解のようだ......");
                SituationTextManager.Instance.ShowMessage("incorrect_password");
            }
        }
    }

    IEnumerator MessageCoroutine(float time)
    {
        feedbackText.SetActive(true);
        yield return new WaitForSeconds(time); // 3秒待つ
        feedbackText.SetActive(false);
        CloseTab();

        // GameStepを進める
        if (GameManager.Instance.GetGameStep() == activateStep)
        {
            // SituationTextManager.Instance.ShowMessage("正解だった！");
            SituationTextManager.Instance.ShowMessage("correct_password");

            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                photonView.RPC("RPC_NextStep", RpcTarget.AllBuffered);
            }
            else
            {
                thesis.SetActive(true);
                GameManager.Instance.GoNextStep();
            }
        }
    }

    public void OpenTab()
    {
        selectFunakiPc.SetActive(true); // UIを表示
        Cursor.lockState = CursorLockMode.None; // マウスカーソル解放
        Cursor.visible = true;
        isPcUsed = true; // PC使用中

        // PCの状態を同期
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            photonView.RPC("RPC_PcUpdate", RpcTarget.All, isPcUsed);
    }

    public void CloseTab()
    {
        selectFunakiPc.SetActive(false); // UIを非表示
        isPcUsed = false;
        Cursor.lockState = CursorLockMode.Locked; // マウスカーソル固定
        Cursor.visible = false;

        // PCの状態を同期
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            photonView.RPC("RPC_PcUpdate", RpcTarget.All, isPcUsed);
    }

    [PunRPC]
    public void RPC_PcUpdate(bool update)
    {
        isPcUsed = update;
    }

    [PunRPC]
    public void RPC_NextStep()
    {
        thesis.SetActive(true);
        GameManager.Instance.GoNextStep();
    }
}

