using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace atomtwist.AudioNodes
{
    [RequireComponent(typeof(AudioSource))]
    [ExecuteInEditMode]
    public class SoundInstance : MonoBehaviour
    {
        public AudioSource audioSource;
        public SoundsSettings soundSettings;
        public int soundId;
        
        public bool IsPlaying
        {
            get { return audioSource.isPlaying; }
        }

        public float KeyToPitch(int midiKey, int transpose, int octave)
        {
            int c4Key = midiKey - 72;
            //apply transpose & octave
            c4Key += transpose;
            c4Key += octave * 12;
            float pitch = Mathf.Pow(2, c4Key / 12f);
            return pitch;
        }

        void ApplySettings(SoundsSettings soundsSettings)
        {
            this.soundSettings = soundsSettings;
            audioSource.outputAudioMixerGroup = this.soundSettings.mixerGroup;
            if(this.soundSettings.playAtTransform != null)
                transform.position = this.soundSettings.playAtTransform.position;
            if (this.soundSettings.playAtTransform == null)
                transform.position = this.soundSettings.positionToPlayAt;
            audioSource.volume = this.soundSettings.volume;
            audioSource.pitch = this.soundSettings.pitch * KeyToPitch(this.soundSettings.midiNote, this.soundSettings.transpose, this.soundSettings.octave);
            audioSource.spatialize = this.soundSettings.spatialized;
            audioSource.spatialBlend = this.soundSettings.spatialBlend;
            audioSource.loop = this.soundSettings.loop;
            audioSource.dopplerLevel = this.soundSettings.dopplerLevel;
        }

        public void Play(SoundsSettings soundSettings,int id, AudioClip clipToPlay)
        {
            soundId = id;
            ApplySettings(soundSettings);
            audioSource.clip = clipToPlay;
            if (this.soundSettings.delay != 0)
                audioSource.PlayScheduled(AudioSettings.dspTime + this.soundSettings.delay);
            else if (this.soundSettings.delay == 0)
                audioSource.Play();
        }

        public void Stop()
        {
            //Debug.Log("Instance stopped: "+soundId);
            audioSource.Stop();
        }

        public void SetVolume(float volume)
        {
            audioSource.volume = volume;
        }
        
        public void SetPitch(float pitch)
        {
            audioSource.pitch = pitch;
        }
    }
}