using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private Spine.Unity.SkeletonAnimation anim;
    [SerializeField]
    private MonsterHead monsterHead;
    [SerializeField]
    private GameObject middle;
    [SerializeField]
    private Transform[] heads;
    Vector3 defaultMonsterHeadScale;
    public async UniTask SetUp(List<ItemData.Item> selectedItems)
    {
        foreach(ItemData.Item item in selectedItems)
        {
            switch (item.category)
            {
                case ItemData.Category.Body:
                    //anim.initialSkinName=(item.skin);
                    //anim.Initialize(true, true);
                    anim.gameObject.SetActive(true);
                    anim.Skeleton.SetSkin(item.skin);

                    switch (item.skin)
                    {
                        case "0":
                        case "1":
                            monsterHead.transform.SetParent(heads[0]);
                            monsterHead.transform.localPosition = Vector3.zero;
                            break;
                        case "15":
                            monsterHead.transform.SetParent(heads[2]);
                            monsterHead.transform.localPosition = Vector3.zero;
                            break;
                        default:
                            monsterHead.transform.SetParent(heads[1]);
                            monsterHead.transform.localPosition = Vector3.zero;
                            break;
                    }
                    break;
                default:
                    await monsterHead.SetItem(item);
                    break;
            }
        }

        gameObject.SetActive(true);
    }

    public async UniTask<GameObject> SetItem(ItemData.Item item)
    {
        switch (item.category)
        {
            case ItemData.Category.Body:
                Debug.Log("SET SKIN: " + item.skin);
                //anim.initialSkinName = (item.skin);
                //anim.Initialize(true, true);
                anim.gameObject.SetActive(true);
                anim.Skeleton.SetSkin(item.skin);

                switch (item.skin)
                {
                    case "0":
                    case "1":
                        monsterHead.transform.SetParent(heads[0]);
                        break;
                    case "15":
                        monsterHead.transform.SetParent(heads[2]);
                       
                        break;
                    case "11":
                        monsterHead.transform.SetParent(heads[3]);

                        break;
                    default:
                        monsterHead.transform.SetParent(heads[1]);
                        break;
                }
                monsterHead.transform.localPosition = Vector3.zero;
                monsterHead.transform.localEulerAngles = Vector3.zero;
                return middle;
            default:
                return await monsterHead.SetItem(item);
        }

    }

    public GameObject GetItemPlace(ItemData.Item item)
    {
        switch (item.category)
        {
            case ItemData.Category.Body:
                return middle;
            default:
                return monsterHead.GetItemPlace(item);
        }
        
    }
    public void SetUp()
    {
        defaultMonsterHeadScale = monsterHead.transform.localScale;
        //SetItem(Game.Controller.Instance.itemData.GetPack(ItemData.Category.Body).GetRandom()).Forget();
        anim.gameObject.SetActive(false);
        gameObject.SetActive(true);
        isIdle = true;
    }
    public void SetAnim(string anim)
    {
        try
        {
            this.anim.AnimationState.SetAnimation(0, anim, true);
        }catch(System.Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void Dance(int musicThemeIndex)
    {
        try
        {
            isIdle = false;
            if (this.anim.Skeleton.Skin.Name.Equals("0") || this.anim.Skeleton.Skin.Name.Equals("1"))
            {
                this.anim.AnimationState.SetAnimation(0, "toilet_dance", true);
            }
            else if (this.anim.Skeleton.Skin.Name.Equals("15"))
            {
                this.anim.AnimationState.SetAnimation(0, "trash_dance", true);
            }
            else
            {
                if (musicThemeIndex == 3)
                {
                    this.anim.AnimationState.SetAnimation(0, "dance_4", false);
                    this.anim.AnimationState.AddAnimation(0, "dance_1", true, 0);
                }
                else if (musicThemeIndex == 4)
                {
                    this.anim.AnimationState.SetAnimation(0, UnityEngine.Random.Range(0f,1f)>0.5f?$"dance_2":"dance_3", true);
                }
                else
                {
                    this.anim.AnimationState.SetAnimation(0, $"dance_{musicThemeIndex+1}", true);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void PickLeft()
    {
        this.anim.AnimationState.SetAnimation(0, "ver_2/left_1", false);
        this.anim.AnimationState.AddAnimation(0, "ver_2/left_2_loop", false, 0);
        this.anim.AnimationState.AddAnimation(0, "ver_2/left_3", false, 0);
        this.anim.AnimationState.AddAnimation(0, "ver_2/idle", true, 0);
    }
    public void PickRight()
    {
        this.anim.AnimationState.SetAnimation(0, "ver_2/right_1", false);
        this.anim.AnimationState.AddAnimation(0, "ver_2/right_2_loop", false, 0);
        this.anim.AnimationState.AddAnimation(0, "ver_2/right_3", false, 0);
        this.anim.AnimationState.AddAnimation(0, "ver_2/idle", true, 0);
    }
    public void SetIdle()
    {
        this.anim.AnimationState.SetAnimation(0, "ver_2/idle", true);

    }

    bool isIdle = false;
   
    Vector3 scale;
    [SerializeField]
    AnimationCurve headCurveX;
    [SerializeField]
    AnimationCurve headCurveY;
    float time = 0;
    private void Update()
    {
        if (isIdle)
        {
            scale.Set(headCurveX.Evaluate(time%1), headCurveY.Evaluate(time%1),1);
            monsterHead.transform.localScale = defaultMonsterHeadScale+scale;
            time += Time.deltaTime;
        }
    }
}
