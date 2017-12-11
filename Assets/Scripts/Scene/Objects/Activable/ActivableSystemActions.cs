using UnityEngine;

public class ActivableSystemActions
{
    #region Common

    public virtual void DoSomething(GameObject activableSystem)
    {
        if (activableSystem.GetComponent<GearSystem>())
        {
            new GearSystemActions().DoSomething(activableSystem);
        }

        else if (activableSystem.GetComponentInChildren<RuneSystem>())
        {
            new RuneSystemActions().DoSomething(activableSystem);
        }
    }

    #endregion

    #region Utils 

    protected void StartAnimation(string animationName, ActivableSystem activableSystem)
    {
        SceneAnimator sceneAnimator = GameObject.FindObjectOfType<SceneAnimator>();
        sceneAnimator.StartAnimation(animationName, activableSystem.gameObject);
    }

    protected void SetAnimatorBool(string parameter, bool value, ActivableSystem activableSystem)
    {
        SceneAnimator sceneAnimator = GameObject.FindObjectOfType<SceneAnimator>();
        sceneAnimator.SetBool(parameter, value, activableSystem.gameObject);
    }

    protected void DestroyObject(string name, float time)
    {
        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.DestroyObject(name, time);
    }

    #endregion

    #region Messaging

    protected void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    #endregion

}
