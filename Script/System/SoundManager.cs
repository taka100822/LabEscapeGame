using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviourPun
{
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] AudioMixerGroup mixerGroup;

    [SerializeField] List<BGMSoundData> bgmSoundDatas;
    [SerializeField] List<SESoundData> seSoundDatas;

    public float masterVolume = 1;
    public float bgmMasterVolume = 1;
    public float seMasterVolume = 1;

    public static SoundManager Instance { get; private set; }
    private BGMSoundData.BGM? currentBGM = null;

    // シングルトンの決まり文句
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(BGMSoundData.BGM bgm)
    {
        if (currentBGM == bgm) return; // 同じBGMなら再生し直さない

        BGMSoundData data = bgmSoundDatas.Find(data => data.bgm == bgm);
        if (data == null)
        {
            Debug.LogWarning($"BGM '{bgm}' が見つかりませんでした．");
            return;
        }

        bgmAudioSource.clip = data.audioClip;
        bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
        bgmAudioSource.Play();

        currentBGM = bgm;
    }

    
    public void PlaySE(SESoundData.SE se)
    {
        SESoundData data = seSoundDatas.Find(d => d.se == se);
        if (data == null) return;

        GameObject tempGO = new GameObject("TempSE_" + data.audioClip.name);
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.outputAudioMixerGroup = mixerGroup;

        tempSource.clip = data.audioClip;
        tempSource.volume = data.volume * seMasterVolume * masterVolume;
        tempSource.Play();

        Destroy(tempGO, data.audioClip.length); // SE終了後に自動破棄
    }

    void OnEnable()
    {
        // シーンがロードされたときに呼び出されるイベントに登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // イベント登録を解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // シーン切り替え時に実行される関数
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "LauncherScene":
                SoundManager.Instance.PlayBGM(BGMSoundData.BGM.LauncherScene);
                break;
            case "LabScene":
                SoundManager.Instance.PlayBGM(BGMSoundData.BGM.LabScene);
                break;
            default:
                Debug.Log("LauncherSceneでもLabSceneでもありません!");
                break;
        }
    }

    void Start()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "LauncherScene":
                SoundManager.Instance.PlayBGM(BGMSoundData.BGM.LauncherScene);
                break;
            case "LabScene":
                SoundManager.Instance.PlayBGM(BGMSoundData.BGM.LabScene);
                break;
            default:
                Debug.Log("LauncherSceneでもLabSceneでもありません!");
                break;
        }
    }
}

[System.Serializable]
public class BGMSoundData
{
    public string bgmName;  // Element 0などと記述されてもわかりにくいため(参照なし)

    public enum BGM
    {
        LauncherScene,
        LabScene,
    }

    public BGM bgm;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}


[System.Serializable]
public class SESoundData
{
    public string seName;   // Element 0などと記述されてもわかりにくいため(参照なし)

    public enum SE
    {
        // ここにどんどんSEを追加していく
        Click,
        DiplaySetting,
        Push,
        Set,
        ChangeTV,
        Correct,
        GetItem,
        Mistake,
        OpenDoor,
        Withdraw,
        FootStep,
        ShibuyaCorrect,
        ShibuyaMistake,
        ShibuyaVoice,
        NumUp,
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}