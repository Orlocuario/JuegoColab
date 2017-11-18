using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableTutorial : MonoBehaviour {

	public Sprite wasShot;
	private SpriteRenderer spriteRenderer;
	public Vector2[] instantiatePoints;


	void Start () {
		spriteRenderer = this.gameObject.GetComponent <SpriteRenderer> ();
	}
	
	private void OnCollisionEnter2D (Collision2D collision)
	{
        CheckDisparando(collision);
    }

    private void CheckDisparando(Collision2D collision)
    {
        if (CheckIfColliderIsAttack(collision))
        { 
            Activate();
        }
    }
    private void Activate()
    {

    }

    private bool CheckIfColliderIsAttack(Collision2D collision)
    {
        GameObject colliderGameObject = collision.collider.gameObject;

        string objectName = colliderGameObject.tag;
        {
            switch (objectName)
            {
                case "Fireball":
                    Destroy(colliderGameObject);
                    if (Client.instance.GetLocalPlayer().name == "Mage")
                    {
                        return true;
                    }
                    break;
                case "Arrow":
                    Destroy(colliderGameObject);
                    if (Client.instance.GetLocalPlayer().name == "Engineer")
                    {
                        return true;
                    }
                    break;
                case "Punch":
                    if (Client.instance.GetLocalPlayer().name == "Warrior")
                    {
                        return true;
                    }
                    break;

            }
        }
        return false;
    }
}
