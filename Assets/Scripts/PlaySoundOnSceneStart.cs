using UnityEngine;

public class PlaySoundOnSceneStart : MonoBehaviour
{
    public AudioClip soundClip; // arrasta o som aqui pelo Inspector
    private AudioSource audioSource;

    void Start()
    {
        // Cria um AudioSource se não tiver
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        audioSource.spatialBlend = 0f; // 0 = som 2D, 1 = som 3D
        audioSource.Play();
    }
}
