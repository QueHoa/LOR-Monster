using GameUtility;
using UnityEngine;

public  class BestViewMarker:MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI viewText;
    RectTransform _transform;
    public void SetUp(Vector2 pos, int view)
    {
        if (_transform == null)
        {
            _transform = GetComponent<RectTransform>();
        }
        viewText.text = GameUtility.GameUtility.ShortenNumber(view);
        _transform.anchoredPosition = pos;
        gameObject.SetActive(true);
    }
    public void Finish()
    {
        _transform.Shake(0.15f, 1f, 1f);
    }
    float lastShake = 0;
    public void Shake()
    {
        if (Time.time - lastShake > 0.15f)
        {
            _transform.Shake(0.1f, 0.2f, 0.01f);
            lastShake = Time.time;
        }
    }
}
