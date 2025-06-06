using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // ← これを忘れずに！


public class SelectHintBoxPaper : MonoBehaviour
{
    [SerializeField] GameObject selectHintBoxPaper;
    [SerializeField] Text situationMessageText;
    private bool isDisplay = false;
    private bool isSeeHintBoxPaper = false;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { //左クリックが押された場合
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //画面上のマウスの位置からRay（光線）を発射
            RaycastHit hit;                                                 //光線がどこに当たったかを調べるための情報をRaycastHit hitに格納
            if (Physics.Raycast(ray, out hit))
            {
                if (!isDisplay)
                {
                    if (hit.transform.name == this.transform.name)
                    {
                        if (GameManager.Instance.GetGameStep() > 1)
                        {
                            selectHintBoxPaper.SetActive(true);
                            isDisplay = true;
                            if (!isSeeHintBoxPaper)
                            {
                                isSeeHintBoxPaper = true;
                            }
                        }
                        else
                        {
                            // SituationTextManager.Instance.ShowMessage("暗くて見えない");
                            SituationTextManager.Instance.ShowMessage("dark");
                        }
                    }
                }
                else
                {
                    // UI外をクリックしたら閉じる
                    if (isDisplay && !EventSystem.current.IsPointerOverGameObject())
                    {
                        selectHintBoxPaper.SetActive(false);
                        isDisplay = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            }
            else if (isDisplay)
            {
                // どこもクリックされてないけどUIは表示中 → 閉じる
                selectHintBoxPaper.SetActive(false);
                isDisplay = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (isDisplay)
        {
            // カーソルを表示してロック解除
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

