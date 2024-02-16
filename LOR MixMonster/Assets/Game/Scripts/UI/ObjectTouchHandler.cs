using GameUtility;
using CandyCoded.HapticFeedback;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTouchHandler : MonoBehaviour
{
    public delegate void OnMonsterSelected(Monster monster);
    public static OnMonsterSelected onMonsterSelected;
    public delegate void OnMonsterReleased(Monster monster);
    public static OnMonsterReleased onMonsterReleased;

    Monster monster;
    [SerializeField]
    private AudioClip pickSFX, releaseSFX;
    private bool hapticsAllowed = true;
    private float scale;
    private void OnEnable()
    {
        monster = GetComponent<Monster>();
        /*var bc = gameObject.AddComponent<BoxCollider2D>();
        bc.offset = new Vector2(0, 4.9f);
        bc.size = new Vector2(10.9f, 21.5f);*/
        scale = 0.25f;
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
        transform.Shake(0.1f, 1, 0.01f, defaultScale: scale);

    }
    private void Update()
    {
        if (!isSelected && isDown && Time.time - touchTime > 0.25f)
        {
            isSelected = true;
            currentPosition = transform.position;
            offset = CameraController.Instance.GetTouchPosition() - transform.position;
            Sound.Controller.Instance.PlayOneShot(pickSFX);
            transform.Shake(0.15f, 1, 0.02f, defaultScale: scale);
        }
    }
    private void OnMouseUp()
    {
        if (!isSelected) return;
        isDown = false;
        isSelected = false;
        if (Sound.Controller.VibrationEnable)
        {
            HapticFeedback.MediumFeedback();
        }
        if (!IsMonsterOnStage())
        {
            transform.position = currentPosition;
            transform.Shake(0.15f, 1, 0.025f, defaultScale: scale);
        }
        onMonsterReleased?.Invoke(monster);
        monster.stageCollectionData.position.Set(transform.position);
        DataManagement.DataManager.Instance.Save();

        Sound.Controller.Instance.PlayOneShot(releaseSFX);

    }
    private bool IsMonsterOnStage()
    {
        if (Physics.Raycast(monster.bottom.position, Vector3.forward, int.MaxValue, layerMask: LayerMask.GetMask("Ground")))
        {
            return true;
        }
        return false;
    }
    private void OnMouseDrag()
    {
        if (isSelected && isDown)
        {
            transform.position = CameraController.Instance.GetTouchPosition() - offset;
            onMonsterSelected?.Invoke(monster);
        }
    }
}
