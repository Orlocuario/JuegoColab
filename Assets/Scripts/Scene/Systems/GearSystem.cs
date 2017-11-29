using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearSystem : MonoBehaviour
{

    #region Attributes

    public Inventory.Item[] requiredGears;
    public PlannerSwitch switchObj;
    public Sprite actionedSprite;

    public int activationTime;

    private float activationDistance = 1f;
    private bool actioned;

    #endregion

    #region Start

    private void Start()
    {
        HideInactiveGears();
    }

    #endregion

    #region Common

    // Call from outside
    public void PlaceGear(GameObject player, GameObject gear)
    {

        if (actioned)
        {
            return;
        }

        if (PlayerIsNear(player) && IsRequiredGear(gear))
        {

            PlaceGear(gear);
            Inventory.instance.RemoveItemFromInventory(gear);

            if (AllGearsPlaced())
            {
                actioned = true;
                StartCoroutine(Actioned());
            }

        }

    }

    // Call only from within
    protected void PlaceGear(GameObject gear)
    {
        Inventory.Item requiredGear = new Inventory.Item();

        // Find the gear first
        for (int i = 0; i < requiredGears.Length; i++)
        {
            if (requiredGears[i].item.Equals(gear))
            {
                requiredGear = requiredGears[i];
            }
        }

        if (requiredGear.item.GetComponent<SpriteRenderer>())
        {
            requiredGear.placed = true;
            requiredGear.item.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            Debug.LogError(gear + " does not have a SpriteRenderer");
        }
    }

    #endregion

    #region Utils

    // Hide every gear that was not "placed" from the editor
    protected void HideInactiveGears()
    {

        for (int i = 0; i < requiredGears.Length; i++)
        {
            if (!requiredGears[i].placed)
            {
                SpriteRenderer spriteRenderer = requiredGears[i].item.GetComponent<SpriteRenderer>();

                if (spriteRenderer)
                {
                    spriteRenderer.enabled = false;
                }
                else
                {
                    Debug.LogError(requiredGears[i] + " does not have a SpriteRenderer");
                }

            }
        }
    }

    protected bool IsRequiredGear(GameObject gear)
    {

        for (int i = 0; i < requiredGears.Length; i++)
        {
            if (requiredGears[i].item.Equals(gear))
            {
                return true;
            }
        }

        return false;

    }

    protected bool AllGearsPlaced()
    {

        for (int i = 0; i < requiredGears.Length; i++)
        {
            if (!requiredGears[i].placed)
            {
                return false;
            }
        }

        return true;
    }

    protected bool PlayerIsNear(GameObject player)
    {
        return Vector2.Distance(player.transform.position, transform.position) <= activationDistance;
    }

    #endregion


    #region Coroutines

    protected IEnumerator Actioned()
    {
        yield return new WaitForSeconds(activationTime);
        new GearSystemActions().DoSomething(this);
    }

    #endregion
}
