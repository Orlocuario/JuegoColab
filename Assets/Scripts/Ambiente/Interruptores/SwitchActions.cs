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
                GameObject platEscaleraEng = (GameObject)Instantiate(Resources.Load("Prefabs/EnginPlath1"));
                platEscaleraEng.GetComponent<Transform>().position = new Vector2(12.9f, -1.3f);
                GameObject feedbackswitchEng = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
                feedbackswitchEng.GetComponent<Transform>().position = new Vector2(13.2f, -1.3f);
                Destroy(feedbackswitchEng, 3f);
                MuereParticula hintPart = GameObject.FindGameObjectWithTag("SwitchEnginPart").GetComponent<MuereParticula>();
                hintPart.living = false;

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
                CameraController mainCamera1 = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
                mainCamera1.ChangeState(CameraState.TargetZoom, 5, 34.9f, -3.06f, false);
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
                GameObject platescalera = (GameObject)Instantiate(Resources.Load("Prefabs/MagePlath1"));
                platescalera.GetComponent<Transform>().position = new Vector2(43f, -16f);
                GameObject feedbackswitch = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
                feedbackswitch.GetComponent<Transform>().position = new Vector2(41.4f, -16.3f);
                break;

            case 4:
                GameObject platparaMage = (GameObject)Instantiate(Resources.Load("Prefabs/plathParaMageWarrior"));
                platparaMage.GetComponent<Transform>().position = new Vector2(61f, -9.5f);
                GameObject feedbackswitchwarr = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
                feedbackswitchwarr.GetComponent<Transform>().position = new Vector2(72.86f, -19.3f);
                Destroy(feedbackswitchwarr, 4f);
                GameObject comebackMessage = (GameObject)Instantiate(Resources.Load("Prefabs/ActivateNPC"));
                comebackMessage.transform.position = new Vector2(70f, -19.2f);
                NPCtrigger mensajeReal = comebackMessage.GetComponent<NPCtrigger>();

                if (mensajeReal.messages == null)
                {
                    mensajeReal.messages = new string[1];
                }

                mensajeReal.messages[0] = "Ahora Debes Regresar. Tal vez alguien te abra la puerta";
                break;

            case 5:
                GameObject platEnginUp = (GameObject)Instantiate(Resources.Load("Prefabs/PlataformaPastVoladora"));
                platEnginUp.GetComponent<Transform>().position = new Vector2(39f, 7.5f);
                GameObject platEnginUp2 = (GameObject)Instantiate(Resources.Load("Prefabs/PlataformaPastVoladora"));
                platEnginUp2.GetComponent<Transform>().position = new Vector2(36f, 7.5f);
                break;

            case 6:
                GameObject platescaleraPrueba = (GameObject)Instantiate(Resources.Load("Prefabs/MagePlathShorter"));
                platescaleraPrueba.GetComponent<Transform>().position = new Vector2(15f, -6.5f);
                GameObject feedbackswitchPrueba = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
                feedbackswitchPrueba.GetComponent<Transform>().position = new Vector2(15f, -6.5f);
                break;

            case 7:
                GameObject exp = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
                exp.GetComponent<Transform>().position = new Vector2(62f, -14.23f);
                GameObject exp1 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
                exp1.GetComponent<Transform>().position = new Vector2(62.5f, -14.23f);
                GameObject exp2 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
                exp2.GetComponent<Transform>().position = new Vector2(64f, -14.23f);
                GameObject exp3 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
                exp3.GetComponent<Transform>().position = new Vector2(61.5f, -14.23f);
                GameObject exp4 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
                exp4.GetComponent<Transform>().position = new Vector2(63f, -14.23f);
                GameObject exp5 = (GameObject)Instantiate(Resources.Load("Prefabs/Ambientales/Exp"));
                exp5.GetComponent<Transform>().position = new Vector2(63.5f, -14.23f);
                break;

            case 8:

                break;

            /* Aquí Comienzan Acciones Escena Tutorial*/

            case 9:

                GameObject particleFeedback = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
                particleFeedback.transform.position = new Vector2(25.72f, 0.1f);
                GameObject primerPlat = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                primerPlat.transform.position = new Vector2(21.8f, 1.1f);
                GameObject primerPlat2 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                primerPlat2.transform.position = new Vector2(20.1f, 1.9f);
                GameObject primerPlat3 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                primerPlat3.transform.position = new Vector2(18.7f, 2.9f);

                break;
            case 10:

                GameObject lilPlatform = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                GameObject lilPlatform2 = (GameObject)Instantiate(Resources.Load("Prefabs/SueloMetalFlotante"));
                lilPlatform.transform.position = new Vector2(46.6f, 3.7f);
                lilPlatform2.transform.position = new Vector2(45.3f, 3.3f);

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