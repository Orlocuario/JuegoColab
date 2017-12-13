using UnityEngine;
using System.Collections;

public class GearSystemActions : ActivableSystemActions
{
    public float blockerSpeed;

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

        // If is Engineer: Start Coroutine
        if (notifyOthers)
        {
            LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
            EngineerController enginController = levelManager.GetEngineer();

            if (enginController && enginController.localPlayer)
            {
                CameraMovementForEngin();
            }

        }

        gearSystem.ToogleParticles(true);
        SetAnimatorBool("startMoving", true, gearSystem);

        GameObject secondMachine = GameObject.Find("MaqEngranaje2");
        if (secondMachine)
        {
            ActivableSystem secondGear = secondMachine.GetComponent<ActivableSystem>();
            SetAnimatorBool("startMoving", true, secondGear);
        }
        
        MoveTowardsAndDie blocksMover = GameObject.Find("GiantBlockers").GetComponent<MoveTowardsAndDie>();
        blocksMover.StartMoving(gearSystem.GetParticles());

        if (notifyOthers)
        {
            SetAnimatorBool("startMovingMachine", false, gearSystem, 2f);
        }

        if (gearSystem.switchObj)
        {
            gearSystem.switchObj.ActivateSwitch();

            Planner planner = GameObject.FindObjectOfType<Planner>();
            planner.Monitor();
        }

        if (notifyOthers)
        {
            SendMessageToServer("ObstacleDestroyed/GiantBlockers", true);
            SendMessageToServer("ActivateSystem/" + gearSystem.name, true);
        }
    }

    #endregion
    private void CameraMovementForEngin()
    {
        CameraController mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

        if (mainCamera == null)
        {
            Debug.Log("Se cayó la Cámara en el GearSystem");
        }

        mainCamera.ChangeState(CameraState.TargetZoom, 4.2f, 80.1f, -1.33f, false, true);
    }

}
