using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UI
{
    public class PanelManager : UnityEngine.MonoBehaviour
    {
        public delegate void OnPanelShow(Panel panel);
        public static OnPanelShow onPanelShow;
        public static PanelManager Instance;
        [SerializeField]
        private List<Panel> panels = new List<Panel>();
        private Stack<Panel> stack = new Stack<Panel>();

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }
        private void OnDestroy()
        {
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].Clear();
                panels[i].OnDestroy();
                panels[i] = null;
            }
        }
        public static void Create(Type type,System.Action<Panel,AsyncOperationHandle> onLoaded)
        {
            Create(type.ToString(), onLoaded);
        }
        public static void Create(string type, System.Action<Panel, AsyncOperationHandle> onLoaded)
        {
            Addressables.InstantiateAsync(type, Instance.transform).Completed += op =>
            {
                Panel panel = op.Result.GetComponent<Panel>();
                panel.PostInit();
                onLoaded?.Invoke(panel, op);
                onPanelShow?.Invoke(panel);
            };
        }
        public static async UniTask<Panel> CreateAsync(string type)
        {
            AsyncOperationHandle<GameObject> op =  Addressables.InstantiateAsync(type, PanelManager.Instance.transform);
            await op;
            Panel panel = op.Result.GetComponent<Panel>();
            panel.PostInit();
            return panel;
        }
        public static async UniTask<Panel> CreateAsync(Type type)
        {
            AsyncOperationHandle<GameObject> op = Addressables.InstantiateAsync(type.ToString(), PanelManager.Instance.transform);
            await op;
            Panel panel = op.Result.GetComponent<Panel>();
            panel.PostInit();
            return panel;
        }
        public void Register(Panel panel)
        {
            if (panels.Contains(panel)) return;
            panels.Add(panel);
        }
        public void DeRegister(Panel panel)
        {
            if (!panels.Contains(panel)) return;
            panels.Remove(panel);
        }
        public bool IsAnythingOpenned()
        {
            foreach(Panel panel in panels)
            {
                if (!panel.isPersistant && panel.gameObject.activeSelf)
                {
                    return true;
                }
            }
            return false;
        }
        public void Init()
        {
            Instance = this;
            Transform holder = transform;
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].PostInit();
            }
            for (int i = 0; i < holder.childCount; i++)
            {
                Panel panel = holder.GetChild(i).GetComponent<Panel>();
                if (panel == null) continue;
                try
                {
                    panel.PostInit();
                    panels.Add(panel);
                }
                catch (System.Exception e) { GameUtility.GameUtility.LogError(panel.gameObject.name + " \n" + e); }
            }
        }
        public bool canBack = true;
//#if UNITY_ANDROID || UNITY_EDITOR
//        private void Update()
//        {
//            if (canBack &&Input.GetKeyDown(KeyCode.Escape) &&stack!=null && stack.Count > 0 )
//            {
//                while (stack.Count>0 &&(stack.Peek()==null ||!stack.Peek().gameObject.activeSelf ) && (stack.Peek()==null||( stack.Peek()!=null&&!stack.Peek().isPersistant)))
//                {
//                    stack.Pop();
//                }
//                if (stack.Count > 0 && stack.Peek()!=null)
//                {
//                    //GameUtility.GameUtility.Log("back :" + stack.Peek().gameObject.name);
//                    if (stack.Peek().isPersistant)
//                    {
//                        stack.Peek().OnBack();
//                    }
//                    else
//                    {
//                        stack.Pop().OnBack();
//                    }
//                }
//            }
//        }
//#endif
      
        public void OnPanelShown(UI.Panel panel)
        {
#if UNITY_ANDROID || UNITY_EDITOR
            if (!stack.Contains(panel))
            {
                //GameUtility.GameUtility.Log("Show :" + panel.gameObject.name);
                stack.Push(panel);
            }
#endif
        }
        public void OnPanelHidden(UI.Panel panel)
        {
#if UNITY_ANDROID || UNITY_EDITOR
            if (stack.Count>0 &&stack.Peek() == panel)
            {
                GameUtility.GameUtility.Log("HIDE :" + panel.gameObject.name);
                stack.Pop();
            }
#endif
        }
        public Panel GetPanel(int index)
        {
            return panels[index];
        }

        public void SetBack(bool v)
        {
            canBack = v;
        }
    }
}