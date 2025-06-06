using UnityEngine;

public class DrawerTrigger : MonoBehaviour
{
    public Animator drawerAnimator;
    private bool isCursorLocked = true;     //カーソルがロックされているかどうかを判定するためのフラグ変数
    private bool isOpen = false;

    void Start()
    {
        LockCursor(true); // 初期状態ではロック
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)){ // 右クリックでロック切り替え
            isCursorLocked = !isCursorLocked;   //isCursorLocked の値を反転させる（true → false、false → true）
            LockCursor(isCursorLocked);         //カーソルのロック状態を更新
        }

        if (!isCursorLocked && Input.GetMouseButtonDown(0)){ //カーソルがロック解除されている かつ 左クリックが押された場合
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //画面上のマウスの位置からRay（光線）を発射
            RaycastHit hit;                                                 //光線がどこに当たったかを調べるための情報をRaycastHit hitに格納

            if (Physics.Raycast(ray, out hit)){
                if (hit.transform.name == "Drawer")
                {   //光線があたったオブジェクト名が"Drawer"ならば
                    // Debug.Log("Hit object: " + hit.transform.name);
                    // 状態をトグルしてアニメーション切り替え
                    isOpen = !isOpen;
                    drawerAnimator.SetBool("isOpen", isOpen);
                    if (isOpen)
                    {
                        // SituationTextManager.Instance.ShowMessage("引き出しを閉めた");
                        SituationTextManager.Instance.ShowMessage("drawer_closed");
                    }
                    else
                    {
                        // SituationTextManager.Instance.ShowMessage("引き出しを開けた");
                        SituationTextManager.Instance.ShowMessage("drawer_opened");
                    }
                }
            }
        }
    }

    void LockCursor(bool lockState){
        if (lockState){                                         //カーソルがロックされていたら
            Cursor.lockState = CursorLockMode.Locked;           //Cursor.lockState を CursorLockMode.Locked に設定して画面中央に固定
            Cursor.visible = false;                             //Cursor.visible を false にして非表示
        }else{                                                  //カーソルがロックされていなかったら
            Cursor.lockState = CursorLockMode.None;             //Cursor.lockState を CursorLockMode.None に設定して 自由に動かせる状態にする
            Cursor.visible = true;                              //Cursor.visible を true にして表示
        }
    }
}

