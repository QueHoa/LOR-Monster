using UnityEngine;

namespace Effect
{
    public abstract class EffectAbstract : MonoBehaviour
    {
        public bool isUsing = false;
        protected Transform t, parent, followedTarget;
        Vector3 defaultScale;

        protected void Init()
        {
            CancelInvoke();
            if (t == null)
            {
                t = transform;
                parent = t.parent;
                defaultScale = t.localScale;

            }
            followedTarget = null;
            ClearParent();
        }
        Vector3 offset;
        public virtual EffectAbstract SetParent(Transform parent)
        {
            Init();
            offset = transform.position - parent.position;
            followedTarget = parent;
            return this;
        }
        private void Update()
        {
            if (followedTarget != null)
            {
                t.localPosition = followedTarget.position+offset;
            }
        }
        public void ClearParent()
        {
            if (followedTarget != null)
            {
                followedTarget = null;
                //t.parent = parent;
                t.localScale = defaultScale;
            }
        }
        protected virtual void OnDisable()
        {
            ClearParent();
            //Invoke(nameof(ClearParent), 0.1f);
        }
        public virtual void Active() { }
        public virtual void Active(Transform parent) { }
        //public virtual void Active(Transform parent, Position p) {} 
        public virtual EffectAbstract Active(SpriteRenderer sr) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, float size) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, Color color) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, Sprite gunLeft,Sprite gunRight) { return this; }
        public virtual EffectAbstract Active(MeshRenderer renderer) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, Vector2 size) { return this; }
        public virtual EffectAbstract Active(Vector3 pos) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, Vector3 direction) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, int amount) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, int amount, bool isCritical) { return this; }
        public virtual EffectAbstract SetColor(Color color) { return this; }
        public virtual EffectAbstract Active(Vector3 pos, string text) { return this; }
        public virtual void Active(Vector3 pos, System.Action callback) { }
        public virtual void Active(int value, Vector3 pos, System.Action callback) { }
        public virtual void Active(Vector3[] pos, System.Action<int>[] callback) { }
        public virtual void Active(System.Action callback) { }
        public abstract bool IsUsing();
        public virtual void Deactive()
        {
            gameObject.SetActive(false);
            isUsing = false;
        }
        public void SetActive()
        {
            gameObject.SetActive(true);
        }
    }
}