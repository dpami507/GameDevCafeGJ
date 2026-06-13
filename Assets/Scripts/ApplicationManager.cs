using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager instance;

    [Header("Sound")]
    [SerializeField] Sound[] sounds;
    [SerializeField] float volume;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        Events.TriggerSound += PlaySound;
    }
    private void OnDestroy()
    {
        Events.TriggerSound -= PlaySound;
    }
    void PlaySound(string id)
    {
        foreach (var sound in sounds)
        {
            if (sound.id == id)
            {
                GameObject soundObj = new GameObject(id);
                AudioSource src = soundObj.AddComponent<AudioSource>();
                src.clip = sound.clip;
                src.volume = sound.volume * volume;

                src.Play();
                Destroy(soundObj, sound.clip.length);
            }
        }
    }
}
[System.Serializable]
public class Sound
{
    public string id;
    public AudioClip clip;
    public float volume;    
}
