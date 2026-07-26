using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip creditsMusic;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameplaySceneName = "Gameplay";
    [SerializeField] private string creditsSceneName = "Credits";

    [Header("Music Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    private AudioSource audioSource;

    private void Awake()
    {
        // Prevent duplicate music managers.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep the music manager when changing scenes.
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = musicVolume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Play music for the first scene.
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        if (sceneName == mainMenuSceneName)
        {
            PlayMusic(mainMenuMusic);
        }
        else if (
            sceneName == gameplaySceneName ||
            sceneName == "Level1" ||
            sceneName == "Level2" ||
            sceneName == "Level3"
        )
        {
            PlayMusic(gameplayMusic);
        }
        else if (sceneName == creditsSceneName)
        {
            PlayMusic(creditsMusic);
        }
        else
        {
            Debug.LogWarning("No music assigned for scene: " + sceneName);
        }
    }

    private void PlayMusic(AudioClip newMusic)
    {
        if (newMusic == null)
        {
            Debug.LogWarning("The music clip is missing.");
            return;
        }

        // Do not restart the same music.
        if (audioSource.clip == newMusic && audioSource.isPlaying)
        {
            return;
        }

        audioSource.Stop();
        audioSource.clip = newMusic;
        audioSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        audioSource.volume = musicVolume;
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void ResumeMusic()
    {
        if (audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}