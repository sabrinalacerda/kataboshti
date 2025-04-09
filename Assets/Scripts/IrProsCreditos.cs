using UnityEngine;
using UnityEngine.SceneManagement;

public class IrProsCreditos : MonoBehaviour
{
    public float delayBeforeSceneChange = 3f; // Tempo de espera antes de trocar de cena
    private SceneFader sceneFader; // Referência ao SceneFader

    void Start()
    {
        // Obtém a referência ao SceneFader na cena
        sceneFader = FindObjectOfType<SceneFader>();
        if (sceneFader == null)
        {
            Debug.LogError("SceneFader não encontrado na cena!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Verifica se o jogador colidiu
        {
            // Ativa o fade-in
            if (sceneFader != null)
            {
                sceneFader.FadeToScene();
            }

            // Inicia a troca de cena após o delay
            StartCoroutine(ChangeToLastSceneAfterDelay());
        }
    }

    private System.Collections.IEnumerator ChangeToLastSceneAfterDelay()
    {
        // Aguarda o tempo configurado antes de trocar de cena
        yield return new WaitForSeconds(delayBeforeSceneChange);

        // Carrega a última cena no Build Settings
        int lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
        if (lastSceneIndex >= 0)
        {
            SceneManager.LoadScene(lastSceneIndex);
        }
        else
        {
            Debug.LogError("Nenhuma cena válida encontrada no Build Settings!");
        }
    }
}
