using Assets.Scripts.Systems.GameEvents.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Systems.GameEvents
{
    public class GameEventListener : MonoBehaviour, IEventListener
    {
        public GameEvent GameEvent;
        public CustomGameEvent Response;

        private void OnEnable()
            => GameEvent.Subscribe(this);

        private void OnDisable()
            => GameEvent.Unsubscribe(this);

        public virtual void OnEventRaised(IEvent gameEvent, Component sender, object data)
            => Response.Invoke(sender, data);
    }
}