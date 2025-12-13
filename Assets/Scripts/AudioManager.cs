using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton pattern để dễ truy cập
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource backgroundMusic; // Nguồn phát nhạc nền
    public AudioSource soundEffect;     // Nguồn phát tiếng động (ăn, chết...)

    [Header("Audio Clips")]
    public AudioClip introMusic;        // Nhạc mở đầu
    public AudioClip ghostSiren;        // Nhạc khi ma đuổi (siren)
    public AudioClip munch1;            // Tiếng ăn 1
    public AudioClip munch2;            // Tiếng ăn 2 (để tạo hiệu ứng waka-waka)
    public AudioClip powerPellet;       // Tiếng khi ăn hạt to (ghost sợ)
    public AudioClip eatGhost;          // Tiếng ăn ma
    public AudioClip pacmanDeath;       // Tiếng chết

    private bool useMunch1 = true;      // Biến để đổi qua lại giữa 2 tiếng munch

    private void Awake()
    {
        // Thiết lập Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Khi game bắt đầu, phát nhạc intro
        PlayIntro();
    }

    public void PlayIntro()
    {
        if (introMusic != null)
        {
            backgroundMusic.clip = introMusic;
            backgroundMusic.loop = false; // Intro chỉ chạy 1 lần
            backgroundMusic.Play();

            // Sau khi intro hết (ví dụ 4 giây), chuyển sang nhạc nền Siren
            // (Bạn có thể điều chỉnh số giây cho khớp file nhạc của bạn)
            Invoke(nameof(PlaySiren), 4.5f);
        }
    }

    public void PlaySiren()
    {
        if (ghostSiren != null)
        {
            backgroundMusic.clip = ghostSiren;
            backgroundMusic.loop = true; // Siren lặp lại liên tục
            backgroundMusic.Play();
        }
    }

    public void PlayMunch()
    {
        AudioClip clip = useMunch1 ? munch1 : munch2;
        soundEffect.PlayOneShot(clip, 0.1f);
        useMunch1 = !useMunch1;
    }

    public void PlayEatGhost()
    {
        if (eatGhost != null)
        {
            soundEffect.PlayOneShot(eatGhost);
        }
    }

    public void PlayDeath()
    {
        // Dừng nhạc nền khi chết
        backgroundMusic.Stop();

        if (pacmanDeath != null)
        {
            soundEffect.PlayOneShot(pacmanDeath);
        }
        
    }
}