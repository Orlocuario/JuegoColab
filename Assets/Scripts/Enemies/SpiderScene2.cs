﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderScene2 : MonoBehaviour {

    LevelManager levelManagerScript;
    Vector3 posInicial;
    Vector3 auxPosition;
    private static float minimunDistance = 3.8f;

	void Start ()
    {
        levelManagerScript = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        posInicial = new Vector3(73.38f, 0.73f);
        Physics2D.IgnoreCollision(GameObject.Find("RocaGiganteAraña").GetComponents<CircleCollider2D>()[0], this.gameObject.GetComponent<CircleCollider2D>());
        Physics2D.IgnoreCollision(GameObject.Find("RocaGiganteAraña").GetComponents<CircleCollider2D>()[1], this.gameObject.GetComponent<CircleCollider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        auxPosition = this.gameObject.transform.position;

        Transform player0Transform = levelManagerScript.players[0].GetComponent<Transform>();
        Transform player1Transform = levelManagerScript.players[1].GetComponent<Transform>();
        Transform player2Transform = levelManagerScript.players[2].GetComponent<Transform>();

        if (Mathf.Abs(transform.position.x - player0Transform.position.x) < minimunDistance || 
            Mathf.Abs(transform.position.x - player1Transform.position.x) < minimunDistance ||
            Mathf.Abs(transform.position.x - player2Transform.position.x) < minimunDistance)
        {
            if (Mathf.Abs(player0Transform.position.x - posInicial.x) < minimunDistance)
            {
                GameObject[] players = levelManagerScript.players;
                foreach (GameObject player in players)
                {
                    if (Client.instance.GetMage().InShield(player))
                    {
                        Client.instance.SendMessageToServer("IgnoreBoxCircleCollision/true/" + player.name + "/" + this.gameObject.name);
                    }
                    else
                    {
                        Client.instance.SendMessageToServer("IgnoreBoxCircleCollision/false/" + player.name + "/" + this.gameObject.name);
                        Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), this.gameObject.GetComponent<CircleCollider2D>(), false);
                    }
                }
            }

            if ((Mathf.Abs(player0Transform.position.x - posInicial.x) < minimunDistance && player0Transform.position.y <= 0f) ||
                (Mathf.Abs(player1Transform.position.x - posInicial.x) < minimunDistance && player1Transform.position.y <= 0f) ||
                (Mathf.Abs(player2Transform.position.x - posInicial.x) < minimunDistance && player2Transform.position.y <= 0f))  // La araña está arriba y alguien aparece abajo y/o arriba.
            {
                this.gameObject.transform.position = new Vector3(posInicial.x, -3.14f);  // Baja la araña
            }

            else if (transform.position.y != posInicial.y)  // La araña está abajo, no hay nadie abajo
            {
               this.gameObject.transform.position = posInicial;  // La araña sube
            }
        }

        else  // No hay nadie cerca
        {
            this.gameObject.transform.position = posInicial;  // La araña sube
        }

        if (auxPosition != transform.position)
        {
            Client.instance.SendMessageToServer("ChangeObjectPosition/" + this.gameObject.name + "/" +
                this.gameObject.transform.position.x.ToString() + "/" + this.gameObject.transform.position.y.ToString() + "/" + this.gameObject.transform.rotation.z.ToString());
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
            Client.instance.SendMessageToServer("ChangeHpHUDToRoom/-5");
        }
    }
}
