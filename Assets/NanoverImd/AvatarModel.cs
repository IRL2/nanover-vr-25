using System;
using UnityEngine;
using UnityEngine.Events;

namespace NanoverImd
{
    public class AvatarModel : MonoBehaviour
    {
        [Serializable]
        private class UnityEventColor : UnityEvent<Color> {}
        
        [Serializable]
        private class UnityEventString : UnityEvent<string> {}

        [Header("Avatar Update Events")]
        [SerializeField]
        private UnityEventColor colorUpdated;
        [SerializeField]
        private UnityEventString nameUpdated;

        public void SetPlayerColor(Color color) => colorUpdated?.Invoke(color);
        
        public void SetPlayerName(string name) => nameUpdated?.Invoke(name);
    }
}