using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPointer : MonoBehaviour
{
    RectTransform _transform,parentRect;
    Camera cam;
    Vector2 position;
    [SerializeField]
    private ParticleSystem ps;
    private void Start()
    {
        _transform = GetComponent<RectTransform>();
        parentRect = _transform.parent.GetComponent<RectTransform>();
        cam = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if(Sound.Controller.Instance!=null)
                Sound.Controller.Instance.PlayClickSFX();
                ps.Play();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, touch.position, cam, out position);
                _transform.anchoredPosition = position;
            }
        }    
    }
}
