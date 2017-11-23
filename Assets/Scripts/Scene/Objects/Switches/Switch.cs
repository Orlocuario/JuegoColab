using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public enum TypeOfActivation { Pisando, Disparando };
    public enum Color { Red, Blue, Yellow, Any };


    public int groupId; //Identificador del grupo de switchs.
    public int individualId; //Identificador del switch dentro del grupo
    public bool desactivable; //Si puede apagarse una vez activado
    public bool on; //Si esta encendido o no
    public TypeOfActivation activation; //Forma en que se activa (pisando o disparando)
    public Color switchColor; //Color del switch. Determina quien lo puede apretar.
    public GroupOfSwitchs switchGroup;
    private SwitchManager manager;
    private bool jobDone = false; //true si es que su grupo de botones ya terminó su función

	public PlannerSwitch switchObj = null;

    private void Start()
    {
		IgnoreCollisionWithPlayers ();
        manager = GameObject.FindGameObjectWithTag("SwitchManager").GetComponent<SwitchManager>();
        manager.Add(this);
        SetSprite();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (activation)
        {
            case TypeOfActivation.Pisando:
                CheckPisando(collision);
                break;
            case TypeOfActivation.Disparando:
                CheckDisparando(collision);
                break;
        }
    }

    private void CheckPisando(Collision2D collision)
    {
        if (CheckIfColliderIsLocalPlayer(collision) && CheckIfColliderIsAbove(collision))
        {
            Activate();
        }
		else if(CheckIfColliderIsLocalPlayer (collision))
        {
			Desactivate();
        }
    }

    private void CheckDisparando(Collision2D collision)
    {
        if (CheckIfColliderIsAttack(collision))
        {
            Activate();
        }
    }

    private bool CheckIfColliderIsLocalPlayer(Collision2D collision)
    {
        GameObject colliderGameObject = collision.collider.gameObject;
        string playerName = colliderGameObject.name;

        if(CheckIfNameMatchWithColor(playerName))
        {
            PlayerController player = colliderGameObject.GetComponent<PlayerController>();
            if (player.localPlayer)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckIfColliderIsAbove(Collision2D collision)
    {
        GameObject colliderGameObject = collision.collider.gameObject;
        return transform.position.y  < (colliderGameObject.transform.position.y);

    }

	private void IgnoreCollisionWithPlayers()
	{
		if (activation == TypeOfActivation.Disparando)
        {
			Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Mage").GetComponent<BoxCollider2D>());
			Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Warrior").GetComponent<BoxCollider2D>());
			Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Engineer").GetComponent<BoxCollider2D>());
        }
	}

    private bool CheckIfObjectMatchWithColor(string name)
    {
        if (switchColor == Color.Any)
        {
            if(name == "Fireball" || name =="Arrow" || name == "Punch"){
                return true;
            }
        }
        switch (name)
        {
            case "Fireball":
                return switchColor == Color.Blue;
            case "Arrow":
                return switchColor == Color.Yellow;
            case "Punch":
                return switchColor == Color.Red;
            default:
                return false;
        }
    }

    private bool CheckIfNameMatchWithColor(string name)
    {
        if(switchColor == Color.Any)
        {
            return true;
        }

        switch (name)
        {
            case "Mage":
                return switchColor == Color.Blue;
            case "Warrior":
                return switchColor == Color.Red;
            case "Engineer":
                return switchColor == Color.Yellow;
            default:
                return false;
        }
    }

    private bool CheckIfColliderIsAttack(Collision2D collision)
    {
        GameObject colliderGameObject = collision.collider.gameObject;

        string objectName = colliderGameObject.tag;

        if (CheckIfObjectMatchWithColor(objectName))
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
                    if(Client.instance.GetLocalPlayer().name == "Warrior")
                    {
                        return true;
                    }
                    break;
                 
            }
        }
        return false;
    }

    private void Activate()
    {
        if (jobDone)
        {
            return;
        }
        bool newOn;
        if (activation == TypeOfActivation.Disparando) {
            newOn = (desactivable && !on) || !desactivable;
        }
        else
        {
            newOn = true;
        }
        if (newOn != on)
        {	          
            on = newOn;
            if(!on && !desactivable)
            {
                return;
            }
            SetSprite();
            SendOnDataToServer(on);

			switchGroup.CheckIfReady (switchObj, FindObjectOfType<Planner> ());
        }
    }

    private void Desactivate()
    {
        if (jobDone)
        {
            return;
        }
        on = false;
        SetSprite();
        SendOnDataToServer(on);
		if (switchObj != null) {
			switchObj.DeactivateSwitch ();
			Planner planner = FindObjectOfType<Planner> ();
			planner.Monitor ();
		}
    }

    public void SetJobDone()
    {
        jobDone = true;
    }

    private int GetIntFromColor(Color color)
    {
        switch (color)
        {
            case Color.Blue:
                return 0;
            case Color.Red:
                return 1;
            case Color.Yellow:
                return 2;
            case Color.Any:
                return 3;
            default:
                throw new Exception("Color invalido");
        }
    }
    
    private Color GetColorFromInt(int entero)
    {
        switch (entero)
        {
            case 0:
                return Color.Blue;
            case 1:
                return Color.Red;
            case 2:
                return Color.Yellow;
            case 3:
                return Color.Any;
            default:
                throw new Exception("Entero no corresponde a un color (recibido desde el servidor)");
        }
    }

    public void SetColorFromInt(int entero)
    {
        switchColor = GetColorFromInt(entero);
    }
    private void SendOnDataToServer(bool data)
    {
        if(data == false && desactivable == false)
        {
            return;
        }
        string message = "ChangeSwitchStatus/" + groupId + "/" + individualId + "/" + data;
        Client.instance.SendMessageToServer(message, true);
    }

    private void SetSprite()
    {
        SpriteRenderer rendererx = this.gameObject.GetComponent<SpriteRenderer>();
        if (on)
        {
           if(activation == TypeOfActivation.Disparando)
            {
                switch (switchColor)
                {
                    case Color.Blue:
                        rendererx.sprite = manager.On11;
                        break;
                    case Color.Red:
                        rendererx.sprite = manager.On21;
                        break;
                    case Color.Yellow:
                        rendererx.sprite = manager.On31;
                        break;
                    case Color.Any:
                        rendererx.sprite = manager.On01;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (switchColor)
                {
                    case Color.Blue:
                        rendererx.sprite = manager.On12;
                        break;
                    case Color.Red:
                        rendererx.sprite = manager.On22;
                        break;
                    case Color.Yellow:
                        rendererx.sprite = manager.On32;
                        break;
                    case Color.Any:
                        rendererx.sprite = manager.On02;
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            if (activation == TypeOfActivation.Disparando)
            {
                switch (switchColor)
                {
                    case Color.Blue:
                        rendererx.sprite = manager.Off11;
                        break;
                    case Color.Red:
                        rendererx.sprite = manager.Off21;
                        break;
                    case Color.Yellow:
                        rendererx.sprite = manager.Off31;
                        break;
                    case Color.Any:
                        rendererx.sprite = manager.Off01;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (switchColor)
                {
                    case Color.Blue:
                        rendererx.sprite = manager.Off12;
                        break;
                    case Color.Red:
                        rendererx.sprite = manager.Off22;
                        break;
                    case Color.Yellow:
                        rendererx.sprite = manager.Off32;
                        break;
                    case Color.Any:
                        rendererx.sprite = manager.Off02;
                        break;
                    default:
                        break;
                }
            }
        }

    }

    public void ReceiveDataFromServer(bool onData)
    {
        Debug.Log("Recibi la info");
        on = onData;
        if (desactivable == false)
        {
            on = true;
        }
        SetSprite();
		switchGroup.CheckIfReady (switchObj, FindObjectOfType<Planner> ());
    }
}
