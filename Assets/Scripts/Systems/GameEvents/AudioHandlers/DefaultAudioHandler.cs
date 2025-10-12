using Assets.Scripts.Systems.GameEvents.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Systems.GameEvents.AudioHandlers
{
    public class DefaultAudioHandler : ScriptableObject, IAudioHandler
    {
        public bool LoggingEnabled = false;

        public virtual void Init(bool loggingEnabled)
        {
            LoggingEnabled = loggingEnabled;
        }

        public virtual void Play(AudioClip clip)
        {
            if (LoggingEnabled)
                Debug.LogAssertion($"[DefaultAudioHandler] Audio clip '{clip.name}' was played");
        }
    }
}