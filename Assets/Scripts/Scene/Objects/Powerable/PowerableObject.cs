using UnityEngine;
using System.Reflection;

public class PowerableObject : MonoBehaviour
{

    #region Attributes

    public AttackController power;

    protected GameObject[] particles;
    protected bool powered;

    #endregion

    #region Start

    protected void Start()
    {
        InitializeParticles();
    }

    #endregion

    #region Common

    public void PowerObject(GameObject attack)
    {
        if (!powered)
        {
            powered = true;
            ToogleParticles(true);
            // TODO: Notify everyone
        }
    }

    #endregion

    #region Events

    protected void OnCollisionEnter2D(Collision2D collision)
    {

        if (CollidedWithPower(collision.gameObject))
        {
            PowerObject(collision.gameObject);
        }

    }

    #endregion

    #region Utils

    protected bool CollidedWithPower(GameObject gameObject)
    {
        AttackController attack = gameObject.GetComponent<AttackController>();
        return attack && attack.GetType().Equals(power.GetType()) && attack.isPowered();
    }

    protected void ToogleParticles(bool activate)
    {
        if (particles != null && particles.Length > 0)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(activate);
            }
        }
    }

    protected void InitializeParticles()
    {
        ParticleSystem[] _particles = gameObject.GetComponentsInChildren<ParticleSystem>();

        if (_particles.Length <= 0)
        {
            return;
        }

        particles = new GameObject[_particles.Length];

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i] = _particles[i].gameObject;
        }

        ToogleParticles(false);

    }

    #endregion

    #region Messaging

    // Notify everyone

    #endregion

}