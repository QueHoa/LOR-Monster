using System.Collections;
using System.Collections.Generic;

namespace Effect
{
    public class EffectSpawner : SpawnerBase<EffectAbstract>
    {
        public static EffectSpawner Instance;
        public void Start()
        {

            Instance = this;
            Init();
        }
        public override void SetActive(EffectAbstract t, bool active)
        {
            t.gameObject.SetActive(active);
        }
        public override bool IsActive(EffectAbstract t)
        {
            return t.IsUsing();
        }
        public override void DestroySpawn(EffectAbstract obj)
        {
            Destroy(obj.gameObject);
        }
    }
}