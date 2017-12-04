using System;
using UnityEngine;


public class ActivableSystemActions : MonoBehaviour
{

    #region Common

    public virtual void DoSomething(GameObject activableSystem)
    {
        throw new NotImplementedException("Every ActivableSystem must have a DoSomething method");  
    }

    #endregion

    #region Utils 

    protected void StartAnimation(string animationName, ActivableSystem activableSystem)
    {
        SceneAnimator sceneAnimator = GameObject.FindObjectOfType<SceneAnimator>();
        StartCoroutine(sceneAnimator.StartAnimation(animationName, activableSystem.gameObject));
    }

    protected void DestroyObject(string name, float time)
    {
        GameObject gameObject = GameObject.Find(name);

        if (gameObject)
        {
            Destroy(gameObject, time);
        }

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
