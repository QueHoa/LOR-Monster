using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    [SerializeField]
    private Spine.Unity.SkeletonAnimation anim;
    public void SetUp(string skinId,Vector2 position)
    {
        anim.initialSkinName=(skinId);
        anim.Initialize(true);
        transform.localPosition = position;
        gameObject.SetActive(true);
    }
}
