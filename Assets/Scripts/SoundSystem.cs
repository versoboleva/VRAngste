using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;

public class SoundSystem : MonoBehaviour 
{
    public AudioClip[] rainInsideSounds = new AudioClip[4];
    public AudioClip[] rainOutsideSounds = new AudioClip[4];
    public AudioClip[] rainCarSounds = new AudioClip[4];
    public AudioClip[] thunder;
    public AudioClip[] windInsideSounds = new AudioClip[4];
    public AudioClip[] windOutsideSounds = new AudioClip[4];
    public AudioClip[] windCarSounds = new AudioClip[4];
    private AudioClip[] currentWind;
    private AudioClip[] currentRain;
    public int currentScene = 0;
    public AudioSource rainSource;
    public AudioSource windSource;
    public AudioSource thunderSource;
    public Transform player;
    public ParticleSystem Particlerain;
    public float speedOfSound = 100f;
    public int windIntencity = 0;
    public int rainIntencity = 0;
    public float thunderVolume = 0;

    private void Awake()
    {
        SetScene();
    }
    private void Start()
    {
        rainSource.transform.position = player.position;
        windSource.transform.position = player.position;
        PlayRain();
        PlayWind();

    }

    private void FixedUpdate()
    {
        rainSource.transform.position = player.position;
        windSource.transform.position = player.position;
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
        if(this.windIntencity != windIntencity)
        {
            this.windIntencity = windIntencity;
            PlayWind();
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

    public void SetWind(int windIntencity)
    {
        if (this.windIntencity != windIntencity)
        {
            this.windIntencity = windIntencity;
            PlayWind();
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
        switch (currentScene)
        {
            case 0:
                currentRain = rainInsideSounds;
                currentWind = windInsideSounds;
                break;
            case 1:
                currentRain = rainOutsideSounds;
                currentWind = windOutsideSounds;
                break;
            case 2:
                currentRain = rainCarSounds;
                currentWind = windCarSounds;
                break;
            default:
                Debug.LogError($"Invalid currentScene: {currentScene}");
                currentRain = null;
                currentWind = null;
                return;
        }

        if (currentRain == null || currentRain.Length == 0)
            Debug.LogError("CurrentRain array is empty! Assign AudioClips in Inspector.");
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
        if (thunder.Length == 0 || thunderSource == null || player == null)
            return;

        
        int index = Random.Range(0, thunder.Length);

        float distance = Vector3.Distance(player.position, lightningPos);

        float delay = distance / speedOfSound;

        StartCoroutine(PlayThunderDelayed(thunder[index], delay, lightningPos));
    }

    private IEnumerator PlayThunderDelayed(AudioClip clip, float delay, Vector3 lightningPos)
    {
        yield return new WaitForSeconds(delay);

        thunderSource.transform.position = lightningPos;

        thunderSource.clip = clip;
        thunderSource.Play();

        Debug.Log($"Thunder played at {lightningPos} after {delay:F2} seconds");
    }
    private void PlayWind()
    {
        if (currentWind[windIntencity] != null && windSource != null)
        {
            windSource.clip = currentWind[windIntencity];
            windSource.Play();
        }
    }


}
