using UnityEngine;
using UnityEngine.UI;

public class StaplerBoxPassButton : MonoBehaviour
{
    [SerializeField] Text numberText = default;
    public int number;

    private void Start()
    {
        number = 0;
        numberText.text = number.ToString(); //テキストの数値を変える
    }


    // 実行されたら数値を変える
    public void OnClickThis()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.NumUp);
        
        number++; // 数値を＋１する
        if(number > 9) // もし９を超えたら
        {
            number = 0; // 0に戻す
        }
        numberText.text = number.ToString(); //テキストの数値を変える
    }
}
