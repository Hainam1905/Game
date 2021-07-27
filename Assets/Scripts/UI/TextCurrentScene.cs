using UnityEngine;
using UnityEngine.UI;

public class TextCurrentScene : MonoBehaviour
{
    LoadScene loadScene = new LoadScene();

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Text>().text = "Màn " + (loadScene.Level - 3);
    }
}
