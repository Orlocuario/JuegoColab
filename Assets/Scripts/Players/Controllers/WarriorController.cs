using System.Collections;
using UnityEngine;


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
        particulas = GameObject.Find("ParticulasWarrior");
        particulas.SetActive(false);
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
