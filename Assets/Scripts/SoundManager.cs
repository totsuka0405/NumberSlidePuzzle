using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip slideSE;
    [SerializeField] private AudioClip buttonSE;
    [SerializeField] private AudioClip bgmClip;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        PlayBGM();
    }

    private void Start()
    {
        
    }

    public void PlayBGM()
    {
        if (audioSource == null || bgmClip == null)
        {
            Debug.LogWarning("BGM Source or Clip is missing.");
            return;
        }

        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlaySlideSE()
    {
        if (slideSE != null)
        {
            audioSource.PlayOneShot(slideSE);
        }
    }

    public void PlaySlideSEUI()
    {
        if (slideSE != null)
        {
            audioSource.PlayOneShot(buttonSE);
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
            Debug.Log("音量が設定されました: " + volume);
        }
        else
        {
            Debug.LogWarning("AudioSourceが設定されていません");
        }
    }

    public float GetVolume()
    {
        return audioSource != null ? audioSource.volume : 0f;
    }
}