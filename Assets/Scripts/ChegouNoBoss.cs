using UnityEngine;

public class ChegouNoBoss : MonoBehaviour
{
    public GameObject bossDoor; // Referência à porta do boss
    private bool hasTriggered = false; // Verifica se a ação já foi executada

    private Animator bossDoorAnimator; // Referência ao Animator da porta do boss
    private BoxCollider bossDoorCollider; // Referência ao BoxCollider da porta do boss

    void Start()
    {
        if (bossDoor == null)
        {
            Debug.LogError("A variável 'bossDoor' não foi configurada no Inspector!");
            return;
        }

        // Obtém o Animator da porta do boss
        bossDoorAnimator = bossDoor.GetComponent<Animator>();
        if (bossDoorAnimator != null)
        {
            // Pausa a animação no frame 0
            bossDoorAnimator.enabled = false; // Desativa o Animator para impedir que a animação toque
            Debug.Log("Animator da porta do boss desativado no início do jogo.");
        }
        else
        {
            Debug.LogError("Animator não encontrado na porta do boss!");
        }

        // Obtém o BoxCollider da porta do boss
        bossDoorCollider = bossDoor.GetComponent<BoxCollider>();
        if (bossDoorCollider != null)
        {
            bossDoorCollider.enabled = false; // Desativa o BoxCollider da porta do boss
            Debug.Log("BoxCollider da porta do boss desativado.");
        }
        else
        {
            Debug.LogError("BoxCollider não encontrado na porta do boss!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o jogador colidiu com este objeto e se a ação ainda não foi executada
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true; // Marca como acionado
            Debug.Log("Jogador chegou ao boss!");

            // Ativa o BoxCollider da porta do boss
            if (bossDoorCollider != null)
            {
                bossDoorCollider.enabled = true;
                Debug.Log("BoxCollider da porta do boss ativado.");
            }

            // Ativa o Animator da porta do boss para tocar a animação
            if (bossDoorAnimator != null)
            {
                bossDoorAnimator.enabled = true; // Reativa o Animator
                bossDoorAnimator.SetTrigger("Abrir"); // Define o trigger "Abrir" para iniciar a animação
                Debug.Log("Animação da porta do boss ativada.");
            }
        }
    }
}
