using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{

    #region Enums

    public enum TypeOfActivation { Stepping, Shooting };
    public enum Color { Red, Blue, Yellow, Any };

    #endregion

    #region Attributes

    public GroupOfSwitchs switchGroup;
    public PlannerSwitch switchObj;

    public TypeOfActivation activation; // Forma en que se activa (pisando o disparando)
    public Color switchColor; // Color del switch. Determina quien lo puede apretar.

    public bool desactivable; // Si puede apagarse una vez activado
    public int individualId; // Identificador del switch dentro del grupo
    public bool isActivated; // Si esta encendido o no
    public int groupId; // Identificador del grupo de switchs.

    private SwitchManager manager;
    private bool jobDone; // true si es que su grupo de botones ya terminó su función
    private ParticleSystem particles;

    #endregion

    #region Start

    private void Start()
    {
        StopMyParticles();
        IgnoreCollisionWithPlayers();
        RegisterOnManager();
        SetSprite();
    }

    #endregion

    #region Common

    private void Activate()
    {
        if (jobDone)
        {
            return;
        }

        isActivated = true;
        SetSprite();
        if (activation == TypeOfActivation.Shooting)
        {
            TurnParticlesOn();
        }
        SendOnDataToServer(isActivated);
        switchGroup.CheckIfReady(switchObj, FindObjectOfType<Planner>());
    }

    private void Desactivate()
    {
        if (jobDone)
        {
            return;
        }

        isActivated = false;
        SetSprite();
        SendOnDataToServer(isActivated);

        if (switchObj != null)
        {
            switchObj.DeactivateSwitch();
            Planner planner = FindObjectOfType<Planner>();
            planner.Monitor();
        }
    }

    #endregion

    #region Events

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool correctActivation = false;

        switch (activation)
        {
            case TypeOfActivation.Stepping:
                correctActivation = ColliderIsCorrectPlayer(collision);
                break;
            case TypeOfActivation.Shooting:
                correctActivation = ColliderIsCorrectAttack(collision);
                break;
        }

        if (correctActivation)
        {
            HandleActivation();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (ColliderIsCorrectPlayer(collision))
        {
            if (desactivable)
            {
                Desactivate();
            }
        }
    }

    #endregion

    #region Utils

    private void HandleActivation()
    {
        if (isActivated)
        {
            if (desactivable)
            {
                Desactivate();
            }
        }
        else
        {
            Activate();
        }
    }

    public void SetJobDone()
    {
        jobDone = true;
    }

    public void ReceiveDataFromServer(bool _isPressed)
    {
        isActivated = _isPressed;
        SetSprite();
        switchGroup.CheckIfReady(switchObj, FindObjectOfType<Planner>());
    }

    private void SetSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (isActivated)
        {
            if (activation == TypeOfActivation.Shooting)
            {
                switch (switchColor)
                {
                    case Color.Blue:
                        spriteRenderer.sprite = manager.ShootBlueOn;
                        break;
                    case Color.Red:
                        spriteRenderer.sprite = manager.ShootRedOn;
                        break;
                    case Color.Yellow:
                        spriteRenderer.sprite = manager.ShootYellowOn;
                        break;
                    case Color.Any:
                        spriteRenderer.sprite = manager.ShootAnyOn;
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
                        spriteRenderer.sprite = manager.StepBlueOn;
                        break;
                    case Color.Red:
                        spriteRenderer.sprite = manager.StepRedOn;
                        break;
                    case Color.Yellow:
                        spriteRenderer.sprite = manager.StepYellowOn;
                        break;
                    case Color.Any:
                        spriteRenderer.sprite = manager.StepAnyOn;
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            if (activation == TypeOfActivation.Shooting)
            {
                switch (switchColor)
                {
                    case Color.Blue:
                        spriteRenderer.sprite = manager.ShootBlueOff;
                        break;
                    case Color.Red:
                        spriteRenderer.sprite = manager.ShootRedOff;
                        break;
                    case Color.Yellow:
                        spriteRenderer.sprite = manager.ShootYellowOff;
                        break;
                    case Color.Any:
                        spriteRenderer.sprite = manager.ShootAnyOff;
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
                        spriteRenderer.sprite = manager.StepBlueOff;
                        break;
                    case Color.Red:
                        spriteRenderer.sprite = manager.StepRedOff;
                        break;
                    case Color.Yellow:
                        spriteRenderer.sprite = manager.StepYellowOff;
                        break;
                    case Color.Any:
                        spriteRenderer.sprite = manager.StepAnyOff;
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

    private bool ColliderIsCorrectPlayer(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player && player.localPlayer)
        {
            return CheckIfPlayerMatchWithColor(other.gameObject);
        }

        return false;
    }

    private void IgnoreCollisionWithPlayers()
    {
        if (activation == TypeOfActivation.Shooting)
        {
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Mage").GetComponent<BoxCollider2D>());
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Warrior").GetComponent<BoxCollider2D>());
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Engineer").GetComponent<BoxCollider2D>());
        }
    }

    private void StopMyParticles()
    {
        particles = gameObject.GetComponent<ParticleSystem>();

        if (particles)
        {
            particles.gameObject.SetActive(false);
        }
    }

    private void TurnParticlesOn()
    {
        if (particles)
        {
            particles.gameObject.SetActive(true);
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

    private bool ColliderIsCorrectAttack(Collision2D other)
    {
        AttackController attack = other.gameObject.GetComponent<AttackController>();

        if (attack)
        {
            return attack.caster.localPlayer && CheckIfAttackMatchWithColor(other.gameObject);
        }

        return false;
    }

    #endregion

    #region Messaging

    private void SendOnDataToServer(bool data)
    {

        string message = "ChangeSwitchStatus/" + groupId + "/" + individualId + "/" + data;

        if (Client.instance)
        {
            Client.instance.SendMessageToServer(message, true);
        }
    }

    #endregion
}
