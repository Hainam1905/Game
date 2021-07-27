using UnityEngine;
using UnityEngine.Events;
using System;

public enum ActionOnCompleteDisble
{
    Nothing,
    Restart,
    Continue,
}

public enum UIAnimationTypes
{
    Move,
    Scale,
    ScaleX,
    ScaleY,
    Fade,
}

public class DOTween : MonoBehaviour
{
    private UIFunction UIFunction = new UIFunction();
    public GameObject objectToAnimate;

    public UIAnimationTypes animationType;
    public LeanTweenType easeType;
    public float duration;
    public float delay;

    public bool loop;
    public bool pingpong;

    public bool startPositionOffset;
    public Vector3 from, to;

    private LTDescr tweenObject;

    public bool showOnEnable = true;
    //public bool workOnDisable;

    public UnityEvent onCompleteActionEnable = null, onCompleteActionDisable = null;
    private ActionOnCompleteDisble actionOnCompleteDisble;

    private void OnEnable()
    {
        if (showOnEnable)
        {
            Show(true);
        }
    }

    public void Show(bool isEnable)
    {
        HandleTween(isEnable);
    }

    private void HandleTween(bool isEnable)
    {
        if (objectToAnimate == null)
        {
            objectToAnimate = gameObject;
        }

        switch (animationType)
        {
            case UIAnimationTypes.Fade:
                Fade();
                break;
            case UIAnimationTypes.Move:
                MoveAbsolute();
                break;
            case UIAnimationTypes.Scale:
                Scale();
                break;
            case UIAnimationTypes.ScaleY:
                Scale();
                break;
            case UIAnimationTypes.ScaleX:
                ScaleX();
                break;
        }

        tweenObject.setDelay(delay);
        tweenObject.setEase(easeType);
        if (isEnable)
        {
            tweenObject.setOnComplete(OnCompleteEnable);
        }
        else
        {
            tweenObject.setOnComplete(OnCompleteDisable);
        }

        if (loop)
        {
            tweenObject.loopCount = int.MaxValue;
        }
        if (pingpong)
        {
            tweenObject.setLoopPingPong();
        }
    }

    private void Fade()
    {
        if (gameObject.GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }

        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<CanvasGroup>().alpha = from.x;
        }
        tweenObject = LeanTween.alphaCanvas(objectToAnimate.GetComponent<CanvasGroup>(), to.x, duration);
    }

    private void MoveAbsolute()
    {
        objectToAnimate.GetComponent<RectTransform>().anchoredPosition = from;

        tweenObject = LeanTween.move(objectToAnimate.GetComponent<RectTransform>(), to, duration);
    }

    private void Scale()
    {
        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<RectTransform>().localScale = from;
        }
        tweenObject = LeanTween.scale(objectToAnimate, to, duration);
    }

    private void ScaleX()
    {
        if (startPositionOffset)
        {
            from.y = objectToAnimate.GetComponent<RectTransform>().localScale.y;
            from.z = objectToAnimate.GetComponent<RectTransform>().localScale.z;
            objectToAnimate.GetComponent<RectTransform>().localScale = from;
        }
        tweenObject = LeanTween.scaleX(objectToAnimate, to.x, duration);
    }

    void SwapDirection()
    {
        var temp = from;
        from = to;
        to = temp;
    }

    public void Disable()
    {
        SwapDirection();
        
        HandleTween(false);

        //tweenObject.setOnComplete(() =>
        //{
        //    SwapDirection();

        //    gameObject.SetActive(false);
        //});
    }

    public void OnCompleteEnable()
    {
        if (onCompleteActionEnable != null)
        {
            onCompleteActionEnable.Invoke();
        }
    }

    public void OnCompleteDisable()
    {
        if (onCompleteActionDisable != null)
        {
            onCompleteActionDisable.Invoke();
        }
        SwapDirection();

        gameObject.SetActive(false);
    }

    public void SetRestartOnCompleteDisable()
    {
        actionOnCompleteDisble = ActionOnCompleteDisble.Restart;
    }

    public void SetContinueOnCompleteDisable()
    {
        actionOnCompleteDisble = ActionOnCompleteDisble.Continue;
    }

    private void Restart()
    {
        UIFunction.Restart();
    }

    private void Continue()
    {
        UIFunction.Continue();
    }

    private void OnDisable()
    {
        switch (actionOnCompleteDisble)
        {
            case ActionOnCompleteDisble.Continue:
                Continue();
                break;
            case ActionOnCompleteDisble.Restart:
                Restart();
                break;
        }
    }
}
