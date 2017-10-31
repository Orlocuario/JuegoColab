using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : EnemyController
{
    private Dictionary<string, bool> ignoresCollisions; // Determina la colisión con la araña para cada player

    private Vector3 bottomTunnelPosition;
    private Vector3 upperTunnelPosition;
    private Vector3 lastPosition;

    public CircleCollider2D collisionZone;

    private static float alertDistance = 3.8f;
    private static float movingSteps = .6f;

    private int positionUpdateFrame = 0; // Contador de frames para actualizar la posición de la araña

    protected override void Start()
    {
        force = new Vector2(3500f, 150f);
        maxHp = 2000f;
        damage = 5;

        protecting = false;

        ignoresCollisions = new Dictionary<string, bool> { { "Mage", false }, { "Warrior", false }, { "Engineer", false } };

        upperTunnelPosition = new Vector3(73.38f, 0.73f);
        bottomTunnelPosition = new Vector3(upperTunnelPosition.x, -3.14f);

        IgnoreCollisionsWithRock();

        base.Start();
    }

    protected void IgnoreCollisionsWithRock()
    {
        GameObject spiderRock = GameObject.Find("RocaGiganteAraña");

        foreach (CircleCollider2D rockCollider in spiderRock.GetComponents<CircleCollider2D>())
        {
            Physics2D.IgnoreCollision(rockCollider, gameObject.GetComponent<CircleCollider2D>());
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        lastPosition = transform.position;

        ProtectTunnel();
        UpdatePosition();
    }

    protected void UpdatePosition()
    {
        if (lastPosition != transform.position)
        {
            SendPositionToServer();
        }
    }

    protected void ProtectTunnel()
    {

        if (levelManager.players == null || levelManager.players.Length == 0)
        {
            return;
        }

        GameObject player = null;

        bool playerIsInBottomTunnel = false;

        foreach (GameObject _player in levelManager.players)
        {
            if (Mathf.Abs(transform.position.x - _player.transform.position.x) < alertDistance)
            {
                if (_player.transform.position.y <= 0f) // Is in the bottom tunnel
                {
                    playerIsInBottomTunnel = true;
                    player = _player;
                    break;
                }
            }
        }

        if (playerIsInBottomTunnel)
        {
            if (transform.position.y != bottomTunnelPosition.y)
            {
                transform.position = Vector3.MoveTowards(transform.position, bottomTunnelPosition, movingSteps);
            }
        }
        else
        {
            if (transform.position.y != upperTunnelPosition.y)
            {
                transform.position = Vector3.MoveTowards(transform.position, upperTunnelPosition, movingSteps);
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

    protected override void SendMessageToServer(string message)
    {
        if (Client.instance.GetLocalPlayer().controlOverEnemies)
        {
            Client.instance.SendMessageToServer(message);
        }
    }

}
