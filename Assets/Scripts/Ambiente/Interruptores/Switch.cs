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

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("SwitchManager").GetComponent<SwitchManager>();
        manager.Add(this);
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
        string playerTag = colliderGameObject.tag;
        if(CheckIfTagMatchWithColor(playerTag))
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
        return transform.position.y < (colliderGameObject.transform.position.y + 0.001);

    }


    private bool CheckIfTagMatchWithColor(string tag)
    {
        if(switchColor == Color.Any)
        {
            return true;
        }
        switch (tag)
        {
            case "Player1":
                return switchColor == Color.Blue;
            case "Player2":
                return switchColor == Color.Red;
            case "Player3":
                return switchColor == Color.Yellow;
            default:
                return false;
        }
    }

    private bool CheckIfColliderIsAttack(Collision2D collision)
    {
        throw new NotImplementedException();
    }

    private void Activate()
    {
        if (jobDone)
        {
            return;
        }

        bool newOn =(desactivable && !on) || !desactivable;
        if(newOn != on)
        {
            on = newOn;
            SetSprite();
            SendOnDataToServer(on);
            switchGroup.CheckIfReady();      
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
        string message = "ChangeSwitchStatus/" + groupId + "/" + individualId + "/" + data;
        Client.instance.SendMessageToServer(message);
    }

    private void SetSprite()
    {
        throw new NotImplementedException();
    }

    public void ReceiveDataFromServer(bool onData)
    {
        on = onData;
        SetSprite();
    }
}
