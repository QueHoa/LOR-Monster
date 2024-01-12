using GameUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTouchHandler : MonoBehaviour
{
    //[SerializeField]
    //private AudioClip pickSFX, releaseSFX;
    private void OnEnable()
    {
        var bc = gameObject.AddComponent<BoxCollider2D>();
        bc.offset = new Vector2(0, 6.2f);
        bc.size = new Vector2(12, 33);
    }
    private void OnDisable()
    {

    }
    bool isDown = false, isSelected;
    Vector3 offset;
    Vector3 currentPosition;
    float touchTime;
    private void OnMouseDown()
    {
        if (!enabled) return;

        isDown = true;
        touchTime = Time.time;
        transform.Shake(0.1f, 1, 0.05f, defaultScale: transform.localScale.x);

    }
    private void Update()
    {
        if (!isSelected && isDown && Time.time - touchTime > 0.25f)
        {
            isSelected = true;
            currentPosition = transform.position;
            offset = CameraController.Instance.GetTouchPosition() - transform.position;
            //Sound.Controller.Instance.PlayOneShot(pickSFX);
            transform.Shake(0.15f, 1, 0.1f, defaultScale: transform.localScale.x);
        }
    }
    private void OnMouseUp()
    {
        if (!isSelected) return;
        isDown = false;
        isSelected = false;
        if (!IsModelOnStage())
        {
            transform.position = currentPosition;
            transform.Shake(0.15f, 1, 0.2f, defaultScale: transform.localScale.x);
        }
        DataManagement.DataManager.Instance.Save();

        //Sound.Controller.Instance.PlayOneShot(releaseSFX);

    }
    private bool IsModelOnStage()
    {
        return false;
    }
    private void OnMouseDrag()
    {
        if (isSelected && isDown)
        {
            transform.position = CameraController.Instance.GetTouchPosition() - offset;
        }
    }
}
