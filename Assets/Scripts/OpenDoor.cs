using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject door; // Referência ao objeto porta
    public int requiredTrashCount = 5; // Quantidade de lixos necessária para abrir a porta (configurável no Inspector)
    private Animator doorAnimator; // Referência ao Animator da porta
    private BallControl ballControlScript; // Referência ao script BallControl do jogador
    private BoxCollider doorCollider; // Referência ao BoxCollider da porta
    private bool isOpened = false; // Indica se a porta já foi aberta
    public AudioClip doorOpenSound; // Som que será tocado ao abrir a porta
    private AudioSource audioSource; // Referência ao componente AudioSource

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        if (door == null)
        {
            Debug.LogError("A variável 'door' não foi configurada no Inspector!");
            return;
        }

        // Obtém o componente Animator da porta
        doorAnimator = door.GetComponent<Animator>();
        if (doorAnimator == null)
        {
            Debug.LogError("Animator não encontrado no objeto da porta!");
            return;
        }

        // Obtém o componente BoxCollider da porta
        doorCollider = door.GetComponent<BoxCollider>();
        if (doorCollider == null)
        {
            Debug.LogError("BoxCollider não encontrado no objeto da porta!");
            return;
        }

        // Obtém o script BallControl do jogador
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            ballControlScript = player.GetComponent<BallControl>();
        }

        if (ballControlScript == null)
        {
            Debug.LogError("BallControl não encontrado no objeto Player!");
            return;
        }

        // Define a animação no frame 23 (estado "fechado")
        doorAnimator.Play("Abrindo", 0, 1f); // Define o estado inicial no frame 23
        doorAnimator.Update(0f); // Força o Animator a aplicar o estado imediatamente
        doorAnimator.SetFloat("Speed", 0f); // Pausa a animação no frame 23
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se a bola colidiu com a porta
        if (collision.gameObject.CompareTag("Player"))
        {
            CheckAndOpenDoor();
        }
    }

    private void CheckAndOpenDoor()
    {
        // Verifica se a porta já foi aberta
        if (isOpened)
        {
            Debug.Log("A porta já foi aberta.");
            return;
        }

        // Verifica se a bola está no estado de dash e tem a quantidade necessária de lixos
        if (ballControlScript.isDashing && ballControlScript.trashCount >= requiredTrashCount)
        {
            // Verifica se o parâmetro "Speed" existe antes de configurá-lo
            if (doorAnimator.parameters.Length > 0 && ParameterExists("Speed"))
            {
                doorAnimator.SetFloat("Speed", -1f); // Define a velocidade negativa para reverter a animação
                doorAnimator.Play("Abrindo", 0, 1f); // Começa do último frame (23)
                audioSource.PlayOneShot(doorOpenSound); // Toca o som de abertura da porta
                Debug.Log("Porta aberta!");

                // Marca a porta como aberta
                isOpened = true;

                // Desativa o BoxCollider da porta para permitir a passagem
                doorCollider.enabled = false;

                // Remove todos os objetos com a tag "Trash" associados ao jogador
                RemoveTrashFromPlayer();

                // Faz a bola parar de girar chamando a função Nocaute
                ballControlScript.GetComponentInChildren<Animacoes>()?.Knockout();
                Debug.Log("A bola parou de girar (Nocaute chamado).");
            }
            else
            {
                Debug.LogError("Parâmetro 'Speed' não encontrado no Animator Controller!");
            }
        }
        else
        {
            Debug.Log("Condições para abrir a porta não foram atendidas.");
        }
    }

    private void RemoveTrashFromPlayer()
    {
        // Itera pelos filhos do jogador e remove os objetos com a tag "Trash"
        foreach (Transform child in ballControlScript.transform)
        {
            if (child.CompareTag("Trash"))
            {
                Destroy(child.gameObject); // Destroi o objeto com a tag "Trash"
            }
        }

        // Atualiza o contador de lixos no BallControl
        ballControlScript.UpdateTrashCount();
        Debug.Log("Todos os objetos com a tag 'Trash' foram removidos do jogador.");
    }

    private bool ParameterExists(string parameterName)
    {
        foreach (var param in doorAnimator.parameters)
        {
            if (param.name == parameterName)
            {
                return true;
            }
        }
        return false;
    }

    // Eventos chamados pela animação
    public void PortaAberta()
    {
        Debug.Log("Evento PortaAberta chamado!");
    }

    public void PortaFechada()
    {
        Debug.Log("Evento PortaFechada chamado!");
    }
}