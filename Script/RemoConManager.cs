using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

/// <summary>
/// リモコンによってテレビ画面を切り替えるマネージャー
/// Photonを使って画面変更を同期する
/// </summary>
public class RemoConManager : MonoBehaviourPun
{
    [Header("テレビ画面")]
    [SerializeField] Renderer tvScreenRenderer;                   // テレビの画面を表すRenderer
    [SerializeField] List<Material> screenMaterials_ls;           // 通常の画面素材リスト
    [SerializeField] Material smaBroScreen;                       // スマブラ風の画面素材
    [SerializeField] Material blackScreen;                        // 黒画面（画面オフ状態）

    [Header("リモコン判定")]
    [SerializeField] float maxDistance = 2.0f;                    // リモコンが届く最大距離
    [SerializeField] LayerMask remoteLayer;                       // リモコンが所属するレイヤー

    private int currentIndex = 0;                                 // 現在表示中の画面インデックス
    private bool hasAddedSmabroScreen = false;                    // スマブラ画面をリストに追加したかのフラグ
    private int activateStep = 5;

    void Start()
    {
        AddBlackScreenToList();                                   // 最初に黒画面をリストへ追加
        SetScreenMaterial(blackScreen);                           // 黒画面を初期表示に設定
        
        currentIndex = screenMaterials_ls.IndexOf(blackScreen); // <- インデックスも合わせる
    }

    void Update()
    {
        // マウス左クリックでリモコンを押したかをチェック
        if (Input.GetMouseButtonDown(0))
        {
            if (ClickedRemote(out RaycastHit hit))
            {
                SoundManager.Instance.PlaySE(SESoundData.SE.ChangeTV);
                HandleRemoteClick();                              // リモコンクリック時の処理
            }
        }
    }

    /// <summary>
    /// リモコンをクリックしたか判定するRaycast処理
    /// </summary>
    private bool ClickedRemote(out RaycastHit hit)
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        return Physics.Raycast(ray, out hit, maxDistance, remoteLayer)
        && hit.transform.name == "TVRemoteController";
    }

    /// <summary>
    /// リモコンが押された時の処理
    /// </summary>
    private void HandleRemoteClick()
    {
        int nextIndex = (currentIndex + 1) % screenMaterials_ls.Count;

        // 現在インデックスをまず進める
        currentIndex = nextIndex;

        // 画面を更新
        SetScreenMaterial(screenMaterials_ls[currentIndex]);

        // 同期
        BroadcastScreenUpdate(currentIndex);

        // 特定条件下でスマブラ画面を追加
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            photonView.RPC("UpdateScreenListIfNeeded", RpcTarget.AllBuffered);
        }
        else
        {
            UpdateScreenListIfNeeded();
        }

        // UpdateScreenListIfNeeded();

        // GameStep関連の同期処理
        if (hasAddedSmabroScreen && currentIndex == screenMaterials_ls.Count - 2)
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                photonView.RPC("RPC_WatchSmabroScreen", RpcTarget.AllBuffered);
            }
            else
            {
                if (GameManager.Instance.GetGameStep() == activateStep)
                {
                    Debug.Log("GameStepは" + GameManager.Instance.GetGameStep() + "に進みました");
                    GameManager.Instance.GoNextStep();
                }
            }
        }
    }

    /// <summary>
    /// 画面に指定されたMaterialを設定する
    /// </summary>
    private void SetScreenMaterial(Material mat)
    {
        if (tvScreenRenderer != null && mat != null)
        {
            tvScreenRenderer.material = mat;
        }
    }

    /// <summary>
    /// RPCで他クライアントにも画面インデックスを送信
    /// </summary>
    private void BroadcastScreenUpdate(int index)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            photonView.RPC("SyncScreen", RpcTarget.OthersBuffered, index);
        }
    }

    /// <summary>
    /// スマブラ画面の追加条件を満たした場合，素材リストを更新する
    /// </summary>
    [PunRPC]
    private void UpdateScreenListIfNeeded()
    {
        if (!hasAddedSmabroScreen && GameManager.Instance.GetGameStep() >= 5)
        {
            if (screenMaterials_ls.Count > 0)
            {
                screenMaterials_ls.RemoveAt(screenMaterials_ls.Count - 1); // 最後の黒画面を削除
            }

            screenMaterials_ls.Add(smaBroScreen);
            screenMaterials_ls.Add(blackScreen);
            hasAddedSmabroScreen = true;
        }
    }

    /// <summary>
    /// 黒画面を素材リストに追加（重複チェック付き）
    /// </summary>
    private void AddBlackScreenToList()
    {
        if (!screenMaterials_ls.Contains(blackScreen))
        {
            screenMaterials_ls.Add(blackScreen);
        }
    }

    /// <summary>
    /// 他クライアントからの同期用RPC
    /// </summary>
    [PunRPC]
    public void SyncScreen(int index)
    {
        if (index >= 0 && index < screenMaterials_ls.Count)
        {
            currentIndex = index;
            SetScreenMaterial(screenMaterials_ls[index]);
        }
    }
    
    [PunRPC]
    public void RPC_WatchSmabroScreen()
    {
        if (GameManager.Instance.GetGameStep() == activateStep)
        {
            Debug.Log("GameStepは" + GameManager.Instance.GetGameStep() + "に進みました");
            GameManager.Instance.GoNextStep();
        }
    }
}
