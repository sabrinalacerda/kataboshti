using UnityEngine;
using UnityEngine.SceneManagement;

public class ComecaJogo : MonoBehaviour
{
    public float delayBeforeSceneChange = 3f; // Tempo em segundos antes de trocar de cena
    private bool hasTriggered = false; // Verifica se a troca de cena já foi acionada
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

    void Update()
    {
        // Verifica se qualquer tecla ou botão do mouse foi pressionado pela primeira vez
        if (!hasTriggered && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            hasTriggered = true; // Marca como acionado
            Debug.Log("Entrada detectada. Iniciando fade-in...");
            StartCoroutine(StartFade());
        }
    }

    private System.Collections.IEnumerator StartFade()
    {
        // Inicia o fade-in
        if (sceneFader != null)
        {
            sceneFader.FadeToScene(); // Chama o método sem argumentos
        }
        else
        {
            Debug.LogError("SceneFader não está configurado corretamente!");
        }

        // Aguarda o tempo configurado antes de continuar
        yield return new WaitForSeconds(delayBeforeSceneChange);

        // Troca para a cena 1
        SceneManager.LoadScene(1); // Certifique-se de que a cena 1 está adicionada no Build Settings
    }
}
