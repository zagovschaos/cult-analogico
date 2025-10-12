using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Systems.GameEvents.AudioHandlers
{
    public class UnityAudioComponent : MonoBehaviour
    {
        [SerializeField] private UnityAudioHandler audioHandler;

        private void OnEnable()
        {
            var audioSource = transform.GetOrAddComponent<AudioSource>();
            audioHandler.Enable(audioSource);
        }

        private void OnDisable()
        {
            audioHandler.Disable();
        }
    }
}
