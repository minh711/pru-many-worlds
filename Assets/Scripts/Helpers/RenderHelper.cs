using UnityEngine;
using UnityEngine.UI;

public class RenderHelper : MonoBehaviour
{
    /// <summary>
    /// Vẽ <b>GameObject</b> lên trung tâm của <b>Spawn Point</b>.
    /// </summary>
    /// <param name="gameObject"><b>GameObject</b> cần vẽ.</param>
    /// <param name="spawnPoint"><b>Spawn Point</b> - vị trí vẽ lên.</param>
    /// <returns>
    /// Trả về một <b>GameObject</b> mới chính là GameObject được vẽ lên trên Scene.
    /// <br/><br/>
    /// Lưu ý là GameObject này <b>khác</b> với GameObject truyền vào, vì vậy cần xử lý phức tạp hơn một chút.
    /// </returns>
    public GameObject InstantiateOnCenter(GameObject gameObject, Transform spawnPoint)
    {
        GameObject spawnedGameObject = Instantiate(gameObject, spawnPoint.position, spawnPoint.rotation);

        return spawnedGameObject;
    }

    /// <summary>
    /// Vẽ Sprite lên trên một Transform
    /// </summary>
    /// <param name="sprite">Sprite cần vẽ</param>
    /// <param name="panel">Transform sẽ vẽ lên</param>
    public void DrawSprite(Sprite sprite, Transform panel)
    {
        Image image = panel.GetComponent<Image>();

        if (sprite == null)
        {
            image.sprite = null;
            Color color = image.color;
            color.a = 0f; // alpha is set to 0% (completely transparent)
            image.color = color;
        }
        else
        {
            image.sprite = sprite;
            Color color = image.color;
            color.a = 1f; // alpha is set to 100% (completely opaque)
            image.color = color;
        }
    }
}