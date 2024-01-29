using UnityEngine;
using MoreMountains.NiceVibrations;

namespace Settings
{
    public enum ToggleType
    {
        Music,
        Sound,
        Vibration,
    }

    public class ToggleButton : MonoBehaviour
    {
        public ToggleType type;
        public UIToggle uiToggle;

        private bool IsOn()
        {
            return type switch
            {
                ToggleType.Music => Sound.Controller.MusicEnable,
                ToggleType.Sound => Sound.Controller.SfxEnable,
                ToggleType.Vibration => Sound.Controller.VibrationEnable,
            };
        }

        private void Reset()
        {
            uiToggle = GetComponentInChildren<UIToggle>();
        }

        private void OnEnable()
        {
            // Sound.Controller.OnSoundChanged += OnSoundChanged;

            if (IsOn())
                uiToggle.TurnOn(immediate: true);
            else
                uiToggle.TurnOff(immediate: false);
        }

        public void OnClick()
        {
            if (IsOn())
                TurnOff();
            else
                TurnOn();
        }

        private void TurnOn()
        {
            uiToggle.TurnOn();

            switch (type)
            {
                case ToggleType.Music:
                    Sound.Controller.MusicEnable = true;
                    Sound.Controller.Instance.PlaySettingsSfx(true);
                    break;
                case ToggleType.Sound:
                    Sound.Controller.SfxEnable = true;
                    Sound.Controller.Instance.PlaySettingsSfx(true);
                    break;
                case ToggleType.Vibration:
                    Sound.Controller.VibrationEnable = true;
                    Sound.Controller.Instance.PlaySettingsSfx(true);
                    // SoundManager.PlaySound(SoundName.BUTTON_ON);
                    break;
            }
        }

        private void TurnOff()
        {
            uiToggle.TurnOff();

            switch (type)
            {
                case ToggleType.Music:
                    Sound.Controller.Instance.PlaySettingsSfx(false);
                    Sound.Controller.MusicEnable = false;
                    break;
                case ToggleType.Sound:
                    Sound.Controller.Instance.PlaySettingsSfx(false);
                    Sound.Controller.SfxEnable = false;
                    break;
                case ToggleType.Vibration:
                    Sound.Controller.Instance.PlaySettingsSfx(false);
                    Sound.Controller.VibrationEnable = false;
                    break;
            }
        }
    }
}