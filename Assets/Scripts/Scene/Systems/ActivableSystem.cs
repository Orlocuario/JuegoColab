using System.Collections;
using UnityEngine;

public class ActivableSystem : MonoBehaviour
{

    #region Attributes

    public Sprite activatedSprite;

    public float activationDistance = 1f;
    public int activationTime;
    public bool activated;

    #endregion

    #region Common

    public virtual bool PlaceItem(GameObject item)
    {
        throw new System.Exception("Every system must implement the PlaceItem method");
    }

    #endregion

}
