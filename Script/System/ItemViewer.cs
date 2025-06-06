using UnityEngine;
using UnityEngine.UI;

public class ItemViewer : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] Text itemText; 
    [SerializeField] float maxRayDistance = 10f;

    private void Update()
    {
        if (itemText == null)
        {
            Debug.LogWarning("itemText が割り当てられていません！");
            return;
        }

        // プレイヤーの視点方向に光線を飛ばす
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            // アイテムに衝突した場合
            if (hit.collider.GetComponent<Item>() != null)
            {
                // アイテム名を表示
                itemText.text = hit.collider.gameObject.name;
                itemText.enabled = true;
            }
            else
            {
                itemText.enabled = false;
            }
        }
        else
        {
            itemText.enabled = false;
        }
    }
}