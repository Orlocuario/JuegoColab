using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActions : MonoBehaviour
{

    int groupId;
    bool meActive = false;

    public GameObject exp;
    public float gridX = 5f;
    public float gridY = 5f;
    public float spacing = 2f;

    public SwitchActions(GroupOfSwitchs group)
    {
        this.groupId = group.groupId;
        foreach (Switch switchi in group.GetSwitchs())
        {
            switchi.SetJobDone();
        }
    }

    public void DoSomething()
    {
        if (meActive)
        {
            return;
        }
        switch (groupId)
        {
            case 3789:
                /*GameObject camaraObject = GameObject.FindGameObjectWithTag ("MainCamera");
                CameraController camaraScript = camaraObject.GetComponent<CameraController> ();
                camaraScript.ChangeState (CameraState.Zoomed, -100);*/


                //Animator animadorRoca = GameObject.FindGameObjectWithTag ("rocaql").GetComponent<Animator> ();
                //animadorRoca.SetBool ("rocasecae", true);
                break;


            //Aquí Comienzan Acciones Switch Etapa 1

            case 0:
                GameObject firstPlatform = (GameObject)Instantiate(Resources.Load("Prefabs/MovPlatform"));
                firstPlatform.GetComponent<Transform>().position = new Vector2(13.3f, -1f);
                MovingObject firstPlatformScript = firstPlatform.GetComponent<MovingObject>();
                firstPlatformScript.startPoint = new Vector2(13.5f, -1.77f);
                firstPlatformScript.endPoint = new Vector2(13.5f, 0.36f);
                firstPlatformScript.moveSpeed = 1f;
                GameObject feedbackswitchEng = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
                feedbackswitchEng.GetComponent<Transform>().position = new Vector2(13.2f, -1.3f);
                Destroy(feedbackswitchEng, 3f);
                //MuereParticula hintPart = GameObject.FindGameObjectWithTag("SwitchEnginPart").GetComponent<MuereParticula>();
                //hintPart.living = false;
                break;

            case 1:
                GameObject rejaEng = GameObject.FindGameObjectWithTag("RejaRocaEng");
                rejaEng.SetActive(false);
                Roca roca = GameObject.FindGameObjectWithTag("rocaCaida").GetComponent<Roca>();
                roca.isReady = true;
                GameObject particleRoca = (GameObject)Instantiate(Resources.Load("Prefabs/ParticulasMageRoca"));
                CajaSwitch caja = GameObject.FindGameObjectWithTag("CajaSwitchFierro").GetComponent<CajaSwitch>();
                caja.meVoy = true;
                caja.ahoraMeVoy = true;
                GameObject spikes = GameObject.FindGameObjectWithTag("KillPlaneSpikes");
                Destroy(spikes);
                GameObject lavaPool = GameObject.Find("LavaPool");
                Destroy(lavaPool, 1f);
                break;

            case 2:
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
                Debug.Log("wiiiiiii");
                break;

		case 3:
			GameObject platescalera = (GameObject)Instantiate (Resources.Load ("Prefabs/MovPlatform"));
			platescalera.GetComponent<Transform> ().position = new Vector2 (43f, -16f);
			MovingObject secondPlatform = platescalera.GetComponent<MovingObject> ();
			secondPlatform.startPoint = new Vector2 (platescalera.transform.position.x, platescalera.transform.position.y);
			secondPlatform.endPoint = new Vector2 (platescalera.transform.position.x, platescalera.transform.position.y + 3.9f);
			secondPlatform.moveSpeed = 1f;
			GameObject feedbackswitch = (GameObject)Instantiate (Resources.Load ("Prefabs/FeedbackParticles/FBMageButt"));
			feedbackswitch.GetComponent<Transform> ().position = new Vector2 (41.4f, -16.3f);

			GameObject mageFilter = GameObject.Find("FilterMage");

                if (mageFilter)
                {
					Debug.Log ("Tengo un " + mageFilter);
                    PlayerFilter playerFilter = mageFilter.GetComponent<PlayerFilter>();
					Debug.Log ("Ahora tengo un " + playerFilter);


                    if (playerFilter)
                    {
					Debug.Log ("Por último tengo un " + playerFilter);

                        playerFilter.SetActive(true);
                    }
                }
                break;

            case 4:
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

                /* Instantiate Arrow feedback*/
                GameObject arrowIndicadora = (GameObject)Instantiate(Resources.Load("Sprites/Arrows/warriorArrowLeft"));
                arrowIndicadora.GetComponent<Transform>().position = new Vector2(70.7f, -20f);
                GameObject arrowFeedback = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/warriorFeedbackSmall"));
                arrowFeedback.GetComponent<Transform>().position = new Vector2(70.7f, -20f);


                break;

            case 5:
                GameObject platEnginUp = (GameObject)Instantiate(Resources.Load("Prefabs/PlataformaPastVoladora"));
                platEnginUp.GetComponent<Transform>().position = new Vector2(39f, 7.5f);
                GameObject platEnginUp2 = (GameObject)Instantiate(Resources.Load("Prefabs/PlataformaPastVoladora"));
                platEnginUp2.GetComponent<Transform>().position = new Vector2(36f, 7.5f);
                break;

            case 6:
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
                break;

            case 7:
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
                break;

            case 8:

                break;

            /* Aquí Comienzan Acciones Escena Tutorial*/

            /* Start Switches Mage */

            case 9:     // Primeros peldaños para mage
                GameObject particleFeedback = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
                particleFeedback.transform.position = new Vector2(26.5f, -43.6f);
                GameObject primerPlat = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                primerPlat.transform.position = new Vector2(23f, -43.5f);
                GameObject primerPlat2 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                primerPlat2.transform.position = new Vector2(23.4f, -42.3f);
                    
                break;

            case 10:    // peldaño switch 2

                GameObject particleFeedback2 = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
                particleFeedback2.transform.position = new Vector2(26.5f, -42.11f);
                GameObject secondPlat = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                secondPlat.transform.position = new Vector2(23f, -41.2f);
                //GameObject primerPlat2 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                //primerPlat2.transform.position = new Vector2(23.4f, -42.3f);


                break;

            case 11:    // peldaño 3rd Switch + Exp
                GameObject secondPlat1 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                secondPlat1.transform.position = new Vector2(24.3f, -40.5f);
                break;


            case 12:    // disparo primeros 2 switch. 
                GameObject lilPlatform = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                lilPlatform.transform.position = new Vector2(44.1f, 3.75f);
                break;

            case 13:    // Disparo tercer switch
                GameObject lilMovPlatform = (GameObject)Instantiate(Resources.Load("Prefabs/MovPlatform"));
                lilMovPlatform.transform.position = new Vector2(43.1f, 3.7f);
                MovingObject movPlat1 = lilMovPlatform.GetComponent<MovingObject>();
                movPlat1.startPoint = new Vector2(43.1f, 3.7f);
                movPlat1.endPoint = new Vector2(43.1f, 7f);
                movPlat1.moveSpeed = 1f;
                break;

            case 14:   // Dispar Switches que se mueven
                GameObject lilMovPlatform2 = (GameObject)Instantiate(Resources.Load("Prefabs/MovPlatform"));
                lilMovPlatform2.transform.position = new Vector2(35.5f, 6.72f);
                MovingObject movPlat2 = lilMovPlatform2.GetComponent<MovingObject>();
                movPlat2.startPoint = new Vector2(36.5f, 6.72f);
                movPlat2.endPoint = new Vector2(36.5f, 9.35f);
                movPlat2.moveSpeed = 1f;
                break;

            case 15:
                GameObject lilPlatform2 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                lilPlatform2.transform.position = new Vector2(40.03f, 12.17f);
                GameObject lilPlatform3 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                lilPlatform3.transform.position = new Vector2(38.6f, 11.58f);
                //GameObject hojasArbol = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/HojasPlat"));
                //hojasArbol.transform.position = new Vector2(35.55f, 12.03f);
                break;

            case 16:
                GameObject lilMovPlatform3 = (GameObject)Instantiate(Resources.Load("Prefabs/MovPlatform"));
                lilMovPlatform3.transform.position = new Vector2(50.35f, 14f);
                MovingObject movPlat3 = lilMovPlatform3.GetComponent<MovingObject>();
                movPlat3.startPoint = new Vector2(50.35f, 14f);
                movPlat3.endPoint = new Vector2(50.35f, 21f);
                movPlat3.moveSpeed = 1.2f;
                GameObject plataformaFinal = (GameObject)Instantiate(Resources.Load("Prefabs/PlataformaPastVoladora"));
                plataformaFinal.transform.position = new Vector2(48.37f, 21f);
                break;

            default:
                break;

                //GameObject tevoyamataroe = GameObject.FindGameObjectWithTag ("plataforma1");
                //GameObject.Destroy (tevoyamataroe);
                //GameObject fireball = (GameObject)Instantiate (Resources.Load ("Prefabs/Attacks/BolaM1"));
                //fireball.GetComponent<Transform> ().position = new Vector2 (3, 4);*/
        }
        meActive = true;
    }
}