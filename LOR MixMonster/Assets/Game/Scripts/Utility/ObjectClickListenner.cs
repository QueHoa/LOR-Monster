using UnityEngine;


namespace GameUtility
{
    public class ObjectClickListenner:MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Events.UnityEvent onClick,onMouseDown,onMouseUp,onDrag;
        float time = 0;
        private void OnMouseDown()
        {
            time = Time.time;
            onMouseDown?.Invoke();
        }
        private void OnMouseDrag()
        {
            onDrag?.Invoke();
        }
        private void OnMouseUp()
        {
            onMouseUp?.Invoke();
            if (Time.time - time < 0.2f)
            {
                onClick?.Invoke();
            }

        }
    }
}