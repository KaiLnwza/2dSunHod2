using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource walkSource;
    public AudioSource sfxSource;

    [Header("System & BGM")]
    public AudioClip bgMusic;
    [Range(0f, 1f)] public float bgMusicVol = 0.5f;

    public AudioClip winSound;
    [Range(0f, 1f)] public float winVol = 1f;

    public AudioClip loseSound;
    [Range(0f, 1f)] public float loseVol = 1f;

    [Header("Player SFX")]
    public AudioClip walkSound;
    [Range(0f, 1f)] public float walkVol = 0.4f;

    public AudioClip attackSound;
    [Range(0f, 1f)] public float attackVol = 1f;

    public AudioClip attackHitSound;
    [Range(0f, 1f)] public float attackHitVol = 1f;

    public AudioClip getHitSound;
    [Range(0f, 1f)] public float getHitVol = 1f;

    [Header("Boss SFX")]
    public AudioClip bossLaserSound;
    [Range(0f, 1f)] public float bossLaserVol = 1f;

    public AudioClip bossLavaSound;
    [Range(0f, 1f)] public float bossLavaVol = 1f;

    public AudioClip bossBombSound;
    [Range(0f, 1f)] public float bossBombVol = 1f;

    public AudioClip bossGhostSound;
    [Range(0f, 1f)] public float bossGhostVol = 1f;

    public AudioClip bossDieSound;
    [Range(0f, 1f)] public float bossDieVol = 1f;

    private bool isGameEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        isGameEnded = false;
        if (bgMusic != null && bgmSource != null)
        {
            bgmSource.clip = bgMusic;
            bgmSource.volume = bgMusicVol;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    void Update()
    {
        if (isGameEnded) return;

        if (bgmSource != null) bgmSource.volume = bgMusicVol;
        if (walkSource != null) walkSource.volume = walkVol;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, GetClipVolume(clip));
    }

    public void SetWalking(bool isWalking)
    {
        if (isGameEnded || walkSound == null || walkSource == null) return;

        if (isWalking && !walkSource.isPlaying)
        {
            walkSource.clip = walkSound;
            walkSource.volume = walkVol;
            walkSource.loop = true;
            walkSource.Play();
        }
        else if (!isWalking && walkSource.isPlaying)
        {
            walkSource.Stop();
        }
    }

    // สั่งหยุดเพลง BG และเสียงเดินทั้งหมดแบบเด็ดขาด
    public void StopBGM()
    {
        isGameEnded = true;
        if (bgmSource != null)
        {
            bgmSource.Stop();
            bgmSource.loop = false;
        }
        if (walkSource != null)
        {
            walkSource.Stop();
            walkSource.loop = false;
        }
    }

    // จบเกม: หยุด BG + เล่นเสียง Win/Lose
    public void PlayEndGame(AudioClip endClip)
    {
        StopBGM();
        PlaySFX(endClip);
    }

    private float GetClipVolume(AudioClip clip)
    {
        if (clip == winSound) return winVol;
        if (clip == loseSound) return loseVol;
        if (clip == attackSound) return attackVol;
        if (clip == attackHitSound) return attackHitVol;
        if (clip == getHitSound) return getHitVol;
        if (clip == bossLaserSound) return bossLaserVol;
        if (clip == bossLavaSound) return bossLavaVol;
        if (clip == bossBombSound) return bossBombVol;
        if (clip == bossGhostSound) return bossGhostVol;
        if (clip == bossDieSound) return bossDieVol;
        return 1f;
    }
}