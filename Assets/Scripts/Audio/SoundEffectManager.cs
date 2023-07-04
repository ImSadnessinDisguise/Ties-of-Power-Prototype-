using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonoBehaviour<SoundEffectManager>
{
    public int soundVolume = 8;

    private void Start()
    {
        SetSoundsVolume(soundVolume); 
    }

    //play sound effect
    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        //Play sound using a gameobject and component from the object pool
        SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, Vector3.zero, Quaternion.identity);
        sound.SetSound(soundEffect);
        sound.gameObject.SetActive(true);
        StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
    }

    private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }

    private void SetSoundsVolume(int soundVolume)
    {
        float muteDecibels = -80f;

        if (soundVolume == 0)
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibels(soundVolume));
        }
    }
}
