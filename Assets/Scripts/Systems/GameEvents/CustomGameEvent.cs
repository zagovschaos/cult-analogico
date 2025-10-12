using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Systems.GameEvents
{
    [Serializable]
    public class CustomGameEvent : UnityEvent<Component, object> { }
}