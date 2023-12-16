using UnityEngine;

namespace Project._Scripts.Runtime.ScriptableObjects.AudioData
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "Game/AudioData")]
    public class AudioData : ScriptableObject
    {
    #region Fields
        [Header("Fields")]
        public AudioClip AudioClip;
        public AudioType Type;
        public float Volume;
        [Range(-.5f,.5f)]
        public float PitchVariation;
        public enum AudioType
        {
            BGM,
            MainSfx,
            SecondarySfx,
            Ambient
        }
    #endregion
    }
}