using System.Collections.Generic;
using System.Linq;
using Project._Scripts.Runtime.InGame.Audio;
using Project._Scripts.Runtime.ScriptableObjects.AudioData;
using UnityEngine;

namespace Project._Scripts.Runtime.Managers.ManagerClasses
{
    public class AudioManager : MonoBehaviour
    {
        #region Childs

        private AudioSource _bgmSource;

        
        [Header("PoolObjects")]
        [SerializeField] private GameObject MainSfxPoolObject;
        [SerializeField] private GameObject SecondarySfxPoolObject;
        [SerializeField] private GameObject AmbientPoolObject;

        [Space]
        #endregion

        #region Fields

        private List<AudioSource> _mainSfxPool = new();
        private List<AudioSource> _secondarySfxPool = new();
        private List<AudioSource> _ambientSfxPool = new();

        private List<AudioData> _audioDatas = new();

        #endregion


        #region OnAwake

        protected void Awake()
        {
            InitializeAudioDatas();
            Initialize();
        }

        #endregion


        #region Audio Interactions

        private void Initialize()
        {
            //------------------------------INITIALIZING THE AUDIO CHANNELS----------------------------------
            _mainSfxPool = MainSfxPoolObject.GetComponentsInChildren<AudioSource>().ToList();
            _secondarySfxPool = SecondarySfxPoolObject.GetComponentsInChildren<AudioSource>().ToList();
            _ambientSfxPool = AmbientPoolObject.GetComponentsInChildren<AudioSource>().ToList();
        }

        private void InitializeAudioDatas()
        {
            _audioDatas = Resources.LoadAll<AudioData>(nameof(Audio)).ToList();
        }

        /// <summary>
        /// Play Audio on UI Interaction
        /// </summary>
        public void UIF_PlayAudio(string audioName)
        {
            var audioData = GetAudioDataByName(audioName);
            PlayAudio(audioData.AudioClip, 1f, 0.1f, AudioData.AudioType.MainSfx);
        }

        /// <summary>
        /// Play audio clip with default settings
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="pitchVariation"></param>
        /// <param name="type"></param>
        /// <param name="volume"></param>
        public void PlayAudio(AudioClip clip, float volume = 1f, float pitchVariation = 0f,
            AudioData.AudioType type = AudioData.AudioType.SecondarySfx)
        {
            //Null Check
            if (clip == null) return;

            //Get the audio source
            AudioSource source = GetAvailableAudioSource(type);

            //----------------------AUDIO SETTINGS----------------------------
            source.clip = clip;
            source.pitch = 1 + Random.Range(-pitchVariation, pitchVariation);
            source.volume = volume;
            //----------------------------------------------------------------

            //Play audio
            source.Play();
        }

        /// <summary>
        /// Play audio based on given audio object
        /// </summary>
        /// <param name="audioObject"></param>
        public void PlayAudio(AudioData audioObject)
        {
            //Null Check
            if (audioObject.AudioClip == null) return;

            //Get the audio source
            AudioSource source = GetAvailableAudioSource(audioObject.Type);

            //-----------------------------------AUDIO SETTINGS-----------------------------------------
            source.clip = audioObject.AudioClip;
            source.pitch = 1 + Random.Range(audioObject.PitchVariation, -audioObject.PitchVariation);
            source.volume = audioObject.Volume;
            //------------------------------------------------------------------------------------------

            //Play audio
            source.Play();
        }
        
        public void PlayAudio(AudioData audioObject, float volume, float pitchVariation)
        {
            //Null Check
            if (audioObject.AudioClip == null) return;

            //Get the audio source
            AudioSource source = GetAvailableAudioSource(audioObject.Type);

            //-----------------------------------AUDIO SETTINGS-----------------------------------------
            source.clip = audioObject.AudioClip;
            source.pitch = 1 + Random.Range(pitchVariation, -pitchVariation);
            source.volume = volume;
            //------------------------------------------------------------------------------------------

            //Play audio
            source.Play();
        }

        /// <summary>
        /// Play audio based on audio clip name in the audio list
        /// </summary>
        /// <param name="audioList"></param>
        /// <param name="audioName"></param>
        public void PlayAudio(List<Audio> audioList, string audioName)
        {
            AudioData audioObject = GetAudioByName(audioList, audioName).AudioData;

            //Null Check
            if (audioObject.AudioClip == null) return;

            //Get the audio source
            AudioSource source = GetAvailableAudioSource(audioObject.Type);

            //-----------------------------------AUDIO SETTINGS-----------------------------------------
            source.clip = audioObject.AudioClip;
            source.pitch = 1 + Random.Range(audioObject.PitchVariation, -audioObject.PitchVariation);
            source.volume = audioObject.Volume;
            //------------------------------------------------------------------------------------------

            //Play audio
            source.Play();
        }

        /// <summary>
        /// Play audio with it's name
        /// </summary>
        /// <param name="audioName"></param>
        public void PlayAudio(string audioName)
        {
            var audioData = _audioDatas.Find(x => x.name == audioName);

            if (audioData is null)
            {
                Debug.LogError($"There is not audio like{audioName}");
                return;
            }

            PlayAudio(audioData);
        }

        #endregion

        #region Audio Gathering

        /// <summary>
        /// Returns the audio based on it's name from the given audio list
        /// </summary>
        /// <param name="audioList"></param>
        /// <param name="audioName"></param>
        /// <returns></returns>
        public Audio GetAudioByName(List<Audio> audioList, string audioName)
        {
            return audioList.Find(x => x.AudioName == audioName);
        }

        /// <summary>
        /// Returns the audio data based on it's name
        /// </summary>
        /// <param name="audioName"></param>
        /// <returns></returns>
        public AudioData GetAudioDataByName(string audioName)
        {
            var audioData = _audioDatas.Find(x => x.name == audioName);

            if (audioData is not null)
                return audioData;
            
            Debug.LogError($"There is not audio like{audioName}");
            return null;

        }

        /// <summary>
        /// Returns the available audio source channel
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AudioSource GetAvailableAudioSource(AudioData.AudioType type)
        {
            AudioSource source;
            switch ( type )
            {
                case AudioData.AudioType.BGM:
                    return _bgmSource;
                case AudioData.AudioType.MainSfx:
                    if (_mainSfxPool.Exists(x => x.isPlaying == false))
                    {
                        return _mainSfxPool.Find(x => x.isPlaying == false);
                    }
                    source = _mainSfxPool.OrderBy(x => x.time).Last();
                    source.Stop();
                    return source;
                case AudioData.AudioType.SecondarySfx:
                    if (_secondarySfxPool.Exists(x => x.isPlaying == false))
                    {
                        return _secondarySfxPool.Find(x => x.isPlaying == false);
                    }
                    source = _secondarySfxPool.OrderBy(x => x.time).Last();
                    source.Stop();
                    return source;
                case AudioData.AudioType.Ambient:
                    if (_ambientSfxPool.Exists(x => x.isPlaying == false))
                    {
                        return _ambientSfxPool.Find(x => x.isPlaying == false);
                    }
                    source = _ambientSfxPool.OrderBy(x => x.time).Last();
                    source.Stop();
                    return source;
            }
            return null;
        }

        #endregion

    }
}