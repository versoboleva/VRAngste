using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundSystem : MonoBehaviour 
{
    public static SoundSystem Instance;
    
    public AudioClip[] rainInsideSounds = new AudioClip[4];
    public AudioClip[] rainOutsideSounds = new AudioClip[4];
    public AudioClip[] rainCarSounds = new AudioClip[4];
    public AudioClip[] thunderInside;
    public AudioClip[] thunderOutside;
    private AudioClip[] currentThunder;
    private AudioClip[] currentRain;
    public int currentScene = 0;
    public AudioSource rainSource;
    public AudioSource windSource;
    public AudioSource thunderSource;
    public Transform player;
    public ParticleSystem Particlerain;
    public float speedOfSound = 100f;
    public int rainIntencity = 0;
    public float thunderVolume = 0;



    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetAudioForSceneChange();
        StartCoroutine(SetupSceneObjects());
    }

    private void ResetAudioForSceneChange()
    {
        StopAllCoroutines();

        if (rainSource != null)
        {
            rainSource.Stop();
            rainSource.clip = null;
        }
        
        if (thunderSource != null)
        {
            thunderSource.Stop();
            thunderSource.clip = null;
        }
    }
    private IEnumerator SetupSceneObjects()
    {
        yield return null; // wait for scene init

        player = null;
        Particlerain = null;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        Particlerain = GameObject.FindGameObjectWithTag("RainSystem")?.GetComponent<ParticleSystem>();

        SetScene();

        if (player != null)
        {
            if (rainSource != null)
                rainSource.transform.position = player.position;
        }
        rainIntencity = 0;


        PlayRain();
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        if (rainSource != null)
            rainSource.transform.position = player.position;
    }

    private void SetThunderVolume()
    {
        if (thunderSource != null)
        {
            thunderSource.volume = Mathf.Clamp01(thunderVolume);
        }
    }

    public void SetRainIntencity(int intencity)
    {
        rainIntencity = intencity;
        PlayRain();
    }

    public void SetSoundSystem(int sceneNr, int rainIntencity, int windIntencity, int donnerVolume)
    {
        if(currentScene != sceneNr)
        {
            currentScene = sceneNr;
            SetScene();
        }
        if (this.rainIntencity != rainIntencity)
        {
            this.rainIntencity = rainIntencity;
            PlayRain();
        }
        if (thunderVolume != donnerVolume / 100f)
        {
            thunderVolume = donnerVolume / 100f;
            SetThunderVolume();
        }
    }

    public void SetSceneNr(int sceneNr)
    {
        if (currentScene != sceneNr)
        {
            currentScene = sceneNr;
            SetScene();
        }
    }

    public void SetThunder(float donnerVolume)
    {
        if (thunderVolume != donnerVolume)
        {
            thunderVolume = donnerVolume;
            SetThunderVolume();
        }
    }

    private void SetScene()
    {
        rainIntencity = 0;
        thunderVolume = 0;
        switch (currentScene)
        {
            case 0:
                currentRain = null;
                currentThunder = null;
                if (rainSource != null) rainSource.Stop();
                if (thunderSource != null) thunderSource.Stop();
                break;
            case 1:
                currentRain = rainInsideSounds;
                currentThunder = thunderInside;
                break;
            case 2:
                currentRain = rainOutsideSounds;
                currentThunder = thunderOutside;
                break;
            case 3:
                currentRain = rainCarSounds;
                currentThunder = thunderInside;
                break;
            default:
                Debug.LogError($"Invalid currentScene: {currentScene}");
                currentRain = null;
                return;
        }
    }

    private void PlayRain()
    {
        if (currentRain == null)
        {
            Debug.LogError("currentRain array is NULL!");
            return;
        }
        if (currentRain[rainIntencity] != null && rainSource != null)
        {
            rainSource.clip = currentRain[rainIntencity];
            rainSource.Play();
        }

    }

    public void PlayThunder(Vector3 lightningPos)
    {
        if (currentThunder.Length == 0 || thunderSource == null || player == null)
            return;

        
        int index = Random.Range(0, currentThunder.Length);

        float distance = Vector3.Distance(player.position, lightningPos);

        float delay = distance / speedOfSound;

        StartCoroutine(PlayThunderDelayed(currentThunder[index], delay, lightningPos));
    }

    private IEnumerator PlayThunderDelayed(AudioClip clip, float delay, Vector3 lightningPos)
    {
        yield return new WaitForSeconds(delay);

        thunderSource.transform.position = lightningPos;

        thunderSource.clip = clip;
        thunderSource.Play();

        Debug.Log($"Thunder played at {lightningPos} after {delay:F2} seconds");
    }
    /*private void PlayWind()
    {
        if (currentWind[windIntencity] != null && windSource != null)
        {
            windSource.clip = currentWind[windIntencity];
            windSource.Play();
        }
    }*/


}
