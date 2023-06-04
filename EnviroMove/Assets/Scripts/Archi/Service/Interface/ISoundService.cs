using UnityEngine;

namespace Archi.Service.Interface
{
    public interface IAudioService : IService
    {
        void StartSound(AudioClip clip, bool loop = false);
        void StartSoundWithPitch(AudioClip clip, float pitchForce);
        void PlaySound(AudioClip clip);
        void PauseSound(AudioClip clip);
        void StopSound(AudioClip clipToStop);
        
        void PlaySounds();
        void PauseSounds();
        void StopSounds();

        bool IsPlaying(AudioClip clip);
        
        void StartMusic(AudioClip music);
        void PlayMusic();
        void PauseMusic();
        void StopMusic();

        void MuteSound();
        void MuteMusic();
        void UnmuteSound();
        void UnmuteMusic();
    }
}