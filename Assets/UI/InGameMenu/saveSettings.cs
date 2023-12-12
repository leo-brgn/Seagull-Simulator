using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class configScript : MonoBehaviour
{
    public Slider sliderVolume;
    public Slider sliderBrightness;
    public Toggle toggleMusic;
    public Button saveButton;

    public AudioSource audioSource;
    public AudioMixer audioMixer;

    public GUIScript guiScript;

    public void Start()
    {
        float volume = audioMixer.GetFloat("Volume", out float volumeValue) ? volumeValue : 1f;
        // Transform from logarithmic to linear
        float normalizedVolume = Mathf.Pow(10f, volume / 20f);
        float sliderVolumeValue = normalizedVolume * 100f;
        PlayerPrefs.SetFloat("Volume", sliderVolumeValue);
        PlayerPrefs.SetFloat("Brightness", Screen.brightness);
        PlayerPrefs.SetInt("Music", audioSource.isPlaying ? 1 : 0);
        sliderVolume.value = PlayerPrefs.GetFloat("Volume", 1f);
        sliderBrightness.value = PlayerPrefs.GetFloat("Brightness", 1f);
        toggleMusic.isOn = PlayerPrefs.GetInt("Music", 1) == 1;

        sliderVolume.onValueChanged.AddListener(ChangeVolume);
        sliderBrightness.onValueChanged.AddListener(ChangeBrightness);
        toggleMusic.onValueChanged.AddListener(ChangeMusic);
        saveButton.onClick.AddListener(SaveConfig);
    }

    public void ChangeVolume(float newVolume)
    {
        newVolume = Mathf.Clamp(newVolume, 0f, 100f);
        float normalizedVolume = newVolume / 100f;
        float logarithmicVolume = Mathf.Log10(normalizedVolume) * 20;
        audioMixer.SetFloat("Volume", logarithmicVolume);
    }

    public void ChangeBrightness(float newBrightness)
    {
        newBrightness = Mathf.Clamp(newBrightness, 0f, 100f);
        float normalizedBrightness = newBrightness / 100f;

        Screen.brightness = normalizedBrightness;
    }

    public void ChangeMusic(bool estado)
    {
        if (estado){
            audioSource.Play();
        }else {
            audioSource.Stop();
        }
    }

    // Called when we click the "save" button.
    public void SaveConfig()
    {
        PlayerPrefs.SetFloat("Volume", sliderVolume.value);
        PlayerPrefs.SetFloat("Brightness", sliderBrightness.value);
        PlayerPrefs.SetInt("Music", toggleMusic.isOn ? 1 : 0);
        PlayerPrefs.Save();
        guiScript.returnToMenu();
    }
}