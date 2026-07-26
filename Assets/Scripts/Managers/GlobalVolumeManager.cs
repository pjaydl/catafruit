using UnityEngine;

public class GlobalVolumeManager : MonoBehaviour
{
    public static GlobalVolumeManager Instance { get; private set; }

    private const string VolumeKey = "MasterVolume";
    private const float DefaultVolume = 1f;

    public float CurrentVolume { get; private set; }

    private void Awake()
    {
        // If another GlobalVolumeManager already exists,
        // destroy this new duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this manager when changing scenes.
        DontDestroyOnLoad(gameObject);

        LoadMasterVolume();
    }

    public void SetMasterVolume(float newVolume)
    {
        CurrentVolume = Mathf.Clamp01(newVolume);

        AudioListener.volume = CurrentVolume;

        PlayerPrefs.SetFloat(VolumeKey, CurrentVolume);
        PlayerPrefs.Save();

        Debug.Log("Master volume saved: " + CurrentVolume);
    }

    public float GetMasterVolume()
    {
        return CurrentVolume;
    }

    private void LoadMasterVolume()
    {
        CurrentVolume = PlayerPrefs.GetFloat(
            VolumeKey,
            DefaultVolume
        );

        CurrentVolume = Mathf.Clamp01(CurrentVolume);
        AudioListener.volume = CurrentVolume;

        Debug.Log("Master volume loaded: " + CurrentVolume);
    }
}