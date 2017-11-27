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

    #region Handlers

    private void HandlerGroup0()
    {
        GameObject firstPlatform = (GameObject)Instantiate(Resources.Load("Prefabs/MovPlatform"));
        firstPlatform.GetComponent<Transform>().position = new Vector2(13.3f, -1f);
        MovingObject firstPlatformScript = firstPlatform.GetComponent<MovingObject>();
        firstPlatformScript.startPoint = new Vector2(13.5f, -1.77f);
        firstPlatformScript.endPoint = new Vector2(13.5f, 0.36f);
        firstPlatformScript.moveSpeed = 1f;
        GameObject feedbackswitchEng = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
        feedbackswitchEng.GetComponent<Transform>().position = new Vector2(13.2f, -1.3f);
        Destroy(feedbackswitchEng, 3f);
    }

    private void HandlerGroup1()
    {
        GameObject rejaEng = GameObject.FindGameObjectWithTag("RejaRocaEng");
        rejaEng.SetActive(false);
        Roca roca = GameObject.FindGameObjectWithTag("rocaCaida").GetComponent<Roca>();
        roca.isReady = true;
        GameObject particleRoca = (GameObject)Instantiate(Resources.Load("Prefabs/ParticulasMageRoca"));
        CajaSwitch caja = GameObject.FindGameObjectWithTag("CajaSwitchFierro").GetComponent<CajaSwitch>();
        caja.meVoy = true;
        caja.ahoraMeVoy = true;
        GameObject spikes = GameObject.Find("SpikesDead");
        Destroy(spikes);
        GameObject lavaPool = GameObject.Find("LavaPool");
        Destroy(lavaPool, 1f);
    }

    private void HandlerGroup2()
    {
        Roca rocaGigante = GameObject.FindGameObjectWithTag("rocaCaida").GetComponent<Roca>();
        rocaGigante.caidaOn = true;
        CameraController mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        mainCamera.ChangeState(CameraState.TargetZoom, 5, 34.9f, -3.06f, false);
        GameObject arbolGigante = GameObject.FindGameObjectWithTag("ArbolCaida");
        caidArbol arbool = arbolGigante.GetComponent<caidArbol>();
        arbool.colliderOn = true;//arbol
        Animator arbolQl = arbolGigante.GetComponent<Animator>();
        arbolQl.SetBool("RockBottom", true); //activa camino arbol
        PathSub abreteSesamo = GameObject.FindGameObjectWithTag("openPathSub").GetComponent<PathSub>();//destruyePasadizo
        abreteSesamo.killMe = true;
    }
    private void HandlerGroup3()
    {
        GameObject platescalera = (GameObject)Instantiate(Resources.Load("Prefabs/MovPlatform"));
        platescalera.GetComponent<Transform>().position = new Vector2(43f, -16.46f);
        MovingObject secondPlatform = platescalera.GetComponent<MovingObject>();
        secondPlatform.startPoint = new Vector2(platescalera.transform.position.x, platescalera.transform.position.y);
        secondPlatform.endPoint = new Vector2(platescalera.transform.position.x, platescalera.transform.position.y + 4.2f);
        secondPlatform.moveSpeed = 1f;
        GameObject feedbackswitch = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
        feedbackswitch.GetComponent<Transform>().position = new Vector2(41.4f, -16.3f);

        GameObject mageFilter = GameObject.Find("FilterMage");

        if (mageFilter)
        {
            Debug.Log("Tengo un " + mageFilter);
            PlayerFilter playerFilter = mageFilter.GetComponent<PlayerFilter>();
            Debug.Log("Ahora tengo un " + playerFilter);


            if (playerFilter)
            {
                Debug.Log("Por último tengo un " + playerFilter);

                playerFilter.SetActive(true);
            }
        }
    }

    private void HandlerGroup4()
    {
        GameObject platparaMage = (GameObject)Instantiate(Resources.Load("Prefabs/MovPlatform"));
        platparaMage.GetComponent<Transform>().position = new Vector2(61f, -9.5f);
        MovingObject platController = platparaMage.GetComponent<MovingObject>();
        platController.startPoint = new Vector2(platparaMage.transform.position.x, platparaMage.transform.position.y);
        platController.endPoint = new Vector2(platparaMage.transform.position.x, platparaMage.transform.position.y + 1.3f);
        platController.moveSpeed = 1f;
        GameObject feedbackswitchwarr = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
        feedbackswitchwarr.GetComponent<Transform>().position = new Vector2(72.86f, -19.3f);
        Destroy(feedbackswitchwarr, 4f);
        GameObject comebackMessage = (GameObject)Instantiate(Resources.Load("Prefabs/ActivateNPC"));
        comebackMessage.transform.position = new Vector2(70f, -19.2f);

        /* Instantiate Arrow feedback y cambiar arrow de warrior*/
        GameObject arrowIndicadora = (GameObject)Instantiate(Resources.Load("Sprites/Arrows/warriorArrowLeft"));
        arrowIndicadora.GetComponent<Transform>().position = new Vector2(70.7f, -20f);
        GameObject arrowFeedback = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/warriorFeedbackSmall"));
        arrowFeedback.GetComponent<Transform>().position = new Vector2(70.7f, -20f);
        ChangeSprite spriteChanger = GameObject.Find("CartelCambiante").GetComponent<ChangeSprite>();
        spriteChanger.SpriteChanger();

    }

    private void HandlerGroup5()
    {
        GameObject platEnginUp = (GameObject)Instantiate(Resources.Load("Prefabs/PlataformaPastVoladora"));
        platEnginUp.GetComponent<Transform>().position = new Vector2(39f, 7.5f);
        GameObject platEnginUp2 = (GameObject)Instantiate(Resources.Load("Prefabs/PlataformaPastVoladora"));
        platEnginUp2.GetComponent<Transform>().position = new Vector2(36f, 7.5f);
    }

    private void HandlerGroup6()
    {
        GameObject exp6 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp6.GetComponent<Transform>().position = new Vector2(14.1f, -6.3f);
        GameObject exp61 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp61.GetComponent<Transform>().position = new Vector2(14.3f, -6.3f);
        GameObject exp62 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp62.GetComponent<Transform>().position = new Vector2(13.6f, -6.3f);
        GameObject exp63 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp63.GetComponent<Transform>().position = new Vector2(13.1f, -6.3f);
        GameObject exp64 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp64.GetComponent<Transform>().position = new Vector2(15f, -6.3f);
        GameObject exp65 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp65.GetComponent<Transform>().position = new Vector2(15f, -6.3f);
    }

    private void HandlerGroup7()
    {
        GameObject exp7 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp7.GetComponent<Transform>().position = new Vector2(62f, -14.23f);
        GameObject exp71 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp71.GetComponent<Transform>().position = new Vector2(62.5f, -14.23f);
        GameObject exp72 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp72.GetComponent<Transform>().position = new Vector2(64f, -14.23f);
        GameObject exp73 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp73.GetComponent<Transform>().position = new Vector2(61.5f, -14.23f);
        GameObject exp74 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp74.GetComponent<Transform>().position = new Vector2(63f, -14.23f);
        GameObject exp75 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp75.GetComponent<Transform>().position = new Vector2(63.5f, -14.23f);
        GameObject exp76 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp76.GetComponent<Transform>().position = new Vector2(62f, -14.23f);
        GameObject exp77 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp77.GetComponent<Transform>().position = new Vector2(62.5f, -14.23f);
        GameObject exp78 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp78.GetComponent<Transform>().position = new Vector2(64f, -14.23f);
        GameObject exp79 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp79.GetComponent<Transform>().position = new Vector2(61.5f, -14.23f);
        GameObject exp71b = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp71b.GetComponent<Transform>().position = new Vector2(63f, -14.23f);
        GameObject exp71c = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
        exp71c.GetComponent<Transform>().position = new Vector2(63.5f, -14.23f);
    }

    private void HandlerGroup8()
    {
        GameObject expPrefab = (GameObject)Instantiate(Resources.Load("InstantiateEXP"));
        expPrefab.transform.position = new Vector2(80.45f, -18.52f);
    }

    private void HandlerGroup9()
    {
        GameObject particleFeedback = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
        particleFeedback.transform.position = new Vector2(26.5f, -43.6f);
        GameObject primerPlat = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
        primerPlat.transform.position = new Vector2(24.78f, -42.31f);
        GameObject primerPlat2 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
        primerPlat2.transform.position = new Vector2(24.78f, -43.16f);
    }

    private void HandlerGroup10()
    {

        GameObject particleFeedback2 = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
        particleFeedback2.transform.position = new Vector2(26.5f, -42.11f);
        GameObject secondPlat = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
        secondPlat.transform.position = new Vector2(24.86f, -41.2f);
        //GameObject primerPlat2 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
        //primerPlat2.transform.position = new Vector2(23.4f, -42.3f);
    }

    private void HandlerGroup11()
    {
        GameObject secondPlat1 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
        secondPlat1.transform.position = new Vector2(26.11f, -40.40f);
    }

    private void HandlerGroup12()
    {
        GameObject openParticles = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
        GameObject tutorialPath = (GameObject)Instantiate(Resources.Load("Prefabs/TutorialPaths"));
        GameObject pathBlockers = GameObject.Find("PathBlocker");
        openParticles.transform.position = new Vector2(32.11f, -39.31f);
        tutorialPath.transform.position = new Vector2(35.6f, -38.95f);
        Destroy(pathBlockers);
    }

    private void HandlerGroup13()
    {
        Debug.Log("Se activaron los Switch");
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        levelManager.localPlayer.respawnPosition = new Vector3(136.15f, -26.33f, 1f);
        Debug.Log(levelManager.localPlayer.respawnPosition);
        levelManager.Respawn();
        Debug.Log("Respawnnnnnnnnnn");
    }

    #endregion

}