using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string keyData = "level";

    private Text tapToPlay;
    private Animator textAnim;
    private bool isStarted = false;
    private string level = "0";

    private void LoadData()
    {
        level = PlayerPrefs.GetString(keyData);

        if(string.IsNullOrEmpty(level))
        {
            level = "0";
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetString(keyData, level);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(int.Parse(level));
    }

    private void OnMouseUp()
    {
        if (GameController.isWon()) 
        {
            level = (int.Parse(level) + 1).ToString();
            SaveData();
            LoadScene();
        }  
        else if (!isStarted)
        {
            isStarted = true;
            CustomTime.LocalTimeScale = 1;
            textAnim.SetBool("BoolStarted", true);
        }

        ObjectAbstract enemy = GameObject.Find("Enemy").GetComponent<ObjectAbstract>();
        ObjectAbstract character = GameObject.Find("Character").GetComponent<ObjectAbstract>();



        AIEnemy aIEnemy = new AIEnemy(enemy);
        aIEnemy.AFind(enemy, character);
        Stack<ObjectAbstract> path = aIEnemy.path;

        while(path.Count != 0)
        {
            Debug.Log(path.Pop().name);
        } 
    }

    private void Awake()
    {
        tapToPlay = this.GetComponentInChildren<Text>();
        textAnim = tapToPlay.GetComponent<Animator>();
    }

    private void Start()
    {
        CustomTime.LocalTimeScale = 0;
        GameController.InitLevel();
    }

    private void Update()
    {
        if(GameController.isWon())
        {
            tapToPlay.text = "Tap To Next";
            textAnim.SetBool("BoolStarted", false);
            CustomTime.LocalTimeScale = 0;
        }
    }

}


