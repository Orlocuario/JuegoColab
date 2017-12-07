using UnityEngine;

public class SlideRock : KillingObject
{

    #region Start

    protected override void Start()
    {
        activated = true;
        damage = 50;
    }

    #endregion

    #region Common

    public void Slide()
    {
        SceneAnimator sceneAnimator = GameObject.FindObjectOfType<SceneAnimator>();
        sceneAnimator.SetBool("caidaOn", true, this.gameObject);
    }

    private void KillAndDestroy(GameObject pasadizo)
    {
        GameObject humo = (GameObject)Instantiate(Resources.Load("Prefabs/FeedbackParticles/Humo"));
        humo.GetComponent<Transform>().position = new Vector2(34.1f, -7.07f);

        Destroy(humo, 5f);

        GameObject particulasEffect = GameObject.Find("ParticulasMageRoca");

        Destroy(particulasEffect, .1f);
        Destroy(pasadizo, .1f);
        Destroy(this.gameObject, .1f);
    }

    #endregion

    #region Events

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "PasadizoJ1J2")
        {
            Debug.Log("Entré a pitearme el pasadizo porque soy BrígiDO");
            KillAndDestroy(collision.gameObject);
        }

    }
    
    #endregion
    
}
