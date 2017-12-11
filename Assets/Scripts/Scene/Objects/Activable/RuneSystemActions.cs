using UnityEngine;


public class RuneSystemActions : ActivableSystemActions
{

    #region Common

    public void DoSomething(RuneSystem runeSystem, bool notifyOthers)
    {

        // Hide every placed rune
        SpriteRenderer[] componentSlots = runeSystem.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < componentSlots.Length; i++)
        {
            componentSlots[i].sprite = null;
        }

        // Dispose every used rune in case of reconnection
        for (int i = 0; i < runeSystem.components.Length; i++)
        {
            string usedRuneName = runeSystem.components[i].sprite.name;
            GameObject usedRune = GameObject.Find(usedRuneName);

            if (usedRune)
            {
                DestroyObject(usedRuneName, .1f);
            }

        }

        // Change the door sprite
        SpriteRenderer systemSpriteRenderer = runeSystem.GetComponent<SpriteRenderer>();
        systemSpriteRenderer.sprite = runeSystem.activatedSprite;

        // Allow players to pass through the door
        Collider2D collider = runeSystem.GetComponent<Collider2D>();
        collider.enabled = false;

        if (runeSystem.obstacleObj != null)
        {
            runeSystem.obstacleObj.OpenDoor();

            Planner planner = GameObject.FindObjectOfType<Planner>();
            planner.Monitor();

        }

        if (notifyOthers)
        {
            SendMessageToServer("ActivateSystem/" + runeSystem.name, true);
        }

    }

    #endregion

}
