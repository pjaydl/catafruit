using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSliderUI : MonoBehaviour
{
    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        RefreshSlider();
    }

    private void OnEnable()
    {
        RefreshSlider();
    }

    public void ChangeVolume(float value)
    {
        if (GlobalVolumeManager.Instance == null)
        {
            Debug.LogError(
                "No GlobalVolumeManager exists in this scene.",
                this
            );

            return;
        }

        GlobalVolumeManager.Instance.SetMasterVolume(value);
    }

    public void RefreshSlider()
    {
        if (volumeSlider == null)
        {
            volumeSlider = GetComponent<Slider>();
        }

        if (GlobalVolumeManager.Instance == null)
        {
            Debug.LogWarning(
                "Cannot refresh slider: GlobalVolumeManager is missing.",
                this
            );

            return;
        }

        float currentVolume =
            GlobalVolumeManager.Instance.GetMasterVolume();

        volumeSlider.SetValueWithoutNotify(currentVolume);

        Debug.Log("Slider refreshed: " + currentVolume);
    }
}