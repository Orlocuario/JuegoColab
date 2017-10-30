using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : EnemyController
{
    private Vector3 lastPosition;
    private Vector3 posInicial;
    private Vector3 posFinal;

    public CircleCollider2D collisionZone;

    private static int positionUpdateFrameRate = 15; // Cantidad de frames que espera para actualizar la posición de la araña 
    private static float minimunDistance = 3.8f;

    private int positionUpdateFrame = 0; // Contador de frames para actualizar la posición de la araña
    private Dictionary<string, bool> ignoresCollisions; // Determina la colisión con la araña para cada player

    protected override void Start()
    {
        base.Start();

        force = new Vector2(3500f, 150f);
        damage = 5;

        ignoresCollisions = new Dictionary<string, bool> { { "Mage", false }, { "Warrior", false }, { "Engineer", false } };
        posInicial = new Vector3(73.38f, 0.73f);
        posFinal = new Vector3(posInicial.x, -3.14f);

        Physics2D.IgnoreCollision(GameObject.Find("RocaGiganteAraña").GetComponents<CircleCollider2D>()[0], gameObject.GetComponent<CircleCollider2D>());
        Physics2D.IgnoreCollision(GameObject.Find("RocaGiganteAraña").GetComponents<CircleCollider2D>()[1], gameObject.GetComponent<CircleCollider2D>());
    }

    // Update is called once per frame
    protected override void Update()
    {

        if (levelManager.players == null || levelManager.players.Length == 0)
        {
            return;
        }

        lastPosition = transform.position;

        Transform player0Transform = levelManager.players[0].GetComponent<Transform>();
        Transform player1Transform = levelManager.players[1].GetComponent<Transform>();
        Transform player2Transform = levelManager.players[2].GetComponent<Transform>();

        GameObject[] players = levelManager.players;

        // Algun player está cerca de la araña
        if (Mathf.Abs(transform.position.x - player0Transform.position.x) < minimunDistance ||
            Mathf.Abs(transform.position.x - player1Transform.position.x) < minimunDistance ||
            Mathf.Abs(transform.position.x - player2Transform.position.x) < minimunDistance)
        {

            /* if (Mathf.Abs(player0Transform.position.x - posInicial.x) < minimunDistance && Client.instance.GetMage().ProtectedByShield(players[0]))
             {
                 for (int i = 0; i < players.Length; i++)
                 {
                     if (Client.instance.GetMage().ProtectedByShield(players[i]) && conColision[i])
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
                     if (!Client.instance.GetMage().ProtectedByShield(players[i]))
                     {
                         Physics2D.IgnoreCollision(players[i].GetComponent<BoxCollider2D>(), this.gameObject.GetComponent<CircleCollider2D>(), false);
                         SendMessageToServer("IgnoreBoxCircleCollision/false/" + players[i].name + "/" + this.gameObject.name);
                         conColision[i] = true;
                     }
                 }
             } */

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

    protected override void DealDamage(GameObject player)
    {

        PlayerController playerController = player.GetComponent<PlayerController>();
        MageController mage = Client.instance.GetMage();

        Vector2 playerPosition = player.transform.position;
        Vector2 attackForce = force;

        // Only hit local players
        if (!playerController.localPlayer)
        {
            return;
        }

        // Don't hit protected players
        if (mage.ProtectedByShield(player))
        {
            if (!ignoresCollisions[player.name])
            {
                Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), collisionZone);
                SendIgnoreCollisionDataToServer(player, true);
                ignoresCollisions[player.name] = true;
            }
            return;
        }
        else
        {
            if (ignoresCollisions[player.name])
            {
                Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), collisionZone, false);
                SendIgnoreCollisionDataToServer(player, false);
                ignoresCollisions[player.name] = false;
            }
        }

        // If player is at the left side of the enemy push it to the left
        if (playerPosition.x < transform.position.x)
        {
            attackForce.x *= -1;
        }

        playerController.TakeDamage(damage, attackForce);

    }

    public override void TakeDamage(float damage)
    {
        // Spider doesn't take any damage boy
    }

    private void SendIgnoreCollisionDataToServer(GameObject player, bool collision)
    {
        SendMessageToServer("IgnoreBoxCircleCollision/" + collision + "/" + player.name + "/" + gameObject.name);
    }

    void SendMessageToServer(string message)
    {
        if (Client.instance.GetLocalPlayer().controlOverEnemies)
        {
            Client.instance.SendMessageToServer(message);
        }
    }

}
