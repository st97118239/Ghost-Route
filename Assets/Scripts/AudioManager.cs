using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] private AudioSource voicelineSource;

    private void Awake()
    {
        instance = this;
        if (voicelineSource)
            voicelineSource.volume = SaveData.voicelinesVolume;
    }

    public static void PlayVoiceline(AudioClip audioClip)
    {
        if (instance.voicelineSource.isPlaying)
            instance.voicelineSource.Stop();

        instance.voicelineSource.clip = audioClip;
        instance.voicelineSource.Play();
    }
}
