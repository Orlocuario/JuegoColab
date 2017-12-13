using UnityEngine;

public class SlideRock : DamagingObject
{
    private GameObject pasadizo;
    #region Start

    protected override void Start()
    {
        base.Start();
        damage = 50;
        pasadizo = GameObject.Find("PasadizoJ1J2");
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

        SendMessageToServer("ObstacleDestroyed/" + name, true);
        SendMessageToServer("ObstacleDestroyed/" + pasadizo.name, true);

        Destroy(particulasEffect, .1f);
        Destroy(pasadizo, .1f);
        Destroy(gameObject, .1f);
    }

    #endregion

    #region Events

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "PasadizoJ1J2")
        {
            Debug.Log("Entré a pitearme el pasadizo porque soy BrígiDO");
            KillAndDestroy(collision.gameObject);
        }

    }

    #endregion

    #region Messaging

    private void SendMessageToServer(string message, bool secure)
    {
        if (Client.instance && Client.instance.GetLocalPlayer() && Client.instance.GetLocalPlayer().controlOverEnemies)
        {
            Client.instance.SendMessageToServer(message, secure);
        }
    }

    #endregion

}
