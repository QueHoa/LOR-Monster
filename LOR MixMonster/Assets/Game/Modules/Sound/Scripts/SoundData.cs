using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(menuName = "SoundData/Data")]
    public class SoundData : ScriptableObject
    {
        public AudioClip []menuTheme,finalThemes;
        public AudioClip purchaseSFX;
        public AudioClip [] coinSfxs, clickSFXs,playerWinSFX,playerLoseSFX,collectSFXs;

        [Space] public AudioClip[] settingSfxs;

        [Space] public AudioClip[] popupSfxs;
        public AudioClip GetCoinSFX()
        {
            return coinSfxs[Random.Range(0, coinSfxs.Length)];
        }
      
    }
}