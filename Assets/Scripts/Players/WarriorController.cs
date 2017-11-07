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
    private double force = 0;
    private int numHits = 0;
    private int hits = 0;
    private float damage;

    protected override void Start()
    {
        base.Start();

        damage = 3;
        attackSpeed = 2;
        particulas = GameObject.Find("ParticulasWarrior");
        particulas.SetActive(false);
    }

    protected override void CastAttack()
    {
        CastLocalAttack();
        SendAttackDataToServer();

        // TODO: refactor this
        if (SceneManager.GetActiveScene().name == "Escena2")
        {
            RemoveRockMass();
        }
    }

    public override void CastLocalAttack()
    {
        isAttacking = true;

        numHits++;
        if (numHits % 2 == 0)
        {
            animator.SetBool("IsAttacking2", isAttacking);
            currentAttackName = "WarriorAttack2";
        }
        else
        {
            animator.SetBool("IsAttacking", isAttacking);
            currentAttackName = "WarriorAttack";
        }

        GameObject punch = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/Punch"));
        PunchController punchController = punch.GetComponent<PunchController>();
        punchController.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);

        StartCoroutine("Attacking");
    }

    public override void SetAttack()
    {
        CastLocalAttack();
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

    public override IEnumerator Attacking()
    {
        float animLength = attackAnimLength[currentAttackName];

        yield return new WaitForSeconds(animLength);

        isAttacking = false;

        animator.SetFloat("Speed", Mathf.Abs(speedX));
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("IsAttacking2", isAttacking);
    }

}
