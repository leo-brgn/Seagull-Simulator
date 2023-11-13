using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip LandingAudioClip;
    public AudioClip JumpAudioClip;
    public AudioClip WingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.1f;

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
        }
    }
    
    private void OnWingflap(AnimationEvent animationEvent)
    {
        AudioSource.PlayClipAtPoint(WingAudioClip, transform.position, FootstepAudioVolume);
    }

    
}
