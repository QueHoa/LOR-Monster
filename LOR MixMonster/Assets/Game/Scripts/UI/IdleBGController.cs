using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class IdleBGController : MonoBehaviour
{
    public static IdleBGController Instance;
    [SerializeField] private float minZoom = 7;
    [SerializeField] private float maxZoom = 22;


    private Camera cam;
    private bool moveAllowed;
    private Vector3 touchPos;

    //private float mapMinX = -30.72f, mapMaxX = 30.72f, mapMinY = -30.72f, mapMaxY = 30.72f;
    //private readonly float mapMinX = -29.5f;
    //private readonly float mapMaxX = 29.5f;

    private readonly float mapMinX = -30.72f;
    private readonly float mapMaxX = 30.72f;

    private readonly float mapMinY = -22f;
    private readonly float mapMaxY = 22f;
    public bool canInteract = true;
    private bool isPinch;

    /// <summary>
    /// condition 1 finger tap
    /// </summary>
    //private bool isClickToRaiseMoney = false;
    //private bool boosterPrevent = false;
    //public bool BoosterPrevent { get => boosterPrevent; set => boosterPrevent = value; }

    private Vector3 endPos;
    public float distanceForSwipe = 2.5f;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;


    }

    public Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;


        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }


    private void Update()
    {
        if (!canInteract) return;
        if (Input.touchCount == 0) isPinch = false;
        if (Input.touchCount > 0)
        {
            if (Input.touchCount == 2)
            {
                isPinch = true;
                //do zooming
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                if (EventSystem.current.IsPointerOverGameObject(touchZero.fingerId) || EventSystem.current.IsPointerOverGameObject(touchOne.fingerId))
                    return;

                Vector2 touchZeroLastPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOneLastPos = touchOne.position - touchOne.deltaPosition;

                float disTouch = (touchZeroLastPos - touchOneLastPos).magnitude;  //khoang cach 2 ngon tay ban dau khi cham vao man hinh
                float currentDisTouch = (touchZero.position - touchOne.position).magnitude; //khoang cach 2 ngon tay sau khi vuot zoom

                float difference = currentDisTouch - disTouch;
                Zoom(difference * 0.02f);
            }
            else if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        {
                            moveAllowed = false;
                        }
                        else
                        {
                            moveAllowed = true;
                            //isClickToRaiseMoney = true;
                        }
                        touchPos = cam.ScreenToWorldPoint(touch.position);
                        break;
                    case TouchPhase.Moved:
                        if (moveAllowed && !isPinch)
                        {
                            endPos = cam.ScreenToWorldPoint(touch.position);
                            if (Vector3.Distance(touchPos, endPos) > distanceForSwipe)
                            {
                                //isClickToRaiseMoney = false;
                                Vector3 direction = touchPos - endPos;
                                cam.transform.position += direction;
                                cam.transform.position = ClampCamera(cam.transform.position);
                            }
                        }
                        break;
                    case TouchPhase.Ended:
                        
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void Zoom(float increment)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - increment, minZoom, maxZoom);
        cam.transform.position = ClampCamera(cam.transform.position);
    }
}
