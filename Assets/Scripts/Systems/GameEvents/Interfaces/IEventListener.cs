using UnityEngine;

namespace Assets.Scripts.Systems.GameEvents.Interfaces
{
    public interface IEventListener
    {
        public void OnEventRaised(IEvent gameEvent, Component sender, object data);
    }
}
