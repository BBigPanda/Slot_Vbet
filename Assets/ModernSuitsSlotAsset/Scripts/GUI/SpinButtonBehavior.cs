using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Mkey
{
    public class SpinButtonBehavior : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField]
        private Text autoText;
        [SerializeField]
        private SlotControls slotControls;

        private bool _canInput = false;
        private void ChangeInputState(bool isActive) {
            _canInput = isActive;
        }

        private void OnEnable() {
            StaticEvents.SceneInputState += ChangeInputState;
        }

        private void OnDisable() {
            StaticEvents.SceneInputState -= ChangeInputState;

        }

        #region events
        public Action ClickEvent;
        public Action <bool> TrySetAutoEvent;
        #endregion events

        #region temp vars
        private bool up = true;
        private float downTime = 0;
        #endregion temp vars

        #region regular
        private void Start()
        {
            if (slotControls) slotControls.ChangeAutoStateEvent += (auto) => { SetButtonText(); };
            SetButtonText();
        }
        #endregion regular

        private void Update() {
            if (_canInput) {
                if (Input.GetMouseButtonUp(0)) {
                    up = true;
                    if (!slotControls) return;
                    if (!slotControls.Auto) {
                        Debug.Log(gameObject.name + " Was up." +
                                  (slotControls ? " SpinType: auto - " + slotControls.Auto.ToString() : ""));
                        ClickEvent?.Invoke();
                    }
                }
                else if (Input.GetMouseButtonDown(0)) {
                    up = false;
                    if (!slotControls) return;
                    if (slotControls.Auto) {
                        TrySetAutoEvent?.Invoke(false);
                        return;
                    }

                    downTime = Time.time;
                }
                if (Input.GetKeyUp(KeyCode.R)) {
                    SceneManager.LoadScene(0);
                }
            }
           
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            up = false;
            if (!slotControls) return;
            if (slotControls.Auto)
            {
                TrySetAutoEvent?.Invoke(false);
                return;
            }
            downTime = Time.time;
            StartCoroutine(CheckAuto());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            up = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            up = true;
            if (!slotControls) return;
            if (!slotControls.Auto)
            {
                Debug.Log(gameObject.name + " Was up." + (slotControls ? " SpinType: auto - " + slotControls.Auto.ToString() : ""));
                ClickEvent?.Invoke();
            }
        }

        private IEnumerator CheckAuto()
        {
            bool cancel = false;
            WaitForEndOfFrame wef = new WaitForEndOfFrame();
            float dTime;
            while (!up && !cancel)
            {
                dTime = Time.time - downTime;
                if (dTime > 2.0f)
                {
                    TrySetAutoEvent?.Invoke(true);
                    cancel = true;
                }
                yield return wef;
            }
        }

        private void SetButtonText()
        {
            if (autoText && slotControls)
            {
                autoText.text = (!slotControls.Auto) ? "Hold to AutoSpin" : "AUTO";
            }
            else if (autoText)
            {
                autoText.text = "";
            }
        }
    }
}