using System.Collections;
using UnityEngine;

public class RuneSystem : MonoBehaviour
{

    #region Attributes

    public struct Rune { public GameObject item; public bool placed; };

    public PlannerObstacle obstacleObj = null;
    public Sprite activatedSprite;
    public Rune[] requiredRunes;

    public int activationTime;

    private float activationDistance = 1f;
    private bool activated;

    #endregion

    #region Start

    private void Start()
    {
        HideInactiveRunes();
    }

    #endregion

    #region Common

    // Call from outside
    public void PlaceGear(GameObject runeGO)
    {

        if (activated)
        {
            return;
        }

        int pos = RunePosition(runeGO);

        if (pos != -1)
        {
            Rune rune = requiredRunes[pos];

            PlaceRune(rune);

            if (AllRunesPlaced())
            {
                activated = true;
                StartCoroutine(Actioned());
            }

        }

    }

    // Call only from within
    protected void PlaceRune(Rune rune)
    {

        if (rune.item.GetComponent<SpriteRenderer>())
        {
            rune.placed = true;
            rune.item.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            Debug.LogError(rune + " does not have a SpriteRenderer");
        }

    }

    #endregion

    #region Utils

    // Hide every rune that was not "placed" from the editor
    protected void HideInactiveRunes()
    {
        if (requiredRunes == null)
        {
            Debug.Log(name + " has no required runes to work ?");
            return;
        }

        for (int i = 0; i < requiredRunes.Length; i++)
        {
            if (!requiredRunes[i].placed)
            {
                SpriteRenderer spriteRenderer = requiredRunes[i].item.GetComponent<SpriteRenderer>();

                if (spriteRenderer)
                {
                    spriteRenderer.enabled = false;
                }
                else
                {
                    Debug.LogError(requiredRunes[i] + " does not have a SpriteRenderer");
                }

            }
        }
    }

    protected int RunePosition(GameObject rune)
    {
        for (int i = 0; i < requiredRunes.Length; i++)
        {
            if (requiredRunes[i].item.Equals(rune))
            {
                return i;
            }
        }

        return -1;
    }

    protected bool AllRunesPlaced()
    {
        for (int i = 0; i < requiredRunes.Length; i++)
        {
            if (!requiredRunes[i].placed)
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
        new RuneSystemActions().DoSomething(this);
    }

    #endregion

}
