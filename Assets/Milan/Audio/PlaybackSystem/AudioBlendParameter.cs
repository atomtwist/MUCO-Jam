using System;
using System.Collections;
using System.Collections.Generic;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace atomtwist.AudioNodes
{
    [RequireComponent(typeof(PlayableDirector))]
    public class AudioBlendParameter : MonoBehaviour
    {
        public PlayableAsset blendSound;
        public bool PlayOnAwake;
        private PlayableDirector director;
        

        [Range(0, 1)] public double blend;

        private bool isPlaying;

        
        void OnEnable()
        {
            if (director == null) director = GetComponent<PlayableDirector>();
            if(PlayOnAwake) isPlaying = true;
            if(director.playableAsset != blendSound)
                director.playableAsset = blendSound;
        }

        void Reset()
        {
            GetComponent<PlayableDirector>().playOnAwake = false;
        }
        
        [DebugButton]
        void Play()
        {
            director.Play();
            director.Evaluate();
            isPlaying = true;
            UpdatePlaybackState();
        }

        [DebugButton]
        void Stop()
        {
            director.Stop();
            isPlaying = false;
        }

        void UpdatePlaybackState()
        {
            if(director!= null)
            {
                director.playOnAwake = false;
                if(director.playableAsset != blendSound)
                    director.playableAsset = blendSound;
            }
            
            if (isPlaying == true)
            {
                var newTime = director.duration * blend;
                director.time = newTime;
                if (!director.playableGraph.IsValid())
                {
                    director.RebuildGraph();
                    director.Play();
                }
                director.playableGraph.GetRootPlayable(0).SetSpeed(0);
                
            }
        }
        
        private void Update()
        {
           UpdatePlaybackState();
        }
    }
}


