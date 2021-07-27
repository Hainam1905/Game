using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadData : MonoBehaviour
{
    private LoadScene LoadScene = new LoadScene();
    public Slider slider;

    //Start is called before the first frame update
    void Start()
    {
        // LoadScene.ResetData()
        //SceneManager.LoadScene(LoadScene.Level);
        StartCoroutine(LoadAsynchronously(LoadScene.Level));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progess = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progess;
            yield return null;
        }
    }
}
