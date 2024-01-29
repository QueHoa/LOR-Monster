using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sound { 
    public class Controller : MonoBehaviour
    {
        private static bool sfxEnable, musicEnable, vibrationEnable;
        public static bool SfxEnable
        {
            get { return sfxEnable; }

            set
            {
                sfxEnable = value;
                onSoundChange?.Invoke(sfxEnable);
                PlayerPrefs.SetInt("Sound", value ? 1 : 0);
            }
        }
        public static bool MusicEnable
        {
            get { return musicEnable; }

            set
            {
                musicEnable = value;
                if (musicEnable)
                {
                    Instance.ContinueMusic();
                }
                else
                {
                    Instance.PauseMusic();
                }
                PlayerPrefs.SetInt("Music", value ? 1 : 0);
            }
        }
        public static bool VibrationEnable
        {
            get => vibrationEnable;
            set
            {
                vibrationEnable = value;
                PlayerPrefs.SetInt("Vibration", value ? 1 : 0);
            }
        }
        public delegate void OnSoundChange(bool state);
        public static OnSoundChange onSoundChange;
        public static Controller Instance;
        [SerializeField]
        private AudioSource sfxPlayer, musicPlayer;

        public SoundData soundData;
        private void Start()
        {
            if (Instance == null)
            {
                sfxEnable = PlayerPrefs.GetInt("Sound", 1) == 1;
                musicEnable = PlayerPrefs.GetInt("Music", 1) == 1;
                vibrationEnable = PlayerPrefs.GetInt("Vibration", 1) == 1;
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Addressables.LoadAssetAsync<SoundData>("Sound Data").Completed += op =>
                {
                    soundData = op.Result;
                };
            }
            else
            {
                Destroy(gameObject);
            }
        }
       
        public bool IsReady()
        {
            return soundData != null;
        }
        public void PlayOneShot(AudioClip clip, float vol = 1)
        {
            if (SfxEnable && clip != null)
            {
                sfxPlayer.PlayOneShot(clip, vol);
            }
        }
        public void PlayMusic(AudioClip clip, float vol = 1)
        {
            if (MusicEnable)
            {
                if (musicPlayer.clip == clip) return;
                musicPlayer.clip = clip;
                musicPlayer.volume = vol;
                musicPlayer.loop = true;
                musicPlayer.Play();
            }
        }
        public AudioSource GetMusicPlayer()
        {
            return musicPlayer;
        }
        public void StopMusic()
        {
            musicPlayer.clip = null;
            musicPlayer.Stop();
        }
        public void PauseMusic()
        {
            if(musicPlayer.isPlaying)
            musicPlayer.Pause();
        }
        public void ContinueMusic()
        {
            if (musicPlayer.clip == null)
            {
                PlayMusic(soundData.menuTheme[Random.Range(0, soundData.menuTheme.Length)]);
            }
            else
            {
                musicPlayer.UnPause();
            }
        }
        public void PlayClickSFX()
        {
            PlayOneShot(soundData.clickSFXs[Random.Range(0,soundData.clickSFXs.Length)],0.1f);
        }
        public void PlaySettingsSfx(bool turnOn, float vol = 1)
        {
            PlayOneShot(soundData.settingSfxs[turnOn ? 1 : 0], vol);
        }

        public void PlayPopupSfx(bool open, float vol = 1)
        {
            PlayOneShot(soundData.popupSfxs[open ? 1 : 0], vol);
        }

    }
}