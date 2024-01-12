using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIHandler
{
    public class ImageButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        RectTransform _transform;
        [SerializeField]
        private float xRatio = 1, yRatio = 1;
        Vector2 defaultScale;
        [SerializeField]
        private UnityEngine.Events.UnityEvent onClick, onStartClick, onClickFailed, onReleased, onEntered;
        bool canInteract = true;
        [SerializeField]
        private AnimationCurve highLightCurve;

        StateHandler[] uiStateHandlers;
        public void SetInteract(bool active)
        {
            if (_transform == null)
            {
                _transform = GetComponent<RectTransform>();
                uiStateHandlers = GetComponentsInChildren<StateHandler>();
            }
            canInteract = active;
            foreach (StateHandler ui in uiStateHandlers)
            {
                ui.SetState(active ? StateHandler.StatusState.Current : StateHandler.StatusState.Lock);
            }
        }
        CancellationTokenSource token;
        private void Start()
        {
            if (_transform == null)
            {
                _transform = GetComponent<RectTransform>();
                uiStateHandlers = GetComponentsInChildren<StateHandler>();
            }
            defaultScale = _transform.sizeDelta;
        }

        void OnDisable()
        {
            if (token == null) return;
            token.Cancel();
            isClick = false;
            isEnter = false;
        }
        void OnDestroy()
        {
            if (token == null) return;
            token.Cancel();
            token.Dispose();
        }

        public void AddListenner(UnityEngine.Events.UnityAction call)
        {
            onClick.AddListener(call);
        }
        public void RemoveListenner(UnityEngine.Events.UnityAction call)
        {
            onClick.RemoveListener(call);
        }
        void OnEnable()
        {
            token = new CancellationTokenSource();
        }
        Vector3 scale=Vector3.one;
        void Update()
        {
            if(highLightCurve.length>1 && !isEnter && !isClick)
            {
                scale.x = scale.y = highLightCurve.Evaluate(Time.time % highLightCurve.keys[highLightCurve.length-1].time);
                _transform.localScale = scale;
            }
        }
        bool isEnter, isClick;
        public void OnPointerExit(PointerEventData eventData)
        {
            if (isClick || !canInteract) return;
            isClick = false;
            isEnter = false;
            onReleased?.Invoke();
            DoRelease(token).Forget();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isClick || !canInteract) return;

            isClick = false;
            isEnter = true;
            onEntered?.Invoke();
            DoHold(token).Forget();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (!canInteract)
            {
                onClickFailed?.Invoke();
                return;
            }
            isEnter = false;
            isClick = true;
            DoClick(token).Forget();
        }


        async UniTaskVoid DoClick(CancellationTokenSource token)
        {
            onStartClick?.Invoke();
            float a = 0;
            Vector2 scale = defaultScale;
            while (a < Mathf.PI * 2)
            {
                if (_transform == null)
                {
                    break;
                }
                float sin = Mathf.Sin(a);
                float ratioX = 1 + xRatio * sin / 40f;
                float ratioY = 1 + yRatio * sin / 40f;

                scale.x = defaultScale.x * ratioX;
                scale.y = defaultScale.y * ratioY;
                _transform.sizeDelta = scale;
                a += Mathf.PI / 5;
                if (!isClick) break;
                await UniTask.Yield(token.Token);
            }
            _transform.sizeDelta = defaultScale;
            isClick = false;
            onClick?.Invoke();
        }
        async UniTaskVoid DoHold(CancellationTokenSource token)
        {
            float a = 0;
            Vector3 scale = defaultScale;
            while (a < Mathf.PI / 2)
            {
                if (_transform == null)
                {
                    break;
                }
                float sin = Mathf.Sin(a);
                float ratioX = 1 - xRatio * sin / 20f;
                float ratioY = 1 - yRatio * sin / 20f;
                scale.x = defaultScale.x * ratioX;
                scale.y = defaultScale.y * ratioY;
                _transform.sizeDelta = scale;
                a += Mathf.PI / 15;
                if (!isEnter) break;
                await UniTask.Yield(token.Token);
            }
        }
        async UniTaskVoid DoRelease(CancellationTokenSource token)
        {
            float a = 0;
            Vector3 scale = defaultScale * 0.9f;
            while (a < Mathf.PI / 2)
            {
                if (_transform == null)
                {
                    break;
                }
                float sin = Mathf.Sin(a);
                float ratioX = 0.95f + xRatio * sin / 20f;
                float ratioY = 0.95f + yRatio * sin / 20f;


                scale.x = defaultScale.x * ratioX;
                scale.y = defaultScale.y * ratioY;
                _transform.sizeDelta = scale;
                a += Mathf.PI / 15;
                if (isEnter) break;
                await UniTask.Yield(token.Token);
            }
        }

    }
}