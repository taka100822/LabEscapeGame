using UnityEngine;

public class PostitManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject postit;
    [SerializeField] GameObject hint_postit;

    public void PressButton(string itemName){
        if(itemName == "Postit"){
            postit.SetActive(false);        //付箋が押されたら非表示にする
            // Debug.Log("Click Post It from PostitManager!!");
        }else if(itemName == "Hint"){
            hint_postit.SetActive(false);        //ヒントが押されたら非表示にする
            // Debug.Log("Click Hint from PostitManager!!");
        }
    }
}
