using GameUtility;
using MoreMountains.NiceVibrations;
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
    [SerializeField]
    protected HapticTypes hapticTypes = HapticTypes.Warning;
    private bool hapticsAllowed = true;
    private float scale;
    private void OnEnable()
    {
        monster = GetComponent<Monster>();
        /*var bc = gameObject.AddComponent<BoxCollider2D>();
        bc.offset = new Vector2(0, 4.9f);
        bc.size = new Vector2(10.9f, 21.5f);*/
        scale = 0.25f;
        MMVibrationManager.SetHapticsActive(hapticsAllowed);
    }
    private void OnDisable()
    {

    }
    bool isDown = false, isSelected, posNow;
    Vector3 offset;
    Vector3 currentPosition;
    float touchTime;
    private void OnMouseDown()
    {
        if (!enabled) return;

        posNow = true;
        isDown = true;
        (Game.Controller.Instance.gameController).isSelected = true;
        currentPosition = transform.position;
        Debug.LogError(currentPosition);
        touchTime = Time.time;

    }
    private void Update()
    {
        if (Input.touchCount > 0 & isDown)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                ((StageGameController)Game.Controller.Instance.gameController).RewardEarningSelect(monster);
                isDown = false;
                posNow = false;
            }
        }
        if (!isSelected && isDown && Time.time - touchTime > 0.3f)
        {
            isSelected = true;
            
            if (posNow)
            {
                transform.position = CameraController.Instance.GetTouchPosition();
                posNow = false;
            }
            offset = CameraController.Instance.GetTouchPosition() - transform.position;
            Sound.Controller.Instance.PlayOneShot(pickSFX);
        }
    }
    private void OnMouseUp()
    {
        if (!isSelected) return;
        //isDown = false;
        isSelected = false;
        if (Sound.Controller.VibrationEnable)
        {
            MMVibrationManager.Haptic(hapticTypes, true, true, this);
        }
        if (!IsMonsterOnStage())
        {
            transform.Shake(0.15f, 1, 0.01f, defaultScale: scale);
            transform.position = currentPosition;
            Debug.LogError(transform.position);
        }
        Debug.LogError(transform.position);
        onMonsterReleased?.Invoke(monster);
        Debug.LogError(transform.position);
        monster.stageCollectionData.position.Set(transform.position);
        Debug.LogError(transform.position);
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
            transform.Shake(2f, 0.7f, 0.01f, defaultScale: scale);
        }
    }
}
