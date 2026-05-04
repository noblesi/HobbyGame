using UnityEngine;

namespace TopDownRoguelite.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class GameAudioPlayer : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip attackClip;
        [SerializeField] private AudioClip hitClip;
        [SerializeField] private AudioClip enemyDeathClip;
        [SerializeField] private AudioClip experiencePickupClip;
        [SerializeField] private AudioClip levelUpClip;
        [SerializeField] private AudioClip upgradeSelectClip;

        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float volume = 0.6f;

        private static GameAudioPlayer instance;
        private AudioSource audioSource;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            DontDestroyOnLoad(gameObject);
        }

        public static void PlayAttack()
        {
            Play(instance != null ? instance.attackClip : null);
        }

        public static void PlayHit()
        {
            Play(instance != null ? instance.hitClip : null);
        }

        public static void PlayEnemyDeath()
        {
            Play(instance != null ? instance.enemyDeathClip : null);
        }

        public static void PlayExperiencePickup()
        {
            Play(instance != null ? instance.experiencePickupClip : null);
        }

        public static void PlayLevelUp()
        {
            Play(instance != null ? instance.levelUpClip : null);
        }

        public static void PlayUpgradeSelect()
        {
            Play(instance != null ? instance.upgradeSelectClip : null);
        }

        private static void Play(AudioClip clip)
        {
            if (instance == null || clip == null)
            {
                return;
            }

            instance.audioSource.PlayOneShot(clip, instance.volume);
        }
    }
}
