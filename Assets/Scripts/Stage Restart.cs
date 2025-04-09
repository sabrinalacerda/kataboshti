using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas

public class StageRestart : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto colidido é a bola
        if (collision.gameObject.CompareTag("Player")) // Certifique-se de que a bola tem a tag "Player"
        {
            RestartStage();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que atravessou o trigger é a bola
        if (other.gameObject.CompareTag("Player")) // Certifique-se de que a bola tem a tag "Player"
        {
            RestartStage();
        }
    }

    private void RestartStage()
    {
        // Reinicia a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
