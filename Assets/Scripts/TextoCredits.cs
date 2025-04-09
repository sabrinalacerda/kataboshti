using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Necessário para carregar cenas

public class CreditsScroll : MonoBehaviour
{
    public RectTransform contentTransform; // Referência ao transform do conteúdo dos créditos
    public float scrollSpeed = 20f; // Velocidade de rolagem
    public float endPositionY = 1000f; // Posição Y onde o texto deve parar de rolar
    public SceneFader sceneFader; // Referência ao SceneFader

    void Start()
    {
        StartCoroutine(ScrollCredits());
    }

    IEnumerator ScrollCredits()
    {
        while (contentTransform.anchoredPosition.y < endPositionY) // Continua rolando até atingir a posição final
        {
            // Incrementa a posição vertical do texto
            contentTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

            // Aguarda o próximo frame
            yield return null;
        }

        // Quando o texto atingir a posição final, ativa o fade-in
        if (sceneFader != null)
        {
            sceneFader.FadeToScene(); // Ativa o fade-in
            yield return new WaitForSeconds(2f); // Aguarda 2 segundos
            SceneManager.LoadScene(0); // Troca para a cena 0 (menu)
        }
        else
        {
            Debug.LogError("SceneFader não foi atribuído no Inspector!");
        }
    }
}
