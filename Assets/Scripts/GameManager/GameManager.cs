using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent onWin, onLose;
    private SoundManager soundManager;

    public Image circleTransition;
    public Text tapToPlay;
    //private Animator textAnim;
    private bool isStarted = false;
    private UIFunction UIFunction = new UIFunction();

    private LoadScene loadScene = new LoadScene();
    
    public void LoadScene(bool isWin)
    {
        circleTransition.GetComponent<DOTween>().onCompleteActionEnable.AddListener(() =>
        {
            loadScene.LoadingScene(isWin);
        });
        circleTransition.gameObject.SetActive(true);
    }

    private void OnMouseUp()
    {
        if (tapToPlay != null)
        {
            if (GameController.isWon())
            {
                circleTransition.GetComponent<DOTween>().onCompleteActionEnable.AddListener(() =>
                {
                    loadScene.LoadingScene(true);
                });
                circleTransition.gameObject.SetActive(true);
                //loadScene.LoadingScene(true);
            }
            else if (GameController.isLose())
            {
                circleTransition.GetComponent<DOTween>().onCompleteActionEnable.AddListener(() =>
                {
                    loadScene.LoadingScene(false);
                });
                circleTransition.gameObject.SetActive(true);
            }
            else if (!isStarted)
            {
                isStarted = true;
                tapToPlay.GetComponents<DOTween>()[0].Disable();
                tapToPlay.GetComponents<DOTween>()[1].Disable();
            }
        }
    }

    private void Awake()
    {
        //tapToPlay = this.GetComponentInChildren<Text>();
        //textAnim = tapToPlay.GetComponent<Animator>();
    }

    private void Start()
    {
        CustomTime.LocalTimeScale = 0;
        GameController.InitLevel();

        soundManager = FindObjectOfType<SoundManager>();
        circleTransition.GetComponent<DOTween>().Disable();
    }

    private void Update()
    {
        if (GameController.isWon() && !CustomTime.IsPaused)
        {
            if (tapToPlay != null)
            {
                tapToPlay.text = "Tiếp Tục Nào";
                tapToPlay.gameObject.SetActive(true);
            }
            OnCompleteWin();

            soundManager.PlaySound(soundManager.SoundWin, 1);

            CustomTime.LocalTimeScale = 0;
        }
        else if(GameController.isLose() && !CustomTime.IsPaused)
        {
            if (tapToPlay != null)
            {
                tapToPlay.text = "Làm Lại Thôi";
                tapToPlay.gameObject.SetActive(true);
            }
            OnCompleteLose();
            //textAnim.SetBool("BoolStarted", false);
            CustomTime.LocalTimeScale = 0;
        }
    }

    private void OnCompleteWin()
    {
        if (onWin != null)
        {
            onWin.Invoke();
        }
    }

    private void OnCompleteLose()
    {
        if (onLose != null)
        {
            onLose.Invoke();
        }
    }

    public void StopGame()
    {
        UIFunction.Pause();
    }

    public void ContinueGame()
    {
        UIFunction.Continue();
    }

    public void Restart()
    {
        circleTransition.GetComponent<DOTween>().onCompleteActionEnable.AddListener(() =>
        {
            UIFunction.Restart();
        });
        circleTransition.gameObject.SetActive(true);
    }

    public void NextLevel()
    {
        circleTransition.GetComponent<DOTween>().onCompleteActionEnable.AddListener(() =>
        {
            loadScene.NextLevel();
        });
        circleTransition.gameObject.SetActive(true);
    }

    public void BackLevel()
    {
        circleTransition.GetComponent<DOTween>().onCompleteActionEnable.AddListener(() =>
        {
            loadScene.PreLevel();
        });
        circleTransition.gameObject.SetActive(true);
    }

    public void LoadLevel(int level)
    {
        circleTransition.GetComponent<DOTween>().onCompleteActionEnable.AddListener(() =>
        {
            loadScene.LoadingScene(level);
        });
        circleTransition.gameObject.SetActive(true);
    }
}

public class LoadScene
{
    private const string keyData = "level";
    private const string keyMaxUnlockLevel = "maxLevel";
    public const int maxLevel = 6;
    private int maxUnlockLevel = 1;
    private int level = 1;

    public int MaxUnlockLevel
    {
        get
        {
            LoadMaxUnlockLevel();
            return maxUnlockLevel;
        }
    }

    public int Level
    {
        get 
        {
            LoadData();
            return level; 
        }
    }

    private void LoadMaxUnlockLevel()
    {
        maxUnlockLevel = PlayerPrefs.GetInt(keyMaxUnlockLevel);
    }

    private void SaveMaxUnlockLevel()
    {
        PlayerPrefs.SetInt(keyMaxUnlockLevel, maxUnlockLevel);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(keyData, level);
    }

    private void LoadData()
    {
        level = PlayerPrefs.GetInt(keyData);
        
        if (level == 0)
        {
            level = 1;
        }
    }

    public void LoadingScene(bool isWon)
    {
        level = Level;
        if(isWon)
        {
            if (level < maxLevel)
            {
                level = Level + 1;
                SaveData();
            }
            if (Level > MaxUnlockLevel)
            {
                maxUnlockLevel = level;
                SaveMaxUnlockLevel();
            }
        }
        SceneManager.LoadScene(0);
    }
    public void LoadingScene()
    {
        LoadData();
        SceneManager.LoadScene(level);
    }
    public void LoadingScene(int level)
    {
        if (level <= MaxUnlockLevel)
        {
            this.level = level;
            SaveData();
        }
        else throw new IndexOutOfRangeException();
        SceneManager.LoadScene(0);
    }

    public void ResetData()
    {
        PlayerPrefs.SetInt(keyData, 1);
        PlayerPrefs.SetInt(keyMaxUnlockLevel, 1);
    }

    public void NextLevel()
    {
        level = Level;
        if (level < MaxUnlockLevel)
        {
            level = level + 1;
            SaveData();
        }
        SceneManager.LoadScene(0);
    }

    public void PreLevel()
    {
        level = Level;
        if (level > 1) level = level - 1;
        SaveData();
        LoadData();
        SceneManager.LoadScene(0);
    }
}
