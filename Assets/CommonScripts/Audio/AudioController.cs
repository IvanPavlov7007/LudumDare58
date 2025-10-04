using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Pixelplacement;

//Based on Daniel Mullins' one
public class AudioController : Singleton<AudioController>
{
    private List<AudioClip> sfx = new List<AudioClip>();
    private List<AudioClip> loops = new List<AudioClip>();
    private AudioMixer currentAudioMixer;
    private List<AudioSource> ActiveSFXSources
    {
        get
        {
            activeSFX.RemoveAll(x => x == null || ReferenceEquals(x, null));
            return activeSFX;
        }
    }
    private List<AudioSource> activeSFX = new List<AudioSource>();

    private Dictionary<string, float> limitedFrequencySounds = new Dictionary<string, float>();
    private Dictionary<string, int> lastPlayedSounds = new Dictionary<string, int>();

    private float DEFAULT_SPATIAL_BLEND = 0.75f;

    private void Awake()
    {
        foreach (object o in Resources.LoadAll("Audio/SFX"))
        {
            sfx.Add((AudioClip)o);
        }
        foreach (object o in Resources.LoadAll("Audio/Loops"))
        {
            loops.Add((AudioClip)o);
        }

        currentAudioMixer = Resources.Load<AudioMixer>("Audio/AudioMixer");
    }

    public void SetMasterVolume(float volume)
    {
        currentAudioMixer.SetFloat("Volume",getDecibels(volume));
    }

    public float GetMasterVolume()
    {
        float dbs;
        if (currentAudioMixer.GetFloat("Volume", out dbs))
        {
            return getlerpedVolume(dbs);
        }

        throw new UnityException("Audio Mixer doesn't have a Volume exposure value");
    }

    float getDecibels(float t)
    {
        return (Mathf.Log10(Mathf.Clamp(t, 0.0001f, 1f)) * 20f);
    }

    float getlerpedVolume(float dbVolume)
    {
        return Mathf.Pow(10f, dbVolume / 20f);
    }

    public AudioClip GetLoopClip(string loopId)
    {
        return loops.Find(x => x.name.ToLowerInvariant() == loopId.ToLowerInvariant());
    }

    public AudioClip GetAudioClip(string soundId)
    {
        return sfx.Find(x => x.name.ToLowerInvariant() == soundId.ToLowerInvariant());
    }

    private AudioSource CreateAudioSourceForSound(string soundId, Vector3 position, bool looping)
    {
        if (!string.IsNullOrEmpty(soundId))
        {
            AudioClip sound = GetAudioClip(soundId);

            if (sound != null)
            {
                return InstantiateAudioObject(sound, position, looping);
            }
        }

        return null;
    }

    private AudioSource InstantiateAudioObject(AudioClip clip, Vector3 pos, bool looping)
    {
        GameObject tempGO = new GameObject("Audio_" + clip.name);
        tempGO.transform.position = pos;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.outputAudioMixerGroup = currentAudioMixer.FindMatchingGroups(looping? "Music" : "FX")[0];
        aSource.spatialBlend = DEFAULT_SPATIAL_BLEND;

        aSource.Play();
        if (looping)
        {
            aSource.loop = true;
        }
        else
        {
            Destroy(tempGO, clip.length * 3f);
        }
        return aSource;
    }

    public AudioSource PlaySoundFlat(string soundId, float volume = 1f, float skipToTime = 0f, AudioParams.Pitch pitch = null,
        AudioParams.Repetition repetition = null, AudioParams.Randomization randomization = null, AudioParams.Distortion distortion = null, bool looping = false)
    {
        var source = PlaySound3D(soundId, Vector3.zero, volume, skipToTime, pitch, repetition, randomization, distortion, looping);

        if(source != null)
        {
            source.spatialBlend = 0f;
            DontDestroyOnLoad(source.gameObject);
        }
        return source;
    }

