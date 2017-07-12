namespace CnControls
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    /// <summary>
    /// Simple button class
    /// Handles press, hold and release, just like a normal button
    /// </summary>
    public class JumpButton : MonoBehaviour
    //IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        /// The name of the button
        /// </summary>
        /// 
        private bool buttonPressed;
        private int touchIdPressed;

        public string ButtonName = "Jump Button";

        public void Start()
        {
            buttonPressed = false;
            touchIdPressed = -1;
        }

        public void Update()
        {
            int touches = Input.touchCount;
            if (touches > 0)
            {
                for(int i=0; i<touches; i++)
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
                _virtualButton.Press();
                buttonPressed = true;
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
                    if(raycasted.gameObject.tag == "JumpButton")
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
                _virtualButton.Release();
                touchIdPressed = -1;
            }
        }
        /// <summary>
        /// Utility object that is registered in the system
        /// </summary>
        private VirtualButton _virtualButton;

        /// <summary>
        /// It's pretty simple here
        /// When we enable, we register our button in the input system
        /// </summary>
        private void OnEnable()
        {
            _virtualButton = _virtualButton ?? new VirtualButton(ButtonName);
            CnInputManager.RegisterVirtualButton(_virtualButton);
        }

        /// <summary>
        /// When we disable, we unregister our button
        /// </summary>
        private void OnDisable()
        {
            CnInputManager.UnregisterVirtualButton(_virtualButton);
        }

        /// <summary>
        /// uGUI Event system stuff
        /// It's also utilised by the editor input helper
        /// </summary>
        /// <param name="eventData">Data of the passed event</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            _virtualButton.Release();
        }

        /// <summary>
        /// uGUI Event system stuff
        /// It's also utilised by the editor input helper
        /// </summary>
        /// <param name="eventData">Data of the passed event</param>
        /// 
       /* public void OnPointerDown(PointerEventData eventData)
        {
            _virtualButton.Press();
        }*/
    }
}