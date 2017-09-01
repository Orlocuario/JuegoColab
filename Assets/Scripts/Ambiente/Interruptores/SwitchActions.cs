using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActions : MonoBehaviour {

    int groupId;

    public SwitchActions(GroupOfSwitchs group)
    {
        this.groupId = group.groupId;
        foreach(Switch switchi in group.GetSwitchs())
        {
            switchi.SetJobDone();
        }
    }

	public void DoSomething()
	{
		switch (groupId) {
		case 3789:
			/*GameObject camaraObject = GameObject.FindGameObjectWithTag ("MainCamera");
			CameraController camaraScript = camaraObject.GetComponent<CameraController> ();
			camaraScript.ChangeState (CameraState.Zoomed, -100);*/


			//Animator animadorRoca = GameObject.FindGameObjectWithTag ("rocaql").GetComponent<Animator> ();
			//animadorRoca.SetBool ("rocasecae", true);
			break;

		case 0:
			GameObject platEscaleraEng = (GameObject)Instantiate (Resources.Load ("Prefabs/EnginPlath1"));
			platEscaleraEng.GetComponent<Transform> ().position = new Vector2 (12.9f, -1.4f);
			GameObject feedbackswitchEng = (GameObject)Instantiate (Resources.Load ("Prefabs/FeedbackParticles/FBMageButt"));
			feedbackswitchEng.GetComponent<Transform> ().position = new Vector2 (8.224f, -0.976f);
			break;

		case 1: 
			GameObject rejaMage = GameObject.FindGameObjectWithTag ("RejaRocaMage");
			GameObject rejaEng = GameObject.FindGameObjectWithTag ("RejaRocaEng");
			rejaMage.SetActive (false);
			rejaEng.SetActive (false);
			bool case1 = true;
			break;
		
		
		case 3:
			if (case1 = false) 
			{
				bool isSwitch = true;
				GameObject sujetaRoca = GameObject.FindGameObjectWithTag ("SujetaRoca");
				Animator sujetaRocaAnim = sujetaRoca.GetComponent<Animator> ();
				sujetaRocaAnim.SetBool ("isSwitch", isSwitch);
			}
			if (case1 = true) 
			{
				bool isSwitch = true;
				GameObject sujetaRoca = GameObject.FindGameObjectWithTag ("SujetaRoca");
				Animator sujetaRocaAnim = sujetaRoca.GetComponent<Animator> ();
				sujetaRocaAnim.SetBool ("isSwitch", isSwitch);
				GameObject rocaCaida = GameObject.FindGameObjectWithTag ("rocaCaida");
				rocaCaida.GetComponent<Animator> ().SetBool ("switchRocaOn", case1);
				GameObject arbolCaida = GameObject.FindGameObjectWithTag ("ArbolCaida");
				Animator arbolCaidaAnim = arbolCaida.GetComponent<Animator> ();
				arbolCaidaAnim.SetBool ("RockBottom", true);
			}
			               
			break;
		
		
		case 2:
			GameObject platescalera = (GameObject)Instantiate (Resources.Load ("Prefabs/MagePlath"));
			platescalera.GetComponent<Transform> ().position = new Vector2 (43f, -16f);
			GameObject feedbackswitch = (GameObject)Instantiate (Resources.Load ("Prefabs/FeedbackParticles/FBMageButt"));
			feedbackswitch.GetComponent<Transform> ().position = new Vector2 (41.4f, -16.3f);
			break;

        case 45:
            Debug.Log("PICO");
            break;
		default:
			break;

		//GameObject tevoyamataroe = GameObject.FindGameObjectWithTag ("plataforma1");
		//GameObject.Destroy (tevoyamataroe);
		//GameObject fireball = (GameObject)Instantiate (Resources.Load ("Prefabs/Attacks/BolaM1"));
		//fireball.GetComponent<Transform> ().position = new Vector2 (3, 4);*/
			}
		}
	}