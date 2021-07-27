using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateLevels : MonoBehaviour
{
    public GameObject levelUI, lockLevel, gameManager;
    private LoadScene loadScene = new LoadScene();
    private SoundManager soundManager;
    private Font font;
    private string pathFont = "Fonts/ARIAL";

    private void Start()
    {
        font = Resources.Load<Font>(pathFont);
        soundManager = FindObjectOfType<SoundManager>();

        if  (!font) Debug.Log("Have no font");
    }

    // Start is called before the first frame update
    public void GenerateLevel()
    {
        if (!font)  return;
        if(this.GetComponentInChildren<Text>() == null)
        {
            for (int i = 1 ; i <= 3; i++) 
            {
                levelUI.GetComponent<DOTween>().delay = i * 1.0f / 10;
                GameObject gameObject = Instantiate(levelUI, this.gameObject.transform);

                if (i == 1) 
                {
                    gameObject.GetComponentInChildren<Text>().font = font;
                    gameObject.GetComponentInChildren<Text>().text = "Luyện Tập 1";

                    gameObject.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        gameManager.GetComponent<GameManager>().LoadLevel(1);              
                    
                        if (soundManager)   soundManager.PlayClickSound();
                    });
                } 
                if (i == 2) 
                {   
                    gameObject.GetComponentInChildren<Text>().font = font;
                    gameObject.GetComponentInChildren<Text>().text = "Luyện Tập 2";

                    gameObject.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        gameManager.GetComponent<GameManager>().LoadLevel(2);

                        if (soundManager)   soundManager.PlayClickSound();
                    });
                }
                if (i == 3) 
                {
                    gameObject.GetComponentInChildren<Text>().font = font;
                    gameObject.GetComponentInChildren<Text>().text = "Luyện Tập 3";
                    
                    gameObject.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        gameManager.GetComponent<GameManager>().LoadLevel(3);                
                    
                        if (soundManager)   soundManager.PlayClickSound();
                    });
                }
            }

            for (int i = 4; i <= loadScene.MaxUnlockLevel; i++)
            {
                levelUI.GetComponent<DOTween>().delay = i * 1.0f / 10;
                GameObject gameObject = Instantiate(levelUI, this.gameObject.transform);
                
                gameObject.GetComponentInChildren<Text>().font = font;
                gameObject.GetComponentInChildren<Text>().text = "Màn " + (i - 3).ToString();

                gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    string[] temp = gameObject.GetComponentInChildren<Text>().text.ToString().Split(new[] {' '});
                    int level = int.Parse(temp[temp.Length - 1]) + 3;

                    gameManager.GetComponent<GameManager>().LoadLevel(level);
                    if (soundManager)   soundManager.PlayClickSound();
                });
            }
            
            for (int i = loadScene.MaxUnlockLevel + 1; i <= LoadScene.maxLevel; i++)
            {
                lockLevel.GetComponent<DOTween>().delay = i * 1.0f / 10;
                GameObject gameObject = Instantiate(lockLevel, this.gameObject.transform);

                gameObject.GetComponentInChildren<Text>().font = font;
                gameObject.GetComponentInChildren<Text>().text = "Màn " + (i - 3).ToString();
            }

            {
                GameObject gameObject = Instantiate(lockLevel, this.gameObject.transform);

                gameObject.GetComponentInChildren<Text>().font = font;
                gameObject.GetComponentInChildren<Text>().text = "Đang phát triển";
            }

        }
    }
}
