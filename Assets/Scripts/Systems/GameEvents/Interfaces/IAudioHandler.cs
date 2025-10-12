using UnityEngine;

namespace Assets.Scripts.Systems.GameEvents.Interfaces
{
    public interface IAudioHandler
    {
        void Init(bool loggingEnabled);
        void Play(AudioClip clip);
    }
}