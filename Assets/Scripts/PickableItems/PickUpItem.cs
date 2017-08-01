using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PickUpItem : MonoBehaviour {

    public GameObject PickUpButton;
    LevelManager levelManager;
    Vector2 myPosition;
    private bool lockValue;
    private bool buttonPressed; 
    private int touchIdPressed;

    void Start()
    {
        lockValue = false;
        myPosition = gameObject.transform.position;
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        PickUpButton.SetActive(false);
        buttonPressed = false;
        touchIdPressed = -1;
    }

    void Update ()
    {
        PlayerController player = levelManager.thePlayer;
        Vector2 playerPosition = player.gameObject.transform.position;
        float distance = (playerPosition - myPosition).magnitude;

        if (distance < 0.4f)
        {
            lockValue = true;
            PickUpButton.SetActive(true);
        }

        else
        {
            PickUpButton.SetActive(false);
            if (lockValue)
            {
                lockValue = false;
            }
        }

        ButtonUpdate();
    }

    public void PickUp()
    {
        Inventory.instance.AddItemToInventory(this.gameObject);
        Destroy(this.gameObject);
    }

    public void Drop(GameObject parent)
    { 

    }

    private void ButtonUpdate()
    {
        int touches = Input.touchCount;
        if (touches > 0)
        {
            for (int i = 0; i < touches; i++)
            {
                Touch touch = Input.GetTouch(i);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        PressButton(i);
                        break;
                    case TouchPhase.Ended:
                        ReleaseButton(i);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void PressButton(int touchId)
    {
        if (!buttonPressed && CheckIfPressed(touchId))
        {
            buttonPressed = true;
            PickUp();
            this.touchIdPressed = touchId;
        } 
    }
 
    private bool CheckIfPressed(int touchId)
    {
        Input.GetTouch(touchId);
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.GetTouch(touchId).position;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0) 
        { 
            foreach(RaycastResult raycasted in raycastResults) 
            { 
                if(raycasted.gameObject.tag == "PickUpButton") 
                { 
                    return true; 
                } 
            } 
        } 
        return false; 
    }

    private void ReleaseButton(int touchId)
    { 
        if((touchIdPressed == touchId) && buttonPressed) 
        { 
            this.buttonPressed = false;
            touchIdPressed = -1; 
        } 
    } 
}
