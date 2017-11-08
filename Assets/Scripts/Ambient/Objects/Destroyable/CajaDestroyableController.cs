using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CajaDestroyableController : DestroyableController
{
    public Sprite brokenBox;

    // Use this for initialization
    protected override void Start()
    {
    }

    public override void DestroyMe()
    {

        Collider2D collider = GetComponent<Collider2D>();
        Destroy(collider);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = brokenBox;

        Destroy(this.gameObject, 2f);

    }

}