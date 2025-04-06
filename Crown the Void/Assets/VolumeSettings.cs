using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        setVolume();
    }

    public void setVolume()
    {
        float volume = volumeSlider.value;
        audioMixer.SetFloat("Volume", Mathf.Log10(volume)*20);
    }
}
