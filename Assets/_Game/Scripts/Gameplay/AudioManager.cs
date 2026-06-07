using UnityEngine;
using Superdude.Core;

namespace Superdude.Gameplay
{
    /// <summary>
    /// Воспроизводит звуки по событиям EventBus.
    /// Все AudioClip назначаются в Inspector.
    /// Вешается на [Systems].
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private AudioClip _menuMusic;
        [SerializeField] private AudioClip _gameMusic;

        [Header("SFX")]
        [SerializeField] private AudioClip _passengerJoinSfx;
        [SerializeField] private AudioClip _chainBreakSfx;
        [SerializeField] private AudioClip _speedBoostSfx;
        [SerializeField] private AudioClip _gameOverSfx;

        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        private void Awake()
        {
            ServiceLocator.Register<AudioManager>(this);

            // Два AudioSource — один для музыки (loop), один для SFX
            _musicSource          = gameObject.AddComponent<AudioSource>();
            _musicSource.loop     = true;
            _musicSource.volume   = 0.5f;

            _sfxSource            = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop       = false;
            _sfxSource.volume     = 1f;

            EventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Subscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Subscribe<MainMenuRequestedEvent>(OnMainMenu);
            EventBus.Subscribe<ChainGrewEvent>(OnChainGrew);
            EventBus.Subscribe<ChainBrokenEvent>(OnChainBroken);
            EventBus.Subscribe<SpeedBoostEvent>(OnSpeedBoost);
            EventBus.Subscribe<GameOverEvent>(OnGameOver);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameStarted);
            EventBus.Unsubscribe<MainMenuRequestedEvent>(OnMainMenu);
            EventBus.Unsubscribe<ChainGrewEvent>(OnChainGrew);
            EventBus.Unsubscribe<ChainBrokenEvent>(OnChainBroken);
            EventBus.Unsubscribe<SpeedBoostEvent>(OnSpeedBoost);
            EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        }

        // ── Music ────────────────────────────────────────────────────────

        private void OnGameStarted(GameStartedEvent e)    => PlayMusic(_gameMusic);
        private void OnGameStarted(GameRestartedEvent e)  => PlayMusic(_gameMusic);
        private void OnMainMenu(MainMenuRequestedEvent e) => PlayMusic(_menuMusic);

        private void PlayMusic(AudioClip clip)
        {
            if (clip == null || _musicSource.clip == clip) return;
            _musicSource.clip = clip;
            _musicSource.Play();
        }

        // ── SFX ──────────────────────────────────────────────────────────

        private void OnChainGrew(ChainGrewEvent e)      => PlaySfx(_passengerJoinSfx);
        private void OnChainBroken(ChainBrokenEvent e)  => PlaySfx(_chainBreakSfx);
        private void OnSpeedBoost(SpeedBoostEvent e)    => PlaySfx(_speedBoostSfx);

        private void OnGameOver(GameOverEvent e)
        {
            _musicSource.Stop();
            PlaySfx(_gameOverSfx);
        }

        private void PlaySfx(AudioClip clip)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip);
        }

        // ── Public API (опционально) ─────────────────────────────────────

        public void SetMusicVolume(float v) => _musicSource.volume = Mathf.Clamp01(v);
        public void SetSfxVolume(float v)   => _sfxSource.volume   = Mathf.Clamp01(v);
    }
}
