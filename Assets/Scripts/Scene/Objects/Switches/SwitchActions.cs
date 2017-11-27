using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActions : MonoBehaviour
{

    #region Attributes

    private LevelManager levelManager;
    private bool activated;
    private int groupId;

    public GameObject exp;

    public float spacing = 2f;
    public float gridX = 5f;
    public float gridY = 5f;

    #endregion

    #region Constructor

    public SwitchActions(GroupOfSwitchs group)
    {
        groupId = group.groupId;
        foreach (Switch switchi in group.GetSwitchs())
        {
            switchi.SetJobDone();
        }
    }

    #endregion

    #region Common

    public void DoSomething()
    {
        if (activated)
        {
            return;
        }

        activated = true;

        switch (groupId)
        {

            // Aquí comienzan acciones switch etapa 2

            case 0:
                HandlerGroup0();
                break;

            case 1:
                HandlerGroup1();
                break;

            case 2:
                HandlerGroup2();
                break;

            case 3:
                HandlerGroup3();
                break;
            case 4:
                HandlerGroup4();
                break;

            case 5:
                HandlerGroup5();
                break;

            case 6:
                HandlerGroup6();
                break;

            case 7:
                HandlerGroup7();
                break;

            case 8:
                HandlerGroup8();
                break;

            // Aquí comienzan acciones switch etapa 1

            case 9:     // Primeros peldaños
                HandlerGroup9();
                break;

            case 10:    // peldaño switch 2
                HandlerGroup10();
                break;

            case 11:    // peldaño 3rd Switch + Exp
                HandlerGroup11();
                break;

            case 12:    // OpenPaths
                HandlerGroup12();
                break;

            case 13:    // to the end of scene
                HandlerGroup13();
                break;

        }
    }

    #endregion

    #region Utils

    private void ShowFeedbackParticles(string name, Vector2 position, float liveTime)
    {
        GameObject feedbackParticles = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/" + name));
        feedbackParticles.GetComponent<Transform>().position = position;

        Destroy(feedbackParticles, liveTime);
    }

    private void DestroyObject(string name, float time)
    {
        GameObject gameObject = GameObject.Find(name);

        if (gameObject)
        {
            Destroy(gameObject, time);
        }

    }

    private void TooglePlayerFilter(string filterName, bool active)
    {

        GameObject mageFilter = GameObject.Find(filterName);

        if (mageFilter)
        {
            PlayerFilter playerFilter = mageFilter.GetComponent<PlayerFilter>();

            if (playerFilter)
            {
                playerFilter.SetActive(true);
            }
        }
    }

    private void SetMovingObjectData(GameObject movingObject, Vector2 startPos, Vector2 endPos, float moveSpeed)
    {

        MovingObject movingController = movingObject.GetComponent<MovingObject>();

        if (movingController)
        {
            movingController.SetData(startPos, endPos, moveSpeed);
        }
    }

    private GameObject InstatiatePrefab(string name, Vector2 initialPos)
    {
        GameObject prefab = (GameObject)Instantiate(Resources.Load("Prefabs/" + name));

        if (prefab)
        {
            prefab.GetComponent<Transform>().position = initialPos;
        }

        return prefab;
    }

    #endregion

    #region Handlers

    private void HandlerGroup0()
    {
        GameObject platEngineer = InstatiatePrefab("MovPlatform", new Vector2(13.3f, -1f));
        SetMovingObjectData(platEngineer, new Vector2(13.5f, -1.77f), new Vector2(13.5f, 0.36f), 1f);
        ShowFeedbackParticles("FBMageButt", new Vector2(13.2f, -1.3f), 3f);
    }

    // REVIEW THIS SHIT
    private void HandlerGroup1()
    {
        GameObject rejaEng = GameObject.FindGameObjectWithTag("RejaRocaEng");
        rejaEng.SetActive(false);

        Roca roca = GameObject.FindGameObjectWithTag("rocaCaida").GetComponent<Roca>();
        roca.isReady = true;

        CajaSwitch caja = GameObject.FindGameObjectWithTag("CajaSwitchFierro").GetComponent<CajaSwitch>();
        caja.meVoy = true;
        caja.ahoraMeVoy = true;

        DestroyObject("SpikesDead", 1f);
        DestroyObject("LavaPool", 1f);
    }

    // REVIEW THIS SHIT
    private void HandlerGroup2()
    {
        Roca rocaGigante = GameObject.FindGameObjectWithTag("rocaCaida").GetComponent<Roca>();
        rocaGigante.caidaOn = true;

        CameraController mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        mainCamera.ChangeState(CameraState.TargetZoom, 5, 34.9f, -3.06f, false);

        GameObject arbolGigante = GameObject.FindGameObjectWithTag("ArbolCaida");
        caidArbol arbool = arbolGigante.GetComponent<caidArbol>();
        arbool.colliderOn = true; //arbol

        Animator arbolQl = arbolGigante.GetComponent<Animator>();
        arbolQl.SetBool("RockBottom", true); //activa camino arbol

        PathSub abreteSesamo = GameObject.FindGameObjectWithTag("openPathSub").GetComponent<PathSub>(); // destruyePasadizo
        abreteSesamo.killMe = true;

    }

    private void HandlerGroup3()
    {
        GameObject platLadder = InstatiatePrefab("MovPlatform", new Vector2(43f, -16.46f));

        Vector2 startPos = platLadder.transform.position;
        Vector2 endPos = new Vector2(startPos.x, startPos.y + 4.2f);

        SetMovingObjectData(platLadder, startPos, endPos, 1f);

        ShowFeedbackParticles("FBMageButt", new Vector2(41.4f, -16.3f), 3f);
        TooglePlayerFilter("FilterMage", true);
    }

    // TODO: Review this
    private void HandlerGroup4()
    {

        GameObject platparaMage = InstatiatePrefab("MovPlatform", new Vector2(61f, -9.5f));

        Vector2 startPos = platparaMage.transform.position;
        Vector2 endPos = new Vector2(startPos.x, startPos.y + 1.3f);

        SetMovingObjectData(platparaMage, startPos, endPos, 1f);

        GameObject comebackMessage = InstatiatePrefab("ActivateNPC", new Vector2(70f, -19.2f)); // Donde está la info de este NPC ???

        /* Instantiate Arrow feedback y cambiar arrow de warrior*/
        GameObject arrowIndicadora = (GameObject)Instantiate(Resources.Load("Sprites/Arrows/warriorArrowLeft"));
        arrowIndicadora.GetComponent<Transform>().position = new Vector2(70.7f, -20f);

        ChangeSprite spriteChanger = GameObject.Find("CartelCambiante").GetComponent<ChangeSprite>();
        spriteChanger.SpriteChanger();

        InstatiatePrefab("Ambientales/InstantateCheckPoints", new Vector2(60.94f, -19.9f));

        ShowFeedbackParticles("FBMageButt", new Vector2(72.86f, -19.3f), 4f);
        ShowFeedbackParticles("warriorFeedbackSmall", new Vector2(70.7f, -20f), 4f);

    }

    private void HandlerGroup5()
    {
        InstatiatePrefab("PlataformaPastVoladora", new Vector2(39f, 7.5f));
        InstatiatePrefab("PlataformaPastVoladora", new Vector2(39f, 7.5f));
    }

    private void HandlerGroup6()
    {
        InstatiatePrefab("Ambientales/Exp", new Vector2(14.1f, -6.3f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(14.3f, -6.3f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(13.6f, -6.3f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(13.1f, -6.3f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(15f, -6.3f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(15f, -6.3f));
    }

    private void HandlerGroup7()
    {
        InstatiatePrefab("Ambientales/Exp", new Vector2(62f, -14.23f));

        InstatiatePrefab("Ambientales/Exp", new Vector2(62.5f, -14.23f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(64f, -14.23f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(61.5f, -14.23f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(63f, -14.23f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(63.5f, -14.23f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(62f, -14.23f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(62.5f, -14.23f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(64f, -14.23f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(61.5f, -14.23f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(63f, -14.23f));
        InstatiatePrefab("Ambientales/Exp", new Vector2(63.5f, -14.23f));
    }

    private void HandlerGroup8()
    {
        InstatiatePrefab("Ambientales/Exp", new Vector2(80.45f, -18.52f));
    }

    private void HandlerGroup9()
    {
        ShowFeedbackParticles("FBMageButt", new Vector2(26.5f, -43.6f), 4f);

        InstatiatePrefab("SueloMetalFlotante", new Vector2(24.78f, -42.31f));
        InstatiatePrefab("SueloMetalFlotante", new Vector2(24.78f, -43.16f));
    }

    private void HandlerGroup10()
    {
        ShowFeedbackParticles("FBMageButt", new Vector2(26.5f, -42.11f), 4f);
        InstatiatePrefab("SueloMetalFlotante", new Vector2(24.86f, -41.2f));
    }

    private void HandlerGroup11()
    {
        InstatiatePrefab("SueloMetalFlotante", new Vector2(26.11f, -40.40f));
    }

    private void HandlerGroup12()
    {
        ShowFeedbackParticles("FBMageButt", new Vector2(32.11f, -39.31f), 4f);
        InstatiatePrefab("TutorialPaths", new Vector2(35.6f, -38.95f));
        DestroyObject("PathBlocker", .1f);
    }

    private void HandlerGroup13()
    {
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.localPlayer.respawnPosition = new Vector3(136.15f, -26.33f, 1f);
        levelManager.Respawn();
    }

    #endregion

}