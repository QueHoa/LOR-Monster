using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    Camera camera;
    Cinemachine.CinemachineVirtualCamera cam;
    Cinemachine.CinemachineBasicMultiChannelPerlin noise;
    Transform _transform;

    float defaultLenSize;
    float power, duration;
    bool active = false;
    float time = 0;
    private void Start()
    {
        camera = Camera.main;
        Instance = this;
        cam = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
        _transform = cam.transform;
        noise = cam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        defaultLenSize = cam.m_Lens.OrthographicSize;

    }
    
    void Toggle(bool active)
    {
        Camera.main.backgroundColor = active?Color.black:new Color(0,0,1);
    }

    Coroutine c;
    CancellationTokenSource cancellation;
    public async UniTask LerpOffset(Vector3 newOffset, float speed = 0.05f)
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = null;
        }
        if (cancellation == null)
        {
            cancellation = new CancellationTokenSource();
        }
        while (Vector3.Distance(CameraController.Instance.GetOffset(), newOffset) > 0.1f)
        {
            CameraController.Instance.SetOffset(Vector3.Lerp(CameraController.Instance.GetOffset(), newOffset, speed));
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellation.Token);
        }
    }
    public void ClearLerp()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = null;
        }
    }
    IEnumerator DoMoveCamera(Vector3 newOffset,float speed=0.05f)
    {
        while (Vector3.Distance(CameraController.Instance.GetOffset(), newOffset) > 0.1f)
        {
            CameraController.Instance.SetOffset(Vector3.Lerp(CameraController.Instance.GetOffset(), newOffset, speed));
            yield return null;
        }
        c = null;
    }
    CancellationTokenSource disableCancellation;

    private void OnEnable()
    {
        disableCancellation = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        if (disableCancellation != null)
            disableCancellation.Cancel();
    }

    private void OnDestroy()
    {
        if (disableCancellation != null)
        {
            disableCancellation.Cancel();
            disableCancellation.Dispose();
        }
    }
    public void SetBoundary(Collider2D collider)
    {
        cam.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingShape2D=collider;
    }
    public void ClearShake()
    {
        noise.m_FrequencyGain = 0;
        noise.m_AmplitudeGain = 0;
        duration = 0;
    }
    public void ShakeCam(float power, float duration,float amptitude)
    {
        if (!active)
        {
            ClearShake();
        }
        active = true;
        time = 0;
        this.power = power;
        this.duration = Mathf.Max(this.duration,duration);
        noise.m_FrequencyGain = Mathf.Max(noise.m_FrequencyGain, power);
        noise.m_AmplitudeGain = Mathf.Max(noise.m_AmplitudeGain, amptitude);
    }

    public async UniTask Zoom(AnimationCurve cameraHighLiveZoomCurve, CancellationToken cancellationToken)
    {
        float t = 0;
        while (t < cameraHighLiveZoomCurve.keys[cameraHighLiveZoomCurve.length - 1].time)
        {
            cam.m_Lens.OrthographicSize = cameraHighLiveZoomCurve.Evaluate(t);
            t += Time.fixedDeltaTime;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
        }
    }

    public void ShakeCam(float power,float amptitude)
    {
        active = false;
        duration = 0;
        noise.m_FrequencyGain = Mathf.Max(noise.m_FrequencyGain, power);
        noise.m_AmplitudeGain = Mathf.Max(noise.m_AmplitudeGain, amptitude);
    }
    public void SetUp(Collider collider)
    {
        cam.GetComponent<CinemachineConfiner>().m_BoundingVolume = collider;
    }
    public void Follow(Transform target,bool instant=false)
    {
        cam.Follow = target;
        if (instant)
        {
            cam.ForceCameraPosition(target.position,Quaternion.identity);
        }
    }
    public Vector3 GetOffset()
    {
        var transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
        return transposer.m_FollowOffset;
    }
    public void SetOffset(Vector3 offset)
    {
        var transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
         transposer.m_FollowOffset=offset;
    }
    public float GetSize()
    {
        return cam.m_Lens.OrthographicSize;
    }
    public void SetSize(float size)
    {
         cam.m_Lens.OrthographicSize=size;
    }
    public async UniTask SetSize(float size,float speed)
    {
        if (speed == 1)
        {
            cam.m_Lens.OrthographicSize = size;
            return;
        }
        float target = size;
        while (Mathf.Abs(cam.m_Lens.OrthographicSize - target)>0.2f)
        {
            cam.m_Lens.OrthographicSize = Mathf.Lerp(cam.m_Lens.OrthographicSize, target, speed);
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate,cancellationToken:disableCancellation.Token);
        }
    }
    public Vector3 GetTouchPosition()
    {
        Vector3 vector3 = camera.ScreenToWorldPoint(Input.mousePosition);
        vector3.z = 0;
        return vector3;
    }
    private void FixedUpdate()
    {
        if (active)
        {
            if (time < duration)
            {
                time += Time.fixedDeltaTime;
                noise.m_FrequencyGain -= Time.fixedDeltaTime * power / duration;
            }
            else
            {
                active = false;
                noise.m_FrequencyGain = 0;

            }
        }
    }
    public Transform GetTransform()
    {
        return _transform;
    }
}
