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

        GameObject punch = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/Punch"));
        PunchController punchController = punch.GetComponent<PunchController>();
        punchController.SetMovement(directionX, attackSpeed, transform.position.x, transform.position.y, this);

        if (!animControl)
        {
            Debug.Log("AnimatorControl not found in " + name);
            return;
        }

        numHits++;
        if (numHits % 2 == 0)
        {
            StartCoroutine(animControl.StartAnimation("Attacking2", this.gameObject));

        }
        else
        {
            StartCoroutine(animControl.StartAnimation("Attacking", this.gameObject));
        }

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
