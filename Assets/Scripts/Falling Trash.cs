using UnityEngine;

public class FallingTrash : MonoBehaviour
{
    public float fallSpeed = 5f; // Velocidade de queda
    private bool hasHitGround = false; // Verifica se o objeto já colidiu com o chão
    public AudioClip spawnSound; // Som que será tocado quando o objeto surgir
    private AudioSource audioSource; // Referência ao componente AudioSource

    void Start()
    {
        // Adiciona o componente AudioSource ao objeto
        audioSource = gameObject.AddComponent<AudioSource>();

        // Configura o som de spawn
        if (spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
            audioSource.pitch = 2.0f; // Aumenta a velocidade do som
        }
    }

    void Update()
    {
        // Se o objeto ainda não colidiu com o chão, ele continua caindo
        if (!hasHitGround)
        {
            // Move o objeto para baixo
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            // Verifica se o objeto colidiu com o chão
            if (IsOnGround())
            {
                hasHitGround = true; // Marca que o objeto atingiu o chão
            }
        }
    }

    // Verifica se o objeto está no chão
    private bool IsOnGround()
    {
        // Faz um raycast para verificar se há um chão logo abaixo do objeto
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }
}