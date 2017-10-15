using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderScene2 : MonoBehaviour {

    LevelManager levelManagerScript;
    Vector3 posInicial;
	Vector3 posFinal;
	Vector3 auxPosition;
	private bool conColision;
    private static float minimunDistance = 3.8f;
	private Animator animAraña;
    int counter = 0; //Lleva un contador de las iteraciones sobre el Update, con el fin de hacer ciertas acciones cada X updates.

	void Start ()
    {
		animAraña = this.gameObject.GetComponent <Animator> ();
		conColision =  true;
        levelManagerScript = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        posInicial = new Vector3(73.38f, 0.73f);
		posFinal = new Vector3 (posInicial.x, -3.14f);
        Physics2D.IgnoreCollision(GameObject.Find("RocaGiganteAraña").GetComponents<CircleCollider2D>()[0], this.gameObject.GetComponent<CircleCollider2D>());
        Physics2D.IgnoreCollision(GameObject.Find("RocaGiganteAraña").GetComponents<CircleCollider2D>()[1], this.gameObject.GetComponent<CircleCollider2D>());
    }

    void SendMessageToServer(string message)
    {
        if (Client.instance.GetLocalPlayer().controlOverEnemies)
        {
            Client.instance.SendMessageToServer(message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        auxPosition = this.gameObject.transform.position;

        Transform player0Transform = levelManagerScript.players[0].GetComponent<Transform>();
        Transform player1Transform = levelManagerScript.players[1].GetComponent<Transform>();
        Transform player2Transform = levelManagerScript.players[2].GetComponent<Transform>();
		GameObject[] players = levelManagerScript.players;

		// Algun player está cerca de la araña
        if (Mathf.Abs(transform.position.x - player0Transform.position.x) < minimunDistance || 
            Mathf.Abs(transform.position.x - player1Transform.position.x) < minimunDistance ||
            Mathf.Abs(transform.position.x - player2Transform.position.x) < minimunDistance)
        {

			//Si no le he avisado a la araña le aviso
			if (!animAraña.GetBool ("llegóPlayer")) {
				animAraña.SetBool ("llegóPlayer", true);
			}


			if (Mathf.Abs (player0Transform.position.x - posInicial.x) < minimunDistance && Client.instance.GetMage ().InShield (players [0]) && conColision) {
				foreach (GameObject player in players) 
				{
					Physics2D.IgnoreCollision (player.GetComponent<BoxCollider2D> (), this.gameObject.GetComponent<CircleCollider2D> ());
					SendMessageToServer("IgnoreBoxCircleCollision/true/" + player.name + "/" + this.gameObject.name);
					conColision = false;
				}
			}
			else if ((!Client.instance.GetMage ().InShield (players [0]) || Mathf.Abs (player0Transform.position.x - posInicial.x) > minimunDistance) && !conColision)
			{
				foreach (GameObject player in players) 
				{
					Physics2D.IgnoreCollision (player.GetComponent<BoxCollider2D> (), this.gameObject.GetComponent<CircleCollider2D> (), false);
					SendMessageToServer ("IgnoreBoxCircleCollision/false/" + player.name + "/" + this.gameObject.name);
					conColision = true;
				}
            }

            if ((Mathf.Abs(player0Transform.position.x - posInicial.x) < minimunDistance && player0Transform.position.y <= 0f) ||
                (Mathf.Abs(player1Transform.position.x - posInicial.x) < minimunDistance && player1Transform.position.y <= 0f) ||
                (Mathf.Abs(player2Transform.position.x - posInicial.x) < minimunDistance && player2Transform.position.y <= 0f))  // La araña está arriba y alguien aparece abajo y/o arriba.
            {
				this.gameObject.transform.position = Vector3.MoveTowards(this.transform.position, posFinal, 0.6f);  // Baja la araña
            }

            else if (transform.position.y != posInicial.y)  // La araña está abajo, no hay nadie abajo
            {
				this.gameObject.transform.position = Vector3.MoveTowards(this.transform.position, posInicial, 0.6f);  // La araña sube
            }
        } 

		else  // No hay nadie cerca
        
		{
			animAraña.SetBool ("llegóPlayer", false);
			this.gameObject.transform.position = Vector3.MoveTowards(this.transform.position, posInicial, 0.6f);  // La araña sube
        }

        if (auxPosition != transform.position)
        {
            counter++;
            if(counter == 15)
            {
                SendMessageToServer("ChangeObjectPosition/" + this.gameObject.name + "/" +
                this.gameObject.transform.position.x.ToString() + "/" + this.gameObject.transform.position.y.ToString() + "/" + this.gameObject.transform.rotation.z.ToString());
                counter = 0;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject == levelManagerScript.players[0] ||
            other.collider.gameObject == levelManagerScript.players[1] ||
            other.collider.gameObject == levelManagerScript.players[2])
        {
            GameObject player = other.collider.gameObject;
            Rigidbody2D rgbd = player.GetComponent<Rigidbody2D>();
            if (player.GetComponent<PlayerController>().direction == -1)
            {
                rgbd.AddForce(new Vector2(3500f, 150f));
            }
            else
            {
                rgbd.AddForce(new Vector2(-3500f, 150f));
            }
            SendMessageToServer("ChangeHpHUDToRoom/-5");
        }
    }

	public void onAttackEnd (string s)
	{
		Debug.Log (s);
		animAraña.SetBool ("llegóPlayer", false);
	}

}
