using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocaFase : MonoBehaviour
{
    public Transform directionTarget; // Referência ao GameObject vazio que define a direção
    public float moveSpeed = 5f; // Velocidade de movimento automático
    public float sceneChangeDelay = 3f; // Tempo em segundos antes de trocar de cena

    [Tooltip("Índice da próxima cena a ser carregada")] // Permite adicionar uma descrição no Inspector
    [SerializeField] // Torna o campo visível e editável no Inspector
    public int nextSceneIndex; // Índice da próxima cena a ser carregada

    private Animacoes animControl; // Referência ao script Animacoes
    private bool hasTriggered = false; // Verifica se a ação já foi executada
    private SceneFader sceneFader; // Referência ao SceneFader

    private void Start()
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
        if (hasTriggered) return; // Se já foi acionado, não executa novamente

        if (other.CompareTag("Player")) // Verifica se o objeto colidido é o jogador
        {
            hasTriggered = true; // Marca como acionado

            BallControl ballControl = other.GetComponent<BallControl>();
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            animControl = other.GetComponentInChildren<Animacoes>();

            if (ballControl != null && playerRb != null)
            {
                // Desativa o controle do jogador
                ballControl.enabled = false;

                // Garante que isSpinning e isDashing sejam false
                ballControl.isSpinning = false;
                ballControl.isDashing = false;

                // Ativa a física da bola
                playerRb.isKinematic = false; // Permite que a gravidade atue
                playerRb.useGravity = true; // Garante que a gravidade está ativa

                // Para o movimento da bola
                playerRb.linearVelocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;

                // Ajusta a rotação da barata para ficar de frente para o alvo
                Vector3 lookDirection = (directionTarget.position - other.transform.position).normalized;
                lookDirection.y = 0; // Garante que a rotação seja apenas no plano horizontal
                other.transform.rotation = Quaternion.LookRotation(lookDirection);

                // Destrói os objetos filhos com a tag "Trash"
                DestroyTrashObjects(other.transform);

                // Define o parâmetro "cutscene" no Animator para true
                if (animControl != null)
                {
                    animControl.SetCutscene(true); // Ativa o modo cutscene
                }

                // Inicia o fade-in
                if (sceneFader != null)
                {
                    sceneFader.FadeToScene();
                }
                else
                {
                    Debug.LogError("SceneFader não está configurado corretamente!");
                }

                // Aguarda 0.5 segundos e move a barata
                StartCoroutine(MovePlayerAfterDelay(other.gameObject));
            }
        }
    }

    private void DestroyTrashObjects(Transform playerTransform)
    {
        foreach (Transform child in playerTransform)
        {
            if (child.CompareTag("Trash")) // Verifica se o objeto tem a tag "Trash"
            {
                Destroy(child.gameObject); // Destroi o objeto
            }
        }
    }

    private System.Collections.IEnumerator MovePlayerAfterDelay(GameObject player)
    {
        yield return new WaitForSeconds(0.5f);

        if (animControl != null)
        {
            animControl.SetCutscene(false); // Sai do modo cutscene
            animControl.PlayWalking(); // Garante que a animação Walking_001 seja tocada
            StartCoroutine(ChangeSceneAfterDelay());
        }

        // Move a barata manualmente em direção ao alvo
        while (Vector3.Distance(player.transform.position, directionTarget.position) > 0.1f)
        {
            Vector3 direction = (directionTarget.position - player.transform.position).normalized;
            player.transform.position += direction * moveSpeed * Time.deltaTime;

            // Mantém a barata "em pé" enquanto se move
            player.transform.rotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);

            yield return null; // Aguarda o próximo quadro
        }

        // Define a animação Idle ao parar
        if (animControl != null)
        {
            animControl.PlayIdle(); // Garante que a animação Idle_001 seja tocada
        }
        
    }

    private System.Collections.IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSeconds(sceneChangeDelay);

        // Verifica se o índice da próxima cena é válido
        if (nextSceneIndex >= 0 && nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError($"O índice da próxima cena ({nextSceneIndex}) está fora do intervalo válido! Total de cenas: {SceneManager.sceneCountInBuildSettings}");
        }
    }
}

