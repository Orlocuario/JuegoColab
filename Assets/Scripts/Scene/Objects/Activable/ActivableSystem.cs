using System;
using System.Collections;
using UnityEngine;

public class ActivableSystem : MonoBehaviour
{

    #region Attributes

    public Sprite activatedSprite;

    public float activationDistance = 1f;
    public int activationTime;
    public bool activated;

    [System.Serializable]
    public struct Component { public Sprite sprite; public bool placed; };
    public Component[] components;

    protected ActivableSystemActions systemActions;

    #endregion

    #region Common

    public virtual bool PlaceItem(Sprite item)
    {
        if (!activated)
        {
            int pos = ComponentPosition(item);

            if (pos != -1)
            {
                Component piece = components[pos];

                piece.placed = true;

                if (AllComponentsPlaced())
                {
                    activated = true;
                    StartCoroutine(Actioned());
                }

                return true;
            }

        }

        return false;
    }

    #endregion

    #region Utils

    protected int ComponentPosition(Sprite item)
    {

        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].sprite.Equals(item))
            {
                return i;
            }
        }

        return -1;

    }

    protected bool AllComponentsPlaced()
    {

        for (int i = 0; i < components.Length; i++)
        {
            if (!components[i].placed)
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Coroutines

    protected virtual IEnumerator Actioned()
    {
        if (!systemActions)
        {
            Debug.LogError("SystemActions not defined");
        }

        else
        {
            yield return new WaitForSeconds(activationTime);
            systemActions.DoSomething(this.gameObject);
        }
    }

    #endregion

}
