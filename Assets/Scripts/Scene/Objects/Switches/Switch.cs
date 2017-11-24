using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{

    #region Enums

    public enum TypeOfActivation { Pisando, Disparando };
    public enum Color { Red, Blue, Yellow, Any };

    #endregion

    #region Attributes

    public GroupOfSwitchs switchGroup;
    public PlannerSwitch switchObj;

    public TypeOfActivation activation; // Forma en que se activa (pisando o disparando)
    public Color switchColor; // Color del switch. Determina quien lo puede apretar.

    public bool desactivable; // Si puede apagarse una vez activado
    public int individualId; // Identificador del switch dentro del grupo
    public int groupId; // Identificador del grupo de switchs.
    public bool on; // Si esta encendido o no

    private SwitchManager manager;
    private bool jobDone; // true si es que su grupo de botones ya terminó su función

    #endregion

    #region Start

    private void Start()
    {
        IgnoreCollisionWithPlayers();
        RegisterOnManager();
        SetSprite();
    }

    #endregion

    #region Utils


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

    public void ReceiveDataFromServer(bool onData)
    {
        on = onData;
        if (desactivable == false)
        {
            on = true;
        }
        SetSprite();
        switchGroup.CheckIfReady(switchObj, FindObjectOfType<Planner>());

    }

    private void SetSprite()
    {
        SpriteRenderer rendererx = this.gameObject.GetComponent<SpriteRenderer>();
        if (on)
        {
            if (activation == TypeOfActivation.Disparando)
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

    protected void RegisterOnManager()
    {
        manager = GameObject.FindObjectOfType<SwitchManager>();
        manager.Add(this);
    }

    private void CheckPisando(Collision2D other)
    {
        if (ColliderIsCorrectPlayer(other))
        {
            if (CheckIfColliderIsAbove(other))
            {
                Activate();
            }
            else
            {
                Desactivate();
            }
        }
    }

    private void CheckDisparando(Collision2D collision)
    {
        if (CheckIfColliderIsAttack(collision))
        {
            Activate();
        }
    }

    private bool ColliderIsCorrectPlayer(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        return player.localPlayer && CheckIfPlayerMatchWithColor(other.gameObject);
    }

    // CUANDO SE USA ESTO??
    private bool CheckIfColliderIsAbove(Collision2D collision)
    {
        GameObject colliderGameObject = collision.collider.gameObject;
        return transform.position.y < (colliderGameObject.transform.position.y);

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

    private bool CheckIfAttackMatchWithColor(GameObject gameObject)
    {
        switch (switchColor)
        {
            case Color.Blue:
                return gameObject.GetComponent<FireballController>();
            case Color.Yellow:
                return gameObject.GetComponent<ProyectileController>();
            case Color.Red:
                return gameObject.GetComponent<PunchController>();
            case Color.Any:
                return gameObject.GetComponent<AttackController>(); ;
            default:
                return false;
        }
    }

    private bool CheckIfPlayerMatchWithColor(GameObject gameObject)
    {

        switch (switchColor)
        {
            case Color.Blue:
                return gameObject.GetComponent<MageController>();
            case Color.Yellow:
                return gameObject.GetComponent<EngineerController>();
            case Color.Red:
                return gameObject.GetComponent<WarriorController>();
            case Color.Any:
                return gameObject.GetComponent<PlayerController>(); ;
            default:
                return false;
        }

    }

    private bool CheckIfColliderIsAttack(Collision2D other)
    {
        AttackController attack = other.gameObject.GetComponent<AttackController>();

        if (attack)
        {
            return attack.caster.localPlayer && CheckIfAttackMatchWithColor(other.gameObject);
        }

        return false;
    }

    #endregion

    #region Events

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

    #endregion

    #region Common

    private void Activate()
    {
        if (jobDone)
        {
            return;
        }

        bool newOn;
        if (activation == TypeOfActivation.Disparando)
        {
            newOn = (desactivable && !on) || !desactivable;
        }
        else
        {
            newOn = true;
        }
        if (newOn != on)
        {
            on = newOn;
            if (!on && !desactivable)
            {
                return;
            }
            SetSprite();
            SendOnDataToServer(on);

            switchGroup.CheckIfReady(switchObj, FindObjectOfType<Planner>());
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
        if (switchObj != null)
        {
            switchObj.DeactivateSwitch();
            Planner planner = FindObjectOfType<Planner>();
            planner.Monitor();
        }
    }

    #endregion

    #region Messaging

    private void SendOnDataToServer(bool data)
    {
        if (data == false && desactivable == false)
        {
            return;
        }
        string message = "ChangeSwitchStatus/" + groupId + "/" + individualId + "/" + data;

        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, true);
        }
    }

    #endregion
}
