using System;
using UnityEngine;


public class RuneSystemActions : MonoBehaviour

{

    #region Common

    public void DoSomething(GameObject runeSystemGO)
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

    public void DoSomething(RuneSystem runeSystem)
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
