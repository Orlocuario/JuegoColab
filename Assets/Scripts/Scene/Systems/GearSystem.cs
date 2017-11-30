using System.Collections;
using UnityEngine;

public class GearSystem : ActivableSystem
{

    #region Attributes

    public struct Gear { public GameObject item; public bool placed; };

    public PlannerSwitch switchObj;
    public Gear[] requiredGears;

    #endregion

    #region Start

    protected void Start()
    {
        HideInactiveGears();
    }

    #endregion

    #region Common

    public override bool PlaceItem(GameObject item)
    {
        return PlaceGear(item);
    }

    // Call from outside
    public bool PlaceGear(GameObject gearGO)
    {

        if (!activated)
        {

            int pos = GearPosition(gearGO);

            if (pos != -1)
            {
                Gear gear = requiredGears[pos];

                PlaceGear(gear);

                if (AllGearsPlaced())
                {
                    activated = true;
                    StartCoroutine(Actioned());
                }

                return true;
            }

        }

        return false;
    }

    // Call only from within
    protected void PlaceGear(Gear gear)
    {

        if (gear.item.GetComponent<SpriteRenderer>())
        {
            gear.placed = true;
            gear.item.GetComponent<SpriteRenderer>().enabled = true;
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
        if (requiredGears == null)
        {
            Debug.Log(name + " has no required gears to work ?");
            return;
        }

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

    protected int GearPosition(GameObject gear)
    {

        for (int i = 0; i < requiredGears.Length; i++)
        {
            if (requiredGears[i].item.Equals(gear))
            {
                return i;
            }
        }

        return -1;

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

    #endregion

    #region Coroutines

    protected IEnumerator Actioned()
    {
        yield return new WaitForSeconds(activationTime);
        new GearSystemActions().DoSomething(this);
    }

    #endregion

}
