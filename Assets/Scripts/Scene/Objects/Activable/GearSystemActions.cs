using UnityEngine;
using System.Collections;

public class GearSystemActions : ActivableSystemActions
{
    public float blockerSpeed;
    private GameObject particles;

    #region Common

    private void Start()
    {
        particles = GameObject.Find("MaquinaParticleSystem");
        if (particles != null)
        {
            particles.SetActive(false);
        }
    }

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

        LevelManager levelManager = GameObject.FindObjectOfType<LevelManager>();
        EngineerController enginController = levelManager.GetEngineer();

        if (!enginController)
        {
            Debug.Log("Se cayó un enginController");
            return; 
        }
        if (enginController.localPlayer)
        {
            CameraMovementForEngin();
        }

        particles.SetActive(true);
        SetAnimatorBool("startMovingMachine", true, gearSystem);
        StartMovingBlockers();
        //  Mago y Engin waiting for Engin.
        StartCoroutine(WaitForCameraEngin());

        //  Eventos tras esperar CameraEngin
      
        SetAnimatorBool("startMovingMachine", false, gearSystem);

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

    private IEnumerator WaitForCameraEngin()
    {
        yield return new WaitForSeconds(2f);
    }

    private void StartMovingBlockers()
    {
        Transform blockers = GameObject.Find("GiantBlockers").GetComponent<Transform>();
        Vector3 blockersTarget = new Vector3(blockers.position.x, blockers.position.y + 4f, blockers.position.z);

        blockers.position = Vector3.MoveTowards(blockers.position, blockersTarget, blockerSpeed);

        if(blockers.position == blockersTarget)
        {
            DestroyObject("GiantBlockers", .1f);
            Destroy(particles, .1f);    
        }
    
    }

}
