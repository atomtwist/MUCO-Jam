using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using atomtwist.AudioNodes;
using ToyBoxHHH;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace atomtwist.AudioNodes
{
    
    [ExecuteAlways]
    public class SoundSystem : Singleton<SoundSystem>
    {
        private static int CounterInt;
        public static int GetUniqueInt
        {
            get { return CounterInt++; }
        }

        public int count = 32;
        public GameObject soundInstancePrefab;
        
        //make this a dic with the ids
        public List<SoundInstance> _playingInstances = new List<SoundInstance>();
        
        private Stack<SoundInstance> pool;
        private List<SoundInstance> active;
        private Transform _inactive, _active;
        
        
        void FillPool()
        {
            //these are for debug in hierarchy
            if(_inactive == null)
            {
                _inactive = new GameObject("Inactive").transform;
                _inactive.SetParent(transform);
            }
            if(_active == null)
            {
                _active = new GameObject("Active").transform;
                _active.SetParent(transform);
            }

            pool = new Stack<SoundInstance>(count);
            active = new List<SoundInstance>(count);
            for (int i = 0; i < count; i++)
            {
                SoundInstance soundInstance = Instantiate(soundInstancePrefab).GetComponent<SoundInstance>();
                soundInstance.gameObject.SetActive(false);
                soundInstance.transform.SetParent(_inactive);
                pool.Push(soundInstance);
            }
        }

        void Awake()
        {
            if(Application.isPlaying)
            {
                DontDestroyOnLoad(this.gameObject);
            }
            ClearPool();   
        }

        void Reset()
        {
            ClearPool();
        }

        void ClearPool()
        {
            foreach (Transform child in transform)
            {
#if UNITY_EDITOR
                DestroyImmediate(child.gameObject);   
#else
            Destroy(child.gameObject);
#endif
            }
            
            _playingInstances.Clear();
        }

        private void OnEnable()
        {
            ClearPool();
            FillPool();
        }

        private void OnDisable()
        {
            ClearPool();
        }

        private void OnDestroy()
        {
            ClearPool();
        }

        SoundInstance GetFromPool()
        {
            var instance = Instantiate(soundInstancePrefab).GetComponent<SoundInstance>();
            instance.transform.SetParent(transform);
            return instance;
            
            //TODO: fix the pooling on runtime bug
            //pooling
            /*if(pool.Count == 0)
                return null;
            var instance = pool.Pop();
            instance.gameObject.SetActive(true);
            active.Add(instance);
            instance.transform.SetParent(_active);
            return instance;*/
        }

        void ReturnToPool(SoundInstance playingInstance)
        {
            playingInstance.Stop();
            _playingInstances.Remove(playingInstance);
#if UNITY_EDITOR
            DestroyImmediate(playingInstance.gameObject);   
#else
            Destroy(playingInstance.gameObject);
#endif
            
            //pooling
            /*if(active.Count == 0)
                return;
            playingInstance.gameObject.SetActive(false);
            active.Remove(playingInstance);
            pool.Push(playingInstance);
            playingInstance.transform.SetParent(_inactive);*/
        }
        
        void CheckIfPlaybackDone()
        {
            if(_playingInstances.Count == 0) return;
            //remove all null in case they got deleted manually
            _playingInstances.RemoveAll(item => item == null);
            
            //
            for (int i = 0; i < _playingInstances.Count; i++)
            {
                var instance = _playingInstances[i];
                if (instance.IsPlaying == false)
                {
                    ReturnToPool(instance);
                }
            }
        }

        void UpdateTransforms()
        {
            if(_playingInstances.Count == 0) return;
            foreach (var playingInstance in _playingInstances)
            {
                if(playingInstance.soundSettings.playAtTransform != null)
                    playingInstance.transform.position = playingInstance.soundSettings.playAtTransform.position;
            }
        }

        private void Update()
        {
            UpdateTransforms();
            CheckIfPlaybackDone();
        }

        void PlayASingleSound(SoundsSettings soundSettings, int id, AudioClip clipToPlay)
        {
            var instance = GetFromPool();
            instance.gameObject.name = id.ToString();
            instance.Play(soundSettings, id,clipToPlay);
            _playingInstances.Add(instance);
        }
        
        /// <summary>
        /// use this to play a sound from a position. the sound will stay at that position. use UpdatePosition to do it manually, or use a transform.
        /// </summary>
        /// <param name="clips"></param>
        /// <param name="playAtPosition"></param>
        /// <param name="style"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <param name="spatialized"></param>
        /// <param name="spatialBlend"></param>
        /// <param name="doppler"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public int Play(List<AudioClip> clips,Vector3 playAtPosition,SoundStyle style = SoundStyle.Single, bool spatialized = true, float spatialBlend = 1, AudioMixerGroup mixerGroup = null, float doppler = 0, bool loop = false,float volume = 1,float pitch = 1, double dspDelay = 0)
        {
            SoundsSettings ss = new SoundsSettings();
            ss.audioClips = clips;
            ss.positionToPlayAt = playAtPosition;
            ss.volume = volume;
            ss.pitch = pitch;
            ss.spatialized = spatialized;
            ss.spatialBlend = spatialBlend;
            ss.dopplerLevel = doppler;
            ss.loop = loop;
            ss.mixerGroup = mixerGroup;
            ss.delay = dspDelay;

            return Play(ss);
        }

        /// <summary>
        /// use this to play a sound from a transform. the sound will move with that transform
        /// </summary>
        /// <param name="clips"></param>
        /// <param name="playAtTransform"></param>
        /// <param name="playAtPosition"></param>
        /// <param name="style"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <param name="spatialized"></param>
        /// <param name="spatialBlend"></param>
        /// <param name="doppler"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public int Play(List<AudioClip> clips,Transform playAtTransform = null,SoundStyle style = SoundStyle.Single, bool spatialized = true, float spatialBlend = 1, AudioMixerGroup mixerGroup = null,float doppler = 0, bool loop = false, float volume = 1,float pitch =1, double dspDelay = 0 )
        {
            SoundsSettings ss = new SoundsSettings();
            ss.audioClips = clips;
            ss.playAtTransform = playAtTransform;
            ss.volume = volume;
            ss.pitch = pitch;
            ss.spatialized = spatialized;
            ss.spatialBlend = spatialBlend;
            ss.dopplerLevel = doppler;
            ss.loop = loop;
            ss.mixerGroup = mixerGroup;
            ss.delay = dspDelay;


            return Play(ss);
        }
        
        /// <summary>
        /// use this to play a sound from a transform. the sound will move with that transform
        /// </summary>
        /// <param name="clips"></param>
        /// <param name="playAtTransform"></param>
        /// <param name="playAtPosition"></param>
        /// <param name="style"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <param name="spatialized"></param>
        /// <param name="spatialBlend"></param>
        /// <param name="doppler"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public int Play(AudioClip clip,Transform playAtTransform = null, bool spatialized = true, float spatialBlend = 1, AudioMixerGroup mixerGroup = null,float doppler = 0, bool loop = false, float volume = 1,float pitch =1, double dspDelay = 0 )
        {
            SoundsSettings ss = new SoundsSettings();
            ss.audioClips.Add(clip);
            ss.playAtTransform = playAtTransform;
            ss.volume = volume;
            ss.pitch = pitch;
            ss.spatialized = spatialized;
            ss.spatialBlend = spatialBlend;
            ss.dopplerLevel = doppler;
            ss.loop = loop;
            ss.mixerGroup = mixerGroup;
            ss.delay = dspDelay;


            return Play(ss);
        }
        
        /// <summary>
        /// use this to play a sound from a position. the sound will stay at that position. use UpdatePosition to do it manually, or use a transform.
        /// </summary>
        /// <param name="clips"></param>
        /// <param name="playAtPosition"></param>
        /// <param name="style"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <param name="spatialized"></param>
        /// <param name="spatialBlend"></param>
        /// <param name="doppler"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public int Play(AudioClip clip,Vector3 playAtPosition, bool spatialized = true, float spatialBlend = 1, AudioMixerGroup mixerGroup = null, float doppler = 0, bool loop = false,float volume = 1,float pitch = 1, double dspDelay = 0)
        {
            SoundsSettings ss = new SoundsSettings();
            ss.audioClips.Add(clip);
            ss.positionToPlayAt = playAtPosition;
            ss.volume = volume;
            ss.pitch = pitch;
            ss.spatialized = spatialized;
            ss.spatialBlend = spatialBlend;
            ss.dopplerLevel = doppler;
            ss.loop = loop;
            ss.mixerGroup = mixerGroup;
            ss.delay = dspDelay;

            return Play(ss);
        }
        
        // WE NEED A WAY TO IDENTIFY THE ID OF THE TRACK THAT PLAYS THIS SOUND.
        //      consider: multiple tracks can play the same clip, transform, etc, so we need a specific track id. but the track references in the timeline might also change when pausing/unpausing becasue of the Unity weird Timeline API....??!??!
        public int Play(SoundsSettings soundSettings)
        {
            if (soundSettings.audioClips.Count == 0)
                return -1;
            var id = GetUniqueInt;
            //Debug.Log("Playing sound " + id);

            switch (soundSettings.soundStyle)
            {
                case SoundStyle.Single:
                {
                    PlayASingleSound(soundSettings,id,soundSettings.audioClips[0]);
                    break;
                }
                case SoundStyle.Multi:
                {
                    for (int i = 0; i < soundSettings.audioClips.Count; i++)
                    {
                        PlayASingleSound(soundSettings,id,soundSettings.audioClips[i]);
                    }
                    break;
                }
                case SoundStyle.Random:
                {
                    var rnd = Random.Range(0, soundSettings.audioClips.Count);
                    PlayASingleSound(soundSettings,id,soundSettings.audioClips[rnd]);
                    break;
                }
                case SoundStyle.Sequence:
                {
                    PlayASingleSound(soundSettings,id,soundSettings.audioClips[soundSettings.SequenceState]);
                    break;
                }
                
            }
            
            return id;
        }

        [DebugButton]
        public void StopAll()
        {
            if (_playingInstances.Count != 0)
            {
                foreach (var instance in _playingInstances)
                {
                    instance.Stop();
                }
            }
        }
        
        public void Stop(int soundID)
        {
            if(soundID == -1) return;
            //Debug.Log("Stopping sound " + soundID);
            if (_playingInstances.Count != 0)
            {
                foreach (var instance in _playingInstances)
                {
                    if (instance.soundId == soundID)
                    {
                        instance.Stop();
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        public void SetVolume(int soundID, float clipVolume)
        {
           // find the clip with the ID, set the volume. 
            if (_playingInstances.Count != 0)
            {
                foreach (var instance in _playingInstances)
                {
                    if (instance.soundId == soundID)
                    {
                        instance.SetVolume(clipVolume);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        
        public void SetPitch(int soundID, float clipPitch)
        {
            // find the clip with the ID, set the volume. 
            if (_playingInstances.Count != 0)
            {
                foreach (var instance in _playingInstances)
                {
                    if (instance.soundId == soundID)
                    {
                        instance.SetPitch(clipPitch);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        
        public AudioSource GetPlayingSource(int soundID)
        {
            // find the clip with the ID, set the volume. 
            if (_playingInstances.Count != 0)
            {
                foreach (var instance in _playingInstances)
                {
                    if (instance.soundId == soundID)
                    {
                        return instance.audioSource;
                    }
                }
            }

            return null;
        }
        
        
    }

    public enum SoundStyle
    {
        Single, Random, Sequence, Multi
    }
    
    [Serializable]
    public class SoundsSettings
    {

        public SoundsSettings Clone()
        {
            SoundsSettings clone = new SoundsSettings();
            clone.delay = this.delay;
            clone.loop = this.loop;
            clone.octave = this.octave;
            clone.pitch = this.pitch;
            clone.spatialized = this.spatialized;
            clone.transpose = this.transpose;
            clone.volume = this.volume;
            clone.audioClips = this.audioClips;
            clone.dopplerLevel = this.dopplerLevel;
            clone.midiNote = this.midiNote;
            clone.mixerGroup = this.mixerGroup;
            clone.sequenceState = this.sequenceState;
            clone.soundStyle = this.soundStyle;
            clone.spatialBlend = this.spatialBlend;
            clone.playAtTransform = this.playAtTransform;
            clone.positionToPlayAt = this.positionToPlayAt;

            return clone;
        }
        
        
        
        
        //to keep track internally when doing sequential playback
        private int sequenceState;
        public int SequenceState {
            get
            {
                var state = sequenceState;
                sequenceState++;
                if (sequenceState >= audioClips.Count)
                    sequenceState = 0;
                return state;
            }

        }
        public SoundStyle soundStyle; 
        public List<AudioClip> audioClips = new List<AudioClip>();
        public AudioMixerGroup mixerGroup;
        public Transform playAtTransform;
        public Vector3 positionToPlayAt;
        [Range(0, 1)] public float volume = 1;
        [Range(-2, 2)] public float pitch = 1;
        public int midiNote = 72; //C5
        [Range(-12,12)]
        public int transpose = 0;
        [Range(-5, 5)] public int octave = 0;

        public bool spatialized = false;
        [Range(0, 1)] public float spatialBlend = 0;

        public bool loop = false;

        [Range(0, 5f)] public float dopplerLevel = 0f;

        [Range(0, 3)]
        public double delay = 0;
    }
}