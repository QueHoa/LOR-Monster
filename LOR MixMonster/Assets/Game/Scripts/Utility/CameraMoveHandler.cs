using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveHandler : MonoBehaviour
{
    [SerializeField]
    private Transform focusPoint;
    [SerializeField]
    private float sensitivity = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
  
    Vector3 touchStartPos,defaultPos;
    bool isDown = false;
    public void OnTouchDown()
    {
        if (UI.PanelManager.Instance.IsAnythingOpenned()) return;
        isDown = true;
        touchStartPos = Input.mousePosition;
        defaultPos = focusPoint.localPosition;
    }
    public void OnTouchDrag()
    {
        if(isDown)
        {
            Vector3 delta = (Input.mousePosition - touchStartPos);
            delta.x = delta.x / Screen.width;
            delta.y = delta.y / Screen.height;
            focusPoint.localPosition = defaultPos - delta * sensitivity;
        }
    }
    public void OnTouchRelease()
    {
        isDown = false;

    }
}
