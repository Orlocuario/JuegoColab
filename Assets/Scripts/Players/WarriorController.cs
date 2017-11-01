using CnControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class WarriorController : PlayerController
{
    GameObject particulas;

    // TODO: refactor this
    private int numHits = 0;
    private int prevNumHits = 0;
    private float damage;
    private double force = 0;
    private int hits = 0;

    protected override void Start()
    {
        base.Start();
        damage = 3;
        particulas = GameObject.Find("ParticulasWarrior");
        particulas.SetActive(false);
    }

    protected override void Attack()
    {

        if (!localPlayer)
        {
            return;
        }

        isAttacking = false;

        bool attackButtonPressed = CnInputManager.GetButtonDown("Attack Button");

        if (attackButtonPressed)
        {
            SendAttackDataToServer();
            CastPunch();
            if (SceneManager.GetActiveScene().name == "Escena2")
            {
                RemoveRockMass();
            }
        }

    }

    public override void SetAttack()
    {
        CastLocalPunch();
    }

    private void CastPunch()
    {
        CastLocalPunch();
        SendAttackDataToServer();
    }

    public void CastLocalPunch()
    {
        isAttacking = true;

        numHits++;
        if (numHits % 2 == 0)
        {
            animator.SetBool("IsAttacking2", isAttacking);

        }
        else
        {
            animator.SetBool("IsAttacking", isAttacking);
        }

        GameObject punch = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/Punch"));
        PunchController punchController = punch.GetComponent<PunchController>();
        punchController.SetPosition(transform.position.x, transform.position.y);

        punch.GetComponent<Transform>().position = new Vector2(transform.position.x, transform.position.y);
    }

    protected override void SetParticlesAnimationState(bool activo)
    {
        particulas.SetActive(activo);
    }

    public void RemoveRockMass()
    {
        Vector3 myPosition = this.GetComponent<Transform>().position;
        GameObject piedra = GameObject.FindGameObjectWithTag("RocaGiganteAraña");
        Rigidbody2D rigidez = piedra.GetComponent<Rigidbody2D>();
        Vector3 posicionPiedra = piedra.GetComponent<Transform>().position;
        if ((myPosition - posicionPiedra).magnitude < 3f)
        {
            hits++;
            if (hits > 7)
            {
                hits = 7;
            }
            force = Math.Pow(hits, damage) / 50;
            rigidez.AddForce(Vector2.right * (float)force);
        }

    }


    public override void OnAttackEnd(string s)
    {
        base.OnAttackEnd(s);
        animator.SetBool("IsAttacking2", false);
    }

}
