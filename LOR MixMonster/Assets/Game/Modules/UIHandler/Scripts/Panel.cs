using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI
{
    public abstract class Panel : UnityEngine.MonoBehaviour
    {
        //public List<Transform> listScaleTrans;
        protected Animator ani;
        protected Transform _transform;
        public bool overrideBack = false,isPersistant=false;
        public virtual void Clear()
        {
            OnDestroy();
        }
        public virtual void OnDestroy()
        {
            
        }
        public void Destroy()
        {
            Addressables.ReleaseInstance(gameObject);
        }
        public virtual void Show()
        {
            Active();
        }
        public void ClearCloseTrigger()
        {
            ani.ResetTrigger("Close");
        }
        public virtual void Hide()
        {
            if (ani == null)
            {
                ani = GetComponent<Animator>();
            }

            if (ani != null)
            {
                ani.SetTrigger("Close");
            }
            else
            {
                PanelFadeAnimation panelFadeAnimation = GetComponent<PanelFadeAnimation>();
                if(panelFadeAnimation!=null)
                    panelFadeAnimation.Close(() => Deactive());
                else
                    Deactive();
            }
        }
        public virtual void Deactive()
        {
            gameObject.SetActive(false);
            UI.PanelManager.Instance.OnPanelHidden(this);
            PanelManager.Instance.DeRegister(this);


            Destroy();

        }
        public virtual void Active()
        {
            PanelFadeAnimation panelFadeAnimation = GetComponent<PanelFadeAnimation>();
            if (panelFadeAnimation != null)
            {
                panelFadeAnimation.Show();
            }
            else
            {
                gameObject.SetActive(true);
            }
            UI.PanelManager.Instance.OnPanelShown(this);
            PanelManager.Instance.Register(this);
        }

        public virtual void ShowAfterAd() { }
        public virtual void Close()
        {
            Hide();
        }
        public abstract void PostInit();

        public virtual void OnBack()
        {
            if (overrideBack) return;
            Close();
        }
        public void PlaySFX(AnimationEvent animationEvent)
        {
            Sound.Controller.Instance.PlayOneShot((AudioClip)animationEvent.objectReferenceParameter);
        }
    }
}