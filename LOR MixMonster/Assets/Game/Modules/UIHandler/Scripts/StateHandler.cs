using UnityEngine;

namespace UIHandler
{
    public abstract class StateHandler : UnityEngine.MonoBehaviour
    {
        public enum StatusState
        {
            Lock, Unlock, Current,Special
        }
        public abstract void SetState(StatusState status);
        public virtual void SetColor(Color c) { }
        public virtual Color GetColor() { return Color.white; }
    }
}