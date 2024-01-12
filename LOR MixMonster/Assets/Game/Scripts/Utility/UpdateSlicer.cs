using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IBatchUpdate
{
    void BatchUpdate();
    void BatchFixedUpdate();
}
public class UpdateSlicer : MonoBehaviour
{
    public enum UpdateMode { BucketA, BucketB, BucketC, Always, Auto ,None}
    public static UpdateSlicer Instance { get; private set; }
    private readonly List<HashSet<IBatchUpdate>> slicedUpdateBehavioursBucket = new List<HashSet<IBatchUpdate>>();
    private readonly List<HashSet<IBatchUpdate>> slicedFixedUpdateBehavioursBucket = new List<HashSet<IBatchUpdate>>();
    private int current = 0, currentFixedUpdateIndex = 0;
    private int registerIndex = 0;
    private int fixedUpdateRegisterIndex = 0;
    [SerializeField]
    private int totalBucket = 4;
    public void RegisterSlicedUpdate(IBatchUpdate slicedUpdateBehaviour, UpdateMode updateMode = UpdateMode.Auto, UpdateMode fixedUpdateMode = UpdateMode.None)
    {
        if (updateMode == UpdateMode.Always)
        {
            for (int i = 0; i < slicedUpdateBehavioursBucket.Count; i++)
            {
                slicedUpdateBehavioursBucket[i].Add(slicedUpdateBehaviour);
            }
        }
        else if (updateMode != UpdateMode.None)
        {
            slicedUpdateBehavioursBucket[updateMode == UpdateMode.Auto ? (registerIndex++ % slicedUpdateBehavioursBucket.Count) : (int)updateMode].Add(slicedUpdateBehaviour);
        }

        if (fixedUpdateMode == UpdateMode.Always)
        {
            for (int i = 0; i < slicedFixedUpdateBehavioursBucket.Count; i++)
            {
                slicedFixedUpdateBehavioursBucket[i].Add(slicedUpdateBehaviour);
            }
        }
        else if(fixedUpdateMode!=UpdateMode.None)
        {
            slicedFixedUpdateBehavioursBucket[fixedUpdateMode == UpdateMode.Auto ? (fixedUpdateRegisterIndex++ % slicedFixedUpdateBehavioursBucket.Count) : (int)fixedUpdateMode].Add(slicedUpdateBehaviour);
        }
    }

    public void DeregisterSlicedUpdate(IBatchUpdate slicedUpdateBehaviour)
    {
        for (int i = 0; i < slicedUpdateBehavioursBucket.Count; i++)
        {
            slicedUpdateBehavioursBucket[i].Remove(slicedUpdateBehaviour);
        }
        for (int i = 0; i < slicedFixedUpdateBehavioursBucket.Count; i++)
        {
            slicedFixedUpdateBehavioursBucket[i].Remove(slicedUpdateBehaviour);
        }
    }
    private void OnDestroy()
    {
        slicedUpdateBehavioursBucket.Clear();
        slicedFixedUpdateBehavioursBucket.Clear();
    }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            for (int i = 0; i < totalBucket; i++)
            {
                slicedUpdateBehavioursBucket.Add(new HashSet<IBatchUpdate>());
                slicedFixedUpdateBehavioursBucket.Add(new HashSet<IBatchUpdate>());
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        var targetUpdateFunctions = slicedUpdateBehavioursBucket[current];
        foreach (var slicedUpdateBehaviour in targetUpdateFunctions)
        {
            slicedUpdateBehaviour.BatchUpdate();
        }
        current++;
        if (current >= slicedUpdateBehavioursBucket.Count)
        {
            current = 0;
        }
    }

    void FixedUpdate()
    {
        var targetUpdateFunctions = slicedFixedUpdateBehavioursBucket[currentFixedUpdateIndex];
        foreach (var slicedUpdateBehaviour in targetUpdateFunctions)
        {
            slicedUpdateBehaviour.BatchFixedUpdate();
        }
        currentFixedUpdateIndex++;
        if (currentFixedUpdateIndex >= slicedFixedUpdateBehavioursBucket.Count)
        {
            currentFixedUpdateIndex = 0;
        }
    }
}