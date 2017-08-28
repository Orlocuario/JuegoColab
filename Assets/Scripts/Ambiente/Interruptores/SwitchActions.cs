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
        switch(groupId)
        {
			case 3789:
				Animator animadorRoca = GameObject.FindGameObjectWithTag ("rocaql").GetComponent<Animator> ();
				animadorRoca.SetBool ("rocasecae",true);
                break;
            case 1:
                //asdada
                break;
			case 3:
				GameObject platescalera = (GameObject)Instantiate(Resources.Load("Prefabs/Plat0.6"));
				platescalera.GetComponent<Transform>().position = new Vector2 (43f, -17f);
				GameObject feedbackSwitch = (GameObject)Instantiate (Resources.Load ("Prefabs/));

					break;
	            default:
	                break;

			/*GameObject tevoyamataroe = GameObject.FindGameObjectWithTag ("plataforma1");
			GameObject.Destroy (tevoyamataroe);

			GameObject fireball = (GameObject)Instantiate (Resources.Load ("Prefabs/Attacks/BolaM1"));
			fireball.GetComponent<Transform> ().position = new Vector2 (3, 4);*/
        }
    }
}
