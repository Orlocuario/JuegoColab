using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderScene2 : MonoBehaviour
{

    private LevelManager levelManager;
    private Animator animator;

    private Vector3 lastPosition;
    private Vector3 posInicial;
    private Vector3 posFinal;

    private static int positionUpdateFrameRate = 15; // Cantidad de frames que espera para actualizar la posición de la araña 
    private static float minimunDistance = 3.8f;

    private int positionUpdateFrame = 0; // Contador de frames para actualizar la posición de la araña
    private bool[] conColision; // Determina la colisión con la araña para cada player

    void Start()
    {
        conColision = new bool[3] { true, true, true };
        animator = this.gameObject.GetComponent<Animator>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        posInicial = new Vector3(73.38f, 0.73f);
        posFinal = new Vector3(posInicial.x, -3.14f);
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
        if (levelManager.players == null  || levelManager.players.Length == 0)
        {
            return;
        }


        lastPosition = this.gameObject.transform.position;

        Transform player0Transform = levelManager.players[0].GetComponent<Transform>();
        Transform player1Transform = levelManager.players[1].GetComponent<Transform>();
        Transform player2Transform = levelManager.players[2].GetComponent<Transform>();

        GameObject[] players = levelManager.players;

        // Algun player está cerca de la araña
        if (Mathf.Abs(transform.position.x - player0Transform.position.x) < minimunDistance ||
            Mathf.Abs(transform.position.x - player1Transform.position.x) < minimunDistance ||
            Mathf.Abs(transform.position.x - player2Transform.position.x) < minimunDistance)
        {

            //Si no le he avisado a la araña le aviso
            if (!animator.GetBool("llegóPlayer"))
            {
                animator.SetBool("llegóPlayer", true);
            }

            if (Mathf.Abs(player0Transform.position.x - posInicial.x) < minimunDistance && Client.instance.GetMage().InShield(players[0]))
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (Client.instance.GetMage().InShield(players[i]) && conColision[i])
                    {
                        Physics2D.IgnoreCollision(players[i].GetComponent<BoxCollider2D>(), this.gameObject.GetComponent<CircleCollider2D>());
                        SendMessageToServer("IgnoreBoxCircleCollision/true/" + players[i].name + "/" + this.gameObject.name);
                        conColision[i] = false;
                    }
                }
            }
            else if (!conColision[0] || !conColision[1] || !conColision[2])
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (!Client.instance.GetMage().InShield(players[i]))
                    {
                        Physics2D.IgnoreCollision(players[i].GetComponent<BoxCollider2D>(), this.gameObject.GetComponent<CircleCollider2D>(), false);
                        SendMessageToServer("IgnoreBoxCircleCollision/false/" + players[i].name + "/" + this.gameObject.name);
                        conColision[i] = true;
                    }
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
            animator.SetBool("llegóPlayer", false);
            this.gameObject.transform.position = Vector3.MoveTowards(this.transform.position, posInicial, 0.6f);  // La araña sube
        }

        if (lastPosition != transform.position)
        {
            positionUpdateFrame++;

            if (positionUpdateFrame == positionUpdateFrameRate)
            {
                SendMessageToServer("ChangeObjectPosition/" +
                this.gameObject.name + "/" +
                this.gameObject.transform.position.x.ToString() + "/" +
                this.gameObject.transform.position.y.ToString() + "/" +
                this.gameObject.transform.rotation.z.ToString());

                positionUpdateFrame = 0;
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject == levelManager.players[0] ||
            other.collider.gameObject == levelManager.players[1] ||
            other.collider.gameObject == levelManager.players[2])
        {
            GameObject player = other.collider.gameObject;
            Rigidbody2D rgbd = player.GetComponent<Rigidbody2D>();
            if (player.GetComponent<PlayerController>().directionX == -1f)
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

    public void onAttackEnd(string s)
    {
        Debug.Log(s);
        animator.SetBool("llegóPlayer", false);
    }

}
