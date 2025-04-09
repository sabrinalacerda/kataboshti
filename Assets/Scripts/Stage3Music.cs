using UnityEngine;

public class Stage3Music : MonoBehaviour
{
    public AudioClip stage3Music; // Música do estágio 3
    private AudioSource audioSource; // Referência ao componente AudioSource

    void Start()
    {
        // Adiciona o componente AudioSource ao GameObject
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Detecta quando o jogador entra no Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Verifica se o objeto que entrou no Trigger tem a tag "Player"
        {
            Debug.Log("Player entrou no Trigger!");
            // Toca a música do estágio 3
            audioSource.clip = stage3Music;
            audioSource.loop = true; // Faz a música tocar em loop
            audioSource.Play();
        }
    }
}
