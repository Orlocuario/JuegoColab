using System;
using UnityEngine;


public class RuneSystemActions : ActivableSystemActions

{

    #region Common

    public override void DoSomething(GameObject runeSystemGO)
    {
        RuneSystem runeSystem = runeSystemGO.GetComponent<RuneSystem>();

        if (runeSystem)
        {
            DoSomething(runeSystem);
        }
        else
        {
            Debug.LogError(runeSystemGO + " does not have a RuneSystem");
        }
    }

    protected void DoSomething(RuneSystem runeSystem)
    {

        SpriteRenderer systemSpriteRenderer = runeSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = runeSystem.activatedSprite;

        Collider2D collider = runeSystem.GetComponent<Collider2D>();
        collider.enabled = false;

        if (runeSystem.obstacleObj != null)
        {
            runeSystem.obstacleObj.OpenDoor();
        }

        Planner planner = FindObjectOfType<Planner>();
        planner.Monitor();

        SendMessageToServer("ActivateRuneSystem/" + runeSystem.name, true);

    }

    #endregion

}
