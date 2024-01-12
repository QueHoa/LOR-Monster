using Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NormalEffect : EffectAbstract
{
    [SerializeField]
    private ParticleSystem ps;
    [SerializeField]
    private AudioClip [] clip;
    [SerializeField]
    private float soundVol = 1;
    public override bool IsUsing()
    {
        return ps.isPlaying;
    }
    public override void Active()
    {
        gameObject.SetActive(true);
        if (ps != null)
        {
            ps.Play();
        }
        //SoundHandler sound = GetComponentInChildren<SoundHandler>();
        if (clip != null && clip.Length>0)
        {
            Sound.Controller.Instance.PlayOneShot(clip[Random.Range(0,clip.Length)],soundVol);
        }
    }

    public override EffectAbstract Active(MeshRenderer renderer)
    {
        gameObject.SetActive(true);
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.meshRenderer = renderer;
        ps.Play();
        if (clip != null && clip.Length > 0)
        {
            Sound.Controller.Instance.PlayOneShot(clip[Random.Range(0, clip.Length)], soundVol);
        }
        return base.Active(renderer);
    }
    public override EffectAbstract Active(SpriteRenderer renderer)
    {
        gameObject.SetActive(true);
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.spriteRenderer = renderer;
        ps.Play();
        if (clip != null && clip.Length > 0)
        {
            Sound.Controller.Instance.PlayOneShot(clip[Random.Range(0, clip.Length)], soundVol);
        }
        return base.Active(renderer);
    }
    public override EffectAbstract Active(Vector3 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        if (ps != null)
        {
            ps.Play();
        }
        if (clip != null && clip.Length > 0)
        {
            Sound.Controller.Instance.PlayOneShot(clip[Random.Range(0, clip.Length)], soundVol);
        }
        return this;
    }

}