    public AudioSource PlaySound3D(string soundId, Vector3 position, float volume = 1f, float skipToTime = 0f, AudioParams.Pitch pitch = null,
        AudioParams.Repetition repetition = null, AudioParams.Randomization randomization = null, AudioParams.Distortion distortion = null, bool looping = false)
    {
        if (repetition != null)
        {
            if (RepetitionIsTooFrequent(soundId, repetition.minRepetitionFrequency, repetition.entryId))
            {
                return null;
            }
        }

        string randomVariationId = soundId;
        if (randomization != null)
        {
            randomVariationId = GetRandomVariationOfSound(soundId, randomization.noRepeating);
        }

        var source = CreateAudioSourceForSound(randomVariationId, position, looping);
        if (source != null)
        {
            source.volume = volume;
            source.time = source.clip.length * skipToTime;

            if (pitch != null)
            {
                source.pitch = pitch.pitch;
            }

            if (distortion != null)
            {
                if (distortion.muffled)
                {
                    MuffleSource(source);
                }
            }
        }

        activeSFX.Add(source);
        return source;
    }

    private bool RepetitionIsTooFrequent(string soundId, float frequencyMin, string entrySuffix = "")
    {
        float time = Time.unscaledTime;
        string soundKey = soundId + entrySuffix;

        if (limitedFrequencySounds.ContainsKey(soundKey))
        {
            if (time - frequencyMin > limitedFrequencySounds[soundKey])
            {
                limitedFrequencySounds[soundKey] = time;
                return false;
            }
        }
        else
        {
            limitedFrequencySounds.Add(soundKey, time);
            return false;
        }

        return true;
    }

    private string GetRandomVariationOfSound(string soundPrefix, bool noRepeating)
    {
        string soundId = "";

        if (!string.IsNullOrEmpty(soundPrefix))
        {
            List<AudioClip> variations = sfx.FindAll(x => x != null && x.name.ToLowerInvariant().StartsWith(soundPrefix.ToLowerInvariant()));

            if (variations.Count > 0)
            {
                int index = Random.Range(0, variations.Count) + 1;
                if (noRepeating) // repeating in 2 consequent draws, otherwise not controlled
                {
                    if (!lastPlayedSounds.ContainsKey(soundPrefix))
                    {
                        lastPlayedSounds.Add(soundPrefix, index);
                    }
                    else
                    {
                        int breakOutCounter = 0;
                        const int BREAK_OUT_THRESHOLD = 100;
                        while (lastPlayedSounds[soundPrefix] == index && breakOutCounter < BREAK_OUT_THRESHOLD)
                        {
                            index = Random.Range(0, variations.Count) + 1;
                            breakOutCounter++;
                        }

                        if (breakOutCounter >= BREAK_OUT_THRESHOLD - 1)
                        {
                            Debug.Log("Broke out of infinite loop! AudioController.PlayRandomSound.");
                        }

                        lastPlayedSounds[soundPrefix] = index;
                    }
                }

                soundId = soundPrefix + index;
            }
            else
            {
                soundId = soundPrefix;
            }
        }

        return soundId;
    }

    private void MuffleSource(AudioSource source, float cutoff = 300f)
    {
        var filter = source.gameObject.AddComponent<AudioLowPassFilter>();
        filter.cutoffFrequency = cutoff;
    }
}

public class AudioParams
{
    [System.Serializable]
    public enum Variation
    {
        VerySmall,
        Small,
        Medium,
        Large
    }
    [System.Serializable]
    public class Pitch
    {
       
        public float pitch;

        public Pitch(float value)
        {
            pitch = value;
        }

        public Pitch(Variation randomVariation)
        {
            switch (randomVariation)
            {
                case Variation.VerySmall:
                    pitch = Random.Range(0.95f, 1.05f);
                    break;
                case Variation.Small:
                    pitch = Random.Range(0.9f, 1.1f);
                    break;
                case Variation.Medium:
                    pitch = Random.Range(0.75f, 1.25f);
                    break;
                case Variation.Large:
                    pitch = Random.Range(0.5f, 1.5f);
                    break;
            }
        }
    }

    [System.Serializable]
    public class Repetition
    {
        public float minRepetitionFrequency;
        public string entryId;

        public Repetition(float minRepetitionFrequency, string entryId = "")
        {
            this.minRepetitionFrequency = minRepetitionFrequency;
            this.entryId = entryId;
        }
    }
    [System.Serializable]
    public class Randomization
    {
        public bool noRepeating;

        public Randomization(bool noRepeating = true)
        {
            this.noRepeating = noRepeating;
        }
    }

    [System.Serializable]
    public class Distortion
    {
        public bool muffled;
    }

}