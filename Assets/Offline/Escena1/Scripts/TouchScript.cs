using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchScript : MonoBehaviour {

    public PlayerController script;
    public GameObject botonSaltar;
    public GameObject botonIzquierda;
    public GameObject botonDerecha;
    public int touchSaltar = -1;
    public int touchIzquierda = -1;
    public int touchDerecha = -1;
    enum Botones { Saltar,Izquierda,Derecha}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        int touchCount = Input.touchCount;
        for(int i=0; i<touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            int fingerId = touch.fingerId;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    CheckTouch(Botones.Derecha, fingerId);
                    CheckTouch(Botones.Izquierda, fingerId);
                    CheckTouch(Botones.Saltar, fingerId);
                    break;
                case TouchPhase.Ended:

                    if (touchDerecha == fingerId) { 
                        touchDerecha = -1; ReleaseRight();}
                    if (touchIzquierda == fingerId)
                    {
                        touchIzquierda = -1; ReleaseLeft();
                    }
                    if(touchSaltar == fingerId)
                    {
                        touchSaltar = -1; ReleaseJump();
                    }
                    break;
                default:
                    break;
            }
        }
	}

    private bool CheckIntersect(GameObject boton, int fingerId)
    {
        int touchCount = Input.touchCount;
        int touchId = -1;
        for(int i=0; i<touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if(touch.fingerId == fingerId)
            {
                touchId = i;
            }
        }
        Input.GetTouch(touchId);
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.GetTouch(touchId).position;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);
        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult raycasted in raycastResults)
            {
                if (raycasted.gameObject.name == boton.name)
                {
                    return true;
                }
            }

        }
        return false;
    }

    private void CheckTouch(Botones boton, int fingerId)
    {
        if (boton == Botones.Derecha && touchDerecha == -1)
        {
            bool intersecta = CheckIntersect(botonDerecha, fingerId);
            if (intersecta)
            {
                touchDerecha = fingerId;
                PressRight();
            }
        }
        else if(boton == Botones.Izquierda && touchIzquierda == -1)
        {
            {
                bool intersecta = CheckIntersect(botonIzquierda, fingerId);
                if (intersecta)
                {
                    touchIzquierda = fingerId;
                    PressLeft();
                }
            }
        }
        else if (boton == Botones.Saltar && touchSaltar == -1)
        {
            {
                bool intersecta = CheckIntersect(botonSaltar, fingerId);
                if (intersecta)
                {
                    touchSaltar = fingerId;
                    PressJump();
                }
            }
        }
    }

    public void PressJump()
    {
        script.jumpPressed = true;
    }

    public void ReleaseJump()
    {
        script.jumpPressed = false;
    }

    public void PressRight()
    {
        script.rightPressed = true;
    }

    public void ReleaseRight()
    {
        script.rightPressed = false;
    }

    public void PressLeft()
    {
        script.leftPressed = true;
    }

    public void ReleaseLeft()
    {
        script.leftPressed = false;
    }
}
