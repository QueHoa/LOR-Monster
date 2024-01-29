using Effect;
using UnityEngine;


public class MoneyEffect : EffectAbstract
{
    [SerializeField]
    private ParticleSystem ps;
    [SerializeField]
    private TMPro.TextMeshPro amountText;
    [SerializeField]
    private AudioClip[] clip;

    public override bool IsUsing()
    {
        return gameObject.activeSelf;
    }

    public override EffectAbstract Active(Vector3 pos, int amount)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("Active");
        amountText.text = $"+{GameUtility.GameUtility.ShortenNumber(amount)}";
        if (ps != null)
        {
            ps.Play();
        }

        if (clip != null && clip.Length > 0)
        {
            Sound.Controller.Instance.PlayOneShot(clip[Random.Range(0, clip.Length)], 1);
        }

        return this;
    }

    public override EffectAbstract SetColor(Color color)
    {
        amountText.color = color;
        return this;
    }
}