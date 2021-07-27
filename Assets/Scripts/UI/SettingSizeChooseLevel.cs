using UnityEngine;
using UnityEngine.UI;

public class SettingSizeChooseLevel : MonoBehaviour
{
    private Vector2 cellSize = new Vector2(47.46875f / 1.25f, 64f / 1.25f);
    private Vector2 spacing = new Vector2(3, 3);
    private float ratio;

    // Start is called before the first frame update
    void Awake()
    {
        // ratio = Camera.main.pixelHeight * 1.0f / 362;

        // cellSize = cellSize * ratio;
        // spacing = spacing * ratio;

        // GetComponent<GridLayoutGroup>().cellSize = cellSize;
        // GetComponent<GridLayoutGroup>().spacing = spacing;
    }
}
