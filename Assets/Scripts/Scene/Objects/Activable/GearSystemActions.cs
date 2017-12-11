using UnityEngine;


public class GearSystemActions : ActivableSystemActions
{

    #region Common

    public void DoSomething(GearSystem gearSystem, bool notifyOthers)
    {

        switch (gearSystem.name)
        {
            case "MaquinaEngranajeA":
                HandleGearSystemA(gearSystem, notifyOthers);
                break;
        }

    }

    #endregion

    #region Handlers

    private void HandleGearSystemA(GearSystem gearSystem, bool notifyOthers)
    {

        // Dispose every used gear in case of reconnection
        for (int i = 0; i < gearSystem.components.Length; i++)
        {
            string usedGearName = gearSystem.components[i].sprite.name;
            GameObject usedGear = GameObject.Find(usedGearName);

            if (usedGear)
            {
                DestroyObject(usedGearName, .1f);
            }

        }

        // Hide every placed gear
        SpriteRenderer[] componentSlots = gearSystem.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Change the gearsystem sprite
        SpriteRenderer systemSpriteRenderer = gearSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = gearSystem.activatedSprite;

        SetAnimatorBool("startMovingMachine", true, gearSystem);

        DestroyObject("GiantBlocker", .1f);
        DestroyObject("GiantBlocker (1)", .1f);

        if (gearSystem.switchObj)
        {
            gearSystem.switchObj.ActivateSwitch();

            Planner planner = GameObject.FindObjectOfType<Planner>();
            planner.Monitor();
        }

        if (notifyOthers)
        {
            SendMessageToServer("ObstacleDestroyed/GiantBlocker", true);
            SendMessageToServer("ObstacleDestroyed/GiantBlocker (1)", true);
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }

    #endregion

}
