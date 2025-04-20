using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSfxVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicVolumeSlider.value;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSfxVolume()
    {
        float volume = sfxVolumeSlider.value;
        audioMixer.SetFloat("SfxVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void LoadVolume()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        SetMusicVolume();
        SetSfxVolume();
    }
}
