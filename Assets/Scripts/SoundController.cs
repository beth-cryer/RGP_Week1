using UnityEngine;

public class SoundController : MonoBehaviour
{
    private AudioSource src;

    [SerializeField] private AudioClip[] clipsCollide;
    [SerializeField] private AudioClip[] clipsStick;

    public static SoundController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        src = GetComponent<AudioSource>();
    }

    public void Play(AudioClip sfx)
    {
        if (sfx == null) return;
        src.PlayOneShot(sfx);
    }

    public void PlayStick()
    {
        int len = clipsStick.Length;
        int r = Random.Range(0, len - 1);

        Play(clipsStick[r]);
    }

    public void PlayCollide()
    {
        int len = clipsCollide.Length;
        int r = Random.Range(0, len - 1);

        Play(clipsStick[r]);
    }
}
