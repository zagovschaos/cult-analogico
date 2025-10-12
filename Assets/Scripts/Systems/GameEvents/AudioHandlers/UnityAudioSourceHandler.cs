using Assets.Scripts.Systems.GameEvents.Interfaces;
using System;
using UnityEngine;

namespace Assets.Scripts.Systems.GameEvents.AudioHandlers
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Game Events/UnityAudioSourceHandler")]
    public class UnityAudioHandler : DefaultAudioHandler, IEventListener
    {
        [Header("Audio Source Configurations")]
        [SerializeField] private bool Loop;
        [SerializeField] private bool OverrideAudioOnPlay;
        [Range(0, 1)] public float Volume;


        [SerializeField] private GameEvent VolumeChanged;

        private AudioSource _audioSource = default;

        public void Enable(AudioSource audioSource)
        {
            _audioSource = audioSource;
            _audioSource.volume = Volume;
            _audioSource.loop = Loop;

            if (VolumeChanged != null)
                VolumeChanged.Subscribe(this);
        }

        public void Disable()
        {
            _audioSource = null;

            if (VolumeChanged != null)
                VolumeChanged.Unsubscribe(this);
        }

        public void OnEventRaised(IEvent gameEvent, Component sender, object data)
        {
            if (gameEvent.Name == VolumeChanged.Name)
                _audioSource.volume = Volume;
        }

        public override void Play(AudioClip clip)
        {
            if (LoggingEnabled)
                Debug.LogAssertion($"[UnityAudioHandler] Audio clip '{clip.name}' was played");

            if (OverrideAudioOnPlay)
                _audioSource.Stop();

            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}