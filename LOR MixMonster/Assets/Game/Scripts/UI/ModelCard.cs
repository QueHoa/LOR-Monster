using DataManagement;
using GameUtility;
using UnityEngine;
using Spine;
using Spine.Unity;
using TMPro;

public class ModelCard : MonoBehaviour, ISelectableButton
{

    [SerializeField] private SkeletonGraphic anim, eventAnim;
    [SerializeField] private AudioClip selectSFX, removeSFX;
    [SerializeField] private TextMeshProUGUI collectionTotalText;

    private Skin _mixAndMatchSkin;
    //private CollectionData _collectionData;


    private void OnEnable()
    {
        //var collectionData = DataManager.Instance.userData.inventory.GetFirstCollection();

        /*if (collectionData != null)
            SetUp(collectionData);
        else
            gameObject.SetActive(false);*/
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
            var skeletonData = anim.Skeleton.Data;
            _mixAndMatchSkin.AddSkin(skeletonData.FindSkin(part));
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void UpdateSkin()
    {
        var skeleton = anim.Skeleton;
        skeleton.SetSkin(_mixAndMatchSkin);
        skeleton.SetSlotsToSetupPose();
    }

    public void OnSelect()
    {
        /*Debug.Log("ON SELECT: " + _collectionData.id);
        onModelSelected?.Invoke(_collectionData);*/
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

    public ModelCard(Skin mixAndMatchSkin)
    {
        _mixAndMatchSkin = mixAndMatchSkin;
    }

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
        /*DataManager.Instance.userData.inventory.RemoveCollection(_collectionData.id);
        onModelCleared?.Invoke(_collectionData);*/

        _ = transform.Shake(0.15f, 1, 0.15f);
        Sound.Controller.Instance.PlayOneShot(removeSFX);
    }

    private void OnCollectionUpdated(Inventory inventory)
    {
        Debug.Log("SET COLLECTION");
        //var collectionData = DataManager.Instance.userData.inventory.GetFirstCollection();
        /*if (collectionData != null)
        {
            Debug.Log("SET COLLECTION " + collectionData.id);
            collectionTotalText.text = DataManager.Instance.userData.inventory.GetTotalCollection().ToString();
        }*/
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