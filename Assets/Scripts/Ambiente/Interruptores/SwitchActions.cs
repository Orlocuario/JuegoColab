﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActions : MonoBehaviour {

    int groupId;
	bool meActive = false;

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
		if (meActive) {
			return;
		}
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
			platEscaleraEng.GetComponent<Transform> ().position = new Vector2 (12.9f, -1.3f);
			GameObject feedbackswitchEng = (GameObject)Instantiate (Resources.Load ("Prefabs/FeedbackParticles/FBMageButt"));
			feedbackswitchEng.GetComponent<Transform> ().position = new Vector2 (13.2f, -1.3f);
			break; 

		case 1: 
			ParticleSystem particleRoca = (ParticleSystem)Instantiate (Resources.Load ("Prefabs/ParticulasMageRoca"));
            particleRoca.GetComponent<Transform>().position = new Vector2(30.5f, 2.4f);
            GameObject rejaEng = GameObject.FindGameObjectWithTag ("RejaRocaEng");
			rejaEng.SetActive (false);
			Roca roca = GameObject.FindGameObjectWithTag ("RocaGigante").GetComponent <Roca>();
			roca.isReady = true;
            break;
		
		
		case 2:
			Roca rocaGigante = GameObject.FindGameObjectWithTag ("RocaGigante").GetComponent <Roca> ();
			if (rocaGigante.isReady == true) {
				GameObject sujetaRoca = GameObject.FindGameObjectWithTag ("SujetaRoca");
				Animator sujetaRocaAnim = sujetaRoca.GetComponent<Animator> ();
				sujetaRocaAnim.SetBool ("isSwitch", true);
				GameObject rocaCaida = GameObject.FindGameObjectWithTag ("rocaCaida");
				rocaCaida.GetComponent<Animator> ().SetBool ("switchRocaOn", true);
				sujetaRocaAnim.SetBool ("isSwitch", false); 

			} 
			else if (rocaGigante.isReady == false)
            {

				GameObject sujetaRoca = GameObject.FindGameObjectWithTag ("SujetaRoca");
				Animator sujetaRocaAnim = sujetaRoca.GetComponent<Animator> ();
				sujetaRocaAnim.SetBool ("isSwitch", true);

				sujetaRocaAnim.SetBool ("isSwitch", false);
			}

			               
			break;
		case 3:
                GameObject platescalera = (GameObject)Instantiate(Resources.Load("Prefabs/MagePlath"));
                platescalera.GetComponent<Transform>().position = new Vector2(43f, -16f);
                GameObject feedbackswitch = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
                feedbackswitch.GetComponent<Transform>().position = new Vector2(41.4f, -16.3f);
                break;
		
		case 4:
                GameObject platparaMage = (GameObject)Instantiate(Resources.Load("Prefabs/plathParaMageWarrior"));
                platparaMage.GetComponent<Transform>().position = new Vector2(61f, -9.5f);
                GameObject feedbackswitchwarr = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/FBMageButt"));
                feedbackswitchwarr.GetComponent<Transform>().position = new Vector2(72.86f, -19.3f);
                break;
                
		case 5:
				
		
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