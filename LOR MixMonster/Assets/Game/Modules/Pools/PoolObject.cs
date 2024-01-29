using UnityEngine;

namespace Game.Pool
{
    public class PoolObject : MonoBehaviour
    {
        public delegate void OnReleased(PoolObject objectBase);
        public OnReleased onReleased;
        private bool isAvailable;
        public bool IsAvailable
        {
            get { return isAvailable; }
            set
            {
                isAvailable = value;

            }
        }

        protected void OnDisable()
        {
            IsAvailable = true;
            onReleased?.Invoke(this);
        }
    }
}