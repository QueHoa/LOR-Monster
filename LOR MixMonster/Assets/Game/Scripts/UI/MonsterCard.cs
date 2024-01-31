using DataManagement;
using GameUtility;
using UnityEngine;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine.UI;
using CodeStage.AntiCheat.Genuine.CodeHash;
using static UnityEngine.Rendering.DebugUI;
using System.IO;
using UnityEditorInternal;
using System.Collections.Generic;

public class MonsterCard : MonoBehaviour, ISelectableButton
{
    public delegate void OnMonsterSelected(CardData cardData);
    public static OnMonsterSelected onMonsterSelected;

    public delegate void OnMonsterCleared(CardData cardData);
    public static OnMonsterCleared onMonsterCleared;

    public Monster monster;
    [SerializeField] private AudioClip selectSFX, removeSFX;
    [SerializeField] private TextMeshProUGUI collectionTotalText;
    [SerializeField] private ParticleSystem newMonsterPS;

    CardData _cardData;
    

    private void OnEnable()
    {
        var collectionData = DataManager.Instance.userData.inventory.GetFirstCollection();

        if (collectionData != null)
            SetUp(collectionData);
        else
            gameObject.SetActive(false);
    }

    public void SetUp(CardData cardData)
    {
        _cardData = cardData;

        foreach (var itemId in cardData.items)
        {
            var item = (ItemData.Item)Sheet.SheetDataManager.Instance.gameData.itemData.GetItem(itemId);

            if (item.category == ItemData.Category.Accessory)
                continue;

            AddPart(item.skin);
        }

        UpdateSkin(cardData);
        gameObject.SetActive(true);
        newMonsterPS.Play();
        DataManager.Instance.userData.inventory.onUpdate += OnCollectionUpdated;
        OnCollectionUpdated(DataManager.Instance.userData.inventory);
    }

    private void OnDisable()
    {
        DataManager.Instance.userData.inventory.onUpdate -= OnCollectionUpdated;
    }

    private void OnDestroy()
    {
        DataManager.Instance.userData.inventory.onUpdate -= OnCollectionUpdated;
    }

    public void AddPart(string part)
    {
        try
        {
            
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void UpdateSkin(CardData cardData)
    {
        List<ItemData.Item> tempMonsterItems = new List<ItemData.Item>();
        foreach (string id in cardData.items)
        {
            if (Game.Controller.Instance.itemData.GetItem(id).category != ItemData.Category.Pet)
            {
                tempMonsterItems.Add(Game.Controller.Instance.itemData.GetItem(id));
            }
        }
        monster.layer.sortingOrder = 201;
        monster.SetUp(tempMonsterItems);
        foreach (ItemData.Item item in tempMonsterItems)
        {
            monster.SetItem(item);
        }

        monster.SetIdle();
    }

    public void OnSelect()
    {
        Debug.Log("ON SELECT: " + _cardData.id);
        onMonsterSelected?.Invoke(_cardData);
        Sound.Controller.Instance.PlayOneShot(selectSFX);
    }

    private float touchTime;
    private bool isDown;

    public void OnTouch()
    {
        isDown = true;
        touchTime = Time.time;
    }

    private Vector3 scale;

    [SerializeField]
    private AnimationCurve holdCurve;


    private void Update()
    {
        if (!isDown) return;

        if (Time.time - touchTime < holdCurve.keys[holdCurve.length - 1].time)
        {
            Debug.Log("Check time...".Color("orange"));
            scale.x = scale.y = holdCurve.Evaluate(Time.time - touchTime);
            transform.localScale = scale;
        }
        else
        {
            Debug.Log("On select...".Color("lime"));
            isDown = false;
            OnSelect();
        }
    }

    public void OnRelease()
    {
        isDown = false;
        transform.localScale = Vector3.one;
    }

    public void Clear()
    {
        DataManager.Instance.userData.inventory.RemoveCollection(_cardData.id);
        onMonsterCleared?.Invoke(_cardData);

        _ = transform.Shake(0.15f, 1, 0.15f);
        Sound.Controller.Instance.PlayOneShot(removeSFX);
    }

    private void OnCollectionUpdated(Inventory inventory)
    {
        Debug.Log("SET COLLECTION");
        var collectionData = DataManager.Instance.userData.inventory.GetFirstCollection();
        if (collectionData != null)
        {
            Debug.Log("SET COLLECTION " + collectionData.id);
            collectionTotalText.text = DataManager.Instance.userData.inventory.GetTotalCollection().ToString();
        }
    }

    private void OnHold()
    {
        Debug.Log("ON HOLD".Color("magenta"));
    }
}

public interface ISelectableButton
{
    void OnSelect();
}