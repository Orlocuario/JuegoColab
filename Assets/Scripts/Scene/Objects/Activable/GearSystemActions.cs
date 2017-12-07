using System;
using UnityEngine;


public class GearSystemActions : ActivableSystemActions
{

    #region Common

    public override void DoSomething(GameObject gearSystemGO)
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

    protected  void DoSomething(GearSystem gearSystem)
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

        SendMessageToServer("ObstacleDestroyed/GiantBlocker", true);
        SendMessageToServer("ObstacleDestroyed/GiantBlocker (1)", true);
        SendMessageToServer("ActivateGearSystem/" + this.gameObject.name, true);
    }

    #endregion

}
