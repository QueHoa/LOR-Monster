using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    [SerializeField]
    Rect rect;
    [SerializeField]
    Vector2 min, max;
    private enum Direction
    {
        Horizontal,Vertical
    }
    [SerializeField]
    private Direction direction=Direction.Horizontal;
    // Start is called before the first frame update
    void OnEnable()
    {
        this.rect = Screen.safeArea;
        RectTransform rt = GetComponent<RectTransform>();
        if (direction == Direction.Vertical)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, ((Screen.height - rect.height)/2f - rect.y));
            rt.offsetMax = new Vector2(rt.offsetMax.x, -((Screen.height - rect.height)/2f - rect.y));
        }
        else
        {
            min = rt.offsetMin;
            max = rt.offsetMax;
            rt.offsetMin = new Vector2(rect.x, 0);
            rt.offsetMax = new Vector2(-rect.x, 0);
        }


    }

}
