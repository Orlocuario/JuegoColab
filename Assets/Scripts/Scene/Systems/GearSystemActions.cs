using System;
using UnityEngine;


public class GearSystemActions : MonoBehaviour
{

    #region Common

    public void DoSomething(GameObject gearSystemGO)
    {
        GearSystem gearSystem = gearSystemGO.GetComponent<GearSystem>();

        if (gearSystem)
        {
            DoSomething(gearSystem);
        }
        else
        {
            Debug.LogError(gearSystemGO + " does not have a GearSystem");

        }
    }

    public void DoSomething(GearSystem gearSystem)
    {

        switch (gearSystem.name)
        {
            case "MaquinaEngranajeA":
                HandleGearSystemA(gearSystem);
                break;
        }

    }

    #endregion

    #region Handlers

    private void HandleGearSystemA(GearSystem gearSystem)
    {

        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        StartAnimation("startMovingMAchine", gearSystem);

        DestroyObject("GiantBlocker", .1f);
        DestroyObject("GiantBlocker (1)", .1f);

        if (gearSystem.switchObj)
        {
            gearSystem.switchObj.ActivateSwitch();
            Planner planner = FindObjectOfType<Planner>();
            planner.Monitor();
        }

        SendMessageToServer("ActivateGearSystem/" + this.gameObject.name, true);

    }

    #endregion

    #region Utils 

    protected void StartAnimation(string animationName, GearSystem gearSystem)
    {
        SceneAnimator sceneAnimator = GameObject.FindObjectOfType<SceneAnimator>();
        StartCoroutine(sceneAnimator.StartAnimation(animationName, gearSystem.gameObject));
    }

    private void DestroyObject(string name, float time)
    {
        GameObject gameObject = GameObject.Find(name);

        if (gameObject)
        {
            Destroy(gameObject, time);
        }

    }

    #endregion

    #region Messaging

    private void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    #endregion

}
