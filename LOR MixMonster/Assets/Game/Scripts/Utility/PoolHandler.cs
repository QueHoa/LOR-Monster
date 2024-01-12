using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtility.Pooling
{
    public class PoolHandler:MonoBehaviour
    {
        public List<PoolComponent> pool = new List<PoolComponent>();
        private List<PoolComponent> busyPool = new List<PoolComponent>();
        GameObject prefab;
        Transform holder;
        int index = 0;
        public PoolComponent Get()
        {
            if (holder == null)
            {
                this.holder = transform;
                this.prefab = transform.GetChild(0).gameObject;
            }
            if (pool.Count > 0)
            {
                PoolComponent result = pool[pool.Count - 1];

                pool.Remove(result);
                busyPool.Add(result);

                return result;
            }
            GameObject obj = GameObject.Instantiate(prefab, holder) as GameObject;
            PoolComponent t = obj.GetComponent<PoolComponent>();
            t.name = index++.ToString();
            busyPool.Add(t);
            t.OnInitialized(this);

            return t;
        }
        public void Release(PoolComponent poolComponent)
        {
            if (busyPool.Contains(poolComponent))
            {
                busyPool.Remove(poolComponent);
                pool.Add(poolComponent);
            }
        }
        public void Clear()
        {
            busyPool.Reverse();
            pool.AddRange(busyPool);
            busyPool.Clear();
        }
    }

    public abstract class PoolComponent : MonoBehaviour
    {
        PoolHandler poolHandler;
        public void OnInitialized(PoolHandler poolHandler)
        {
            this.poolHandler = poolHandler;
        }
        protected virtual void OnDestroy()
        {
            poolHandler.Release(this);
        }
        protected virtual void OnDisable()
        {
            poolHandler.Release(this);
        }

    }
}