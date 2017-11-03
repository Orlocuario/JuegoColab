using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour {

    public ParticleSystem particle;
    public float particle_center_x;
    public float particle_center_y;
    public float particle_radius;
    private bool activated;
    // Use this for initialization

    void Start () {
        activated = false;
        particle.transform.position = new Vector2(particle_center_x, particle_center_y);
        ParticleSystem.ShapeModule particle_shape = particle.shape;
        particle_shape.radius = particle_radius;
	}
	
	// Update is called once per frame
	void Update () {
        for(int i=0; i<3; i++)
        {
            PlayerController current_player = Client.instance.GetById(i);
            bool inside = CheckIfInside(current_player);
            if (inside && current_player.gravity)
            {
                //Hace que la gravedad se invierta
                current_player.SetGravity(false);
            }
            else if(!inside && !current_player.gravity)
            {
                //Hace que la gravedad vuelva a la normalidad
                current_player.SetGravity(true);
            }
        }
    }

    bool CheckIfInside(PlayerController player)
    {
        return CheckIfInside(player.transform.position.x, player.transform.position.y);
    }

    bool CheckIfInside(float x, float y)
    {
        //Verifica si es que el punto está dentro de la circunferencia utilizando la ecuación general de una circunferencia
        bool condition = (x - particle_center_x) * (x - particle_center_x) + (y - particle_center_y) * (y - particle_center_y) < particle_radius * particle_radius;
        return activated && condition;
    }

    void ActivateParticles()
    {
        //TODO
        activated = true;
    }

    void DesactivateParticles()
    {
        //TODO
        activated = false;
    }
}
