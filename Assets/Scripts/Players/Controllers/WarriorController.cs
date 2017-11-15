using System.Collections;
using UnityEngine;


public class WarriorController : PlayerController
{
    GameObject particulas;

    private int attacks = 0;

    protected override void Start()
    {
        base.Start();

        particulas = GameObject.Find("ParticulasWarrior");
        particulas.SetActive(false);
    }

    public override void CastLocalAttack()
    {
        isAttacking = true;

        GameObject punch = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/Punch"));
        PunchController punchController = punch.GetComponent<PunchController>();
        punchController.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);

        StartCoroutine(WaitAttacking());

        if (attacks++ % 2 == 0)
        {
            currentAttack = "Attacking2";
        }
        else
        {
            currentAttack = "Attacking";
        }

        AnimateAttack();
    }

    public override void SetAttack()
    {
        CastLocalAttack();
    }

    protected override void SetParticlesAnimationState(bool activo)
    {
        particulas.SetActive(activo);
    }

}
