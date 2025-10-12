using Assets.Scripts.Systems.GameEvents.AudioHandlers;
using Assets.Scripts.Systems.GameEvents.Interfaces;
using Assets.Scripts.Systems.Util;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Systems.GameEvents
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Game Event")]
    public class GameEvent : ScriptableObject, IEvent
    {
        [SerializeField] private bool LoggingEnabled = false;
        [SerializeField] private List<AudioClip> Clips;
        [SerializeField] private DefaultAudioHandler AudioHandler;

        [HideInInspector] public string Name { get => name; }

        private List<IEventListener> Listeners = new();

        public AudioClip GetClip() => Clips.GetRandom();

        private void OnEnable()
        {
            Listeners = new List<IEventListener>();

            if (AudioHandler == default)
            {
                AudioHandler = CreateInstance<DefaultAudioHandler>();
                AudioHandler.Init(LoggingEnabled);
            }
        }

        #region Public Broadcast Wrapers
        public void Broadcast() => BroadcastEvent(null, null, DoNothing);
        public void Broadcast(string data) => BroadcastEvent(null, data, DoNothing);
        public void Broadcast(object data) => BroadcastEvent(null, data, DoNothing);
        public void Broadcast(Component sender) => BroadcastEvent(sender, null, DoNothing);
        public void Broadcast(Component sender, object data) => BroadcastEvent(sender, data, DoNothing);
        public void Broadcast(object data, Action callback) => BroadcastEvent(null, data, callback);
        public void Broadcast(Component sender, object data, Action callback) => BroadcastEvent(sender, data, callback);
        private void DoNothing() { }
        #endregion

        private void BroadcastEvent(Component sender, object data, Action callback)
        {
            for (int i = Listeners.Count - 1; i >= 0; i--)
                Listeners[i].OnEventRaised(this, sender, data);

            if (Clips.Count > 0)
                AudioHandler.Play(GetClip());

            callback();
        }

        public void Subscribe(IEventListener listener)
        {
            if (!Listeners.Contains(listener))
                Listeners.Add(listener);
        }

        public void Unsubscribe(IEventListener listener)
        {
            if (Listeners.Contains(listener))
                Listeners.Remove(listener);
        }
    }
}
