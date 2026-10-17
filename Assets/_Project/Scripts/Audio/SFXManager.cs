using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] private AudioSource sfxObject;


    private void Awake()
    {
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
    }

    public void PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLenght = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLenght);
    }

    public void PlayRandomSFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, audioClips.Length);

        PlaySFXClip(audioClips[rand], spawnTransform, volume);
    }
}
