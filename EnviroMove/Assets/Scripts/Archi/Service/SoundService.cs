using System;
using System.Collections.Generic;
using System.Linq;
using Archi.Service.Interface;
using Attributes;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Archi.Service
{
    public class AudioService : Service, IAudioService
    {
        [DependeOnService] private ITickService m_tick;
        
        private AudioMixer _mixer;
        
        private List<AudioSource> _listeners = new();
        AudioSource _musicListener;
        
        private List<KeyValuePair<AudioSource, float>> _randomPitchListeners = new();

        private float musicVolume;
        private float fxVolume;

        protected override void Initialize()
        {
            m_tick.OnUpdate += Update;
            
            AdresseHelper.LoadAssetWithCallback<AudioMixer>("AudioMixer", mixer =>
            {
                Debug.Log("Init Sound");
                _mixer = mixer;
                mixer.GetFloat("Music", out musicVolume);
                mixer.GetFloat("FX", out fxVolume);
                
                if(PlayerPrefs.GetInt("Music", 1) == 0)MuteMusic();
                if(PlayerPrefs.GetInt("FX", 1) == 0)MuteSound();
                
                var go = Object.Instantiate(new GameObject());
                go.name = "SoundPlayer";
                Object.DontDestroyOnLoad(go);
                for (int i = 0; i < 3; i++)
                {
                    var listener = go.AddComponent<AudioSource>();
                    listener.outputAudioMixerGroup = mixer.FindMatchingGroups("FX")[0];
                    _listeners.Add(listener);
                }

                _musicListener = go.AddComponent<AudioSource>();
                _musicListener.loop = true;
                _musicListener.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
            });
        }


        void Update()
        {
            _randomPitchListeners.ForEach(listener =>
            {
                if (!listener.Key.isPlaying)
                {
                    listener.Key.pitch = UnityEngine.Random.Range(.8f,1.2f);
                }
            });
        }
        
        
        
        
        public void StartSound(AudioClip clip, bool loop = false)
        {
            foreach (var listener in _listeners)
            {
                if (!listener.isPlaying)
                {
                    listener.clip = clip;
                    listener.loop = loop;
                    listener.Play();
                    return;
                }
            }
        }

        public void StartSoundWithPitch(AudioClip clip, float pitchForce)
        {
            foreach (var listener in _listeners)
            {
                if (!listener.isPlaying)
                {
                    listener.clip = clip;
                    listener.loop = true;
                    _randomPitchListeners.Add(new KeyValuePair<AudioSource, float>(listener, pitchForce));
                    listener.Play();
                    return;
                }
            }
        }

        public void PlaySound (AudioClip clip      )=> _listeners.ForEach(listener => { if(listener.clip == clip      )listener.Play();});
        public void PauseSound(AudioClip clip      )=> _listeners.ForEach(listener => { if(listener.clip == clip      )listener.Pause();});

        public void StopSound(AudioClip clipToStop)
        { 
            var kvp = _randomPitchListeners.FirstOrDefault(kvp => kvp.Key.clip == clipToStop);
            if(kvp.Key != default) _randomPitchListeners.Remove(kvp);
            _listeners.ForEach(listener => { if(listener.clip == clipToStop)listener.Stop();});
        }

        public void PlaySounds()=> _listeners.ForEach(listener => listener.Play());
        public void PauseSounds()=>_listeners.ForEach(listener => listener.Pause());
        public void StopSounds()=> _listeners.ForEach(listener => listener.Stop());

        public bool IsPlaying(AudioClip clip)=>_listeners.Any(source => source.clip == clip && source.isPlaying)|| _randomPitchListeners.Any(kvp => kvp.Key.clip == clip);
          
            

        public void StartMusic(AudioClip music)
        {
            _musicListener.clip = music;
            _musicListener.Play();
        }
        public void PlayMusic()=>  _musicListener.Play();
        public void PauseMusic()=> _musicListener.Pause();
        public void StopMusic()=>  _musicListener.Stop();

        
        public void MuteSound()=>_listeners.ForEach(listener => listener.mute = true);
        public void UnmuteSound()=>_listeners.ForEach(listener => listener.mute = false);
        
        
        public void MuteMusic() => _musicListener.mute = true;
        public void UnmuteMusic()=> _musicListener.mute = false;
    }
}