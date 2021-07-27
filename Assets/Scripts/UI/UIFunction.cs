using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIFunction
{
    private LoadScene loadScene = new LoadScene();

    private void StopTime()
    {
        CustomTime.LocalTimeScale = 0f;
    }

    public void Pause()
    {
        StopTime();
    }

    public void Continue()
    {
        CustomTime.LocalTimeScale = 1f;
    }

    public void Restart()
    {
        loadScene.LoadingScene();
    }
}
