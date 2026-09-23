using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips; // Array of AudioClips
    public AudioClip[] musicClips; // Array of music clips
    public AudioClip bossBattleMusic; // Boss battle music

    [Header("-------- Audio Source -----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("-------- Audio Mixer Groups -----------")]
    [SerializeField] AudioMixerGroup BGMGroup;
    [SerializeField] AudioMixerGroup SFXGroup;
    
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Set the output audio mixer group of the audio sources
        musicSource.outputAudioMixerGroup = BGMGroup;
        SFXSource.outputAudioMixerGroup = SFXGroup;
        
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // This method is called when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.buildIndex);
    }

    public void PlayMusicForScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < musicClips.Length)
        {
            musicSource.Stop();
            musicSource.clip = musicClips[sceneIndex];
            musicSource.Play();
        }
        else
        {
            Debug.Log("Scene index out of range");
        }
    }

    public void PlayBossBattleMusic()
    {
        musicSource.Stop();
        musicSource.clip = bossBattleMusic;
        musicSource.Play();
    }

    public void PlaySFX(int symbolNumber)
    {
        if (symbolNumber >= 0 && symbolNumber < audioClips.Length)
        {
            SFXSource.PlayOneShot(audioClips[symbolNumber]);
        }
        else
        {
            Debug.Log("Symbol number out of range");
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event when the AudioManager object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}