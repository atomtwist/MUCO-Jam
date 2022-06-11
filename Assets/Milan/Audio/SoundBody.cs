using System;
using System.Linq;
using atomtwist.AudioNodes;
using Effects;
using UnityEngine;
using UnityEngine.Audio;
using Random = System.Random;

[RequireComponent(typeof(Rigidbody))]
public class SoundBody : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Seconds before another collision event fires")]
    [Range(0, 5)]
    private float timeout = 1f;

    [SerializeField] private AudioMixerGroup _mixerGroup;
    
    [SerializeField] 
    [Range(0,1)] private float _spatialBlend = .8f;

    [SerializeField]
    private AudioClip[] audioClips = Array.Empty<AudioClip>();

    [SerializeField]
    [Tooltip("Minimum impact force before playing a sound")]
    private float volumeThreshold = 1f;
    
    [SerializeField]
    [Tooltip("Lower values makes sound louder on softer collisions")]
    private float volumeSensitivity = 2f;
    
    [SerializeField]
    private AnimationCurve volumeFalloff = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float m_lastCollisionTime;
    private readonly Random m_random = new Random();

    private SoundsSettings _soundsSettings;

    private void OnEnable()
    {
        if (audioClips.Length == 0)
        {
            Debug.LogError("No audio clips on SoundBody! Disabling component");
            enabled = false;
        }
        
        //set up sounds for playback
        _soundsSettings = new SoundsSettings();
        _soundsSettings.audioClips = audioClips.ToList();
        _soundsSettings.spatialized = true;
        _soundsSettings.spatialBlend = _spatialBlend;
        _soundsSettings.playAtTransform = transform;
        _soundsSettings.mixerGroup = _mixerGroup;
        _soundsSettings.soundStyle = SoundStyle.Random;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time < m_lastCollisionTime + timeout)
        {
            return;
        }

        m_lastCollisionTime = Time.time;

        var collisionForce = Vector3.Magnitude(collision.relativeVelocity);
        if (collisionForce < volumeThreshold)
        {
            return;
        }
        
        var volume = volumeSensitivity <= volumeThreshold 
            ? 1f 
            : volumeFalloff.Evaluate(collisionForce / (volumeSensitivity - volumeThreshold));
        
        //audioSource.PlayOneShot(audioClips[m_random.Next(audioClips.Length)], volume);

        _soundsSettings.volume = volume;
        _soundsSettings.spatialBlend = _spatialBlend;
        SoundSystem.Instance.Play(_soundsSettings);

        //Pouf
        foreach (var contact in collision.contacts)
        {
            var impactDirection = contact.normal;
            var poofPos = contact.point;

            HitEffects.SpawnEffect(poofPos, impactDirection, EffectType.HeadPouf, 0.1f);
        }
        
       
    }
    

    private void OnValidate()
    {
        if (volumeThreshold < 0f)
            volumeThreshold = 0f;

        if (volumeSensitivity < 0f)
            volumeSensitivity = 0f;
    }
}
