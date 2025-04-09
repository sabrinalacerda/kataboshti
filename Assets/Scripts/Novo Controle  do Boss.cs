using UnityEngine;

public class NovoControledoBoss : MonoBehaviour
{
    public Transform jogador; // Referência ao jogador
    public GameObject projetilPrefab; // Prefab do projétil
    public Transform pontoDeDisparo; // Ponto de origem do disparo
    public float velocidadeProjetil = 10f; // Velocidade do projétil
    public Animator animator; // Referência ao Animator
    public AudioClip somDoAtaque; // Som do ataque
    public AudioClip musicaDeBatalha; // Música de batalha
    public int vida = 3; // Vida do boss
    public GameObject objetoParaDestruir; // Objeto que será destruído quando o boss morrer
    public int quantidadeMinimaDeLixos = 5; // Quantidade mínima de lixos necessária para causar dano
    public float intervaloEntreAtaques = 2f; // Intervalo entre os ataques do boss (em segundos)

    private bool jogadorNaArea = false; // Controle para verificar se o jogador está na área de ataque
    private AudioSource audioSource; // Referência ao AudioSource
    private float proximoAtaque = 0f; // Tempo em que o próximo ataque estará disponível

    void Start()
    {
        // Obtém o componente AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Update é chamado uma vez por frame
    void Update()
    {
        if (jogador != null && jogadorNaArea && Time.time >= proximoAtaque)
        {
            // Calcula a direção para o jogador no plano horizontal (ignora o eixo Y)
            Vector3 directionToPlayer = jogador.position - transform.position;
            directionToPlayer.y = 0; // Garante que a rotação seja apenas no plano horizontal

            // Calcula a rotação necessária para olhar para o jogador
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Aplica a rotação suavemente apenas no eixo Y
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

            // Inicia o ataque
            Atacar();

            // Define o tempo para o próximo ataque
            proximoAtaque = Time.time + intervaloEntreAtaques;
        }
    }

    void Atacar()
    {
        // Define o trigger "Ataque" para iniciar a animação de ataque imediatamente
        animator.SetTrigger("Ataque");
        
    }

    // Este método será chamado no frame 29 da animação
    public void LancaProjetil()
    {
        // Instancia o projétil no ponto de disparo
        GameObject projetil = Instantiate(projetilPrefab, pontoDeDisparo.position, Quaternion.identity);
        audioSource.PlayOneShot(somDoAtaque);
        // Define a direção do projétil para o jogador
        Vector3 direcao = (jogador.position - pontoDeDisparo.position).normalized;

        // Adiciona velocidade ao projétil
        Rigidbody rb = projetil.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direcao * velocidadeProjetil;
        }

        // Define a tag do projétil como "Shit"
        projetil.tag = "Shit";
    }

    // Este método será chamado no final da animação
    public void TerminaAtaque()
    {
        // Reseta o trigger "Ataque" para garantir que a animação possa ser reiniciada
        animator.ResetTrigger("Ataque");
    }

    // Detecta quando o jogador entra na área de ataque
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorNaArea = true;
        }
    }

    // Detecta quando o jogador sai da área de ataque
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorNaArea = false;

            // Para a música de batalha
            if (audioSource.isPlaying && audioSource.clip == musicaDeBatalha)
            {
                audioSource.Stop();
            }
        }
    }

    // Detecta quando o jogador colide com o boss
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtém o script do jogador
            BallControl player = collision.gameObject.GetComponent<BallControl>();
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            // Verifica se o jogador está no estado de dash e tem a quantidade mínima de lixos
            if (player != null && player.isDashing && player.trashCount >= quantidadeMinimaDeLixos)
            {
                // Reduz a vida do boss
                vida--;

                // Destroi todos os lixos atrelados ao jogador
                DestroyAllTrash(player);

                // Aplica uma força ao jogador na direção oposta ao boss
                if (playerRb != null)
                {
                    Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerRb.AddForce(knockbackDirection * 200f, ForceMode.Impulse); // Ajuste a força conforme necessário
                }

                // Desativa o controle do jogador por 1 segundo
                StartCoroutine(DisablePlayerControlTemporarily(player));

                // Verifica se o boss morreu
                if (vida <= 0)
                {
                    Morrer();
                }
            }
        }
    }

    private System.Collections.IEnumerator DisablePlayerControlTemporarily(BallControl player)
    {
        // Desativa o controle do jogador e o estado de dash
        player.enabled = false;
        player.isDashing = false;

        // Aguarda 1 segundo
        yield return new WaitForSeconds(1f);

        // Reativa o controle do jogador
        player.enabled = true;
    }

    private void DestroyAllTrash(BallControl player)
    {
        // Itera sobre todos os filhos do jogador
        foreach (Transform child in player.transform)
        {
            // Verifica se o objeto tem a tag "Trash"
            if (child.CompareTag("Trash"))
            {
                Destroy(child.gameObject); // Destroi o objeto
            }
        }

        // Atualiza o contador de lixos no jogador
        player.UpdateTrashCount();
    }

    void Morrer()
    {
        // Garante que todas as animações sejam interrompidas
        animator.ResetTrigger("Ataque");

        // Inicia imediatamente a animação de morte
        animator.Play("Morte", 0, 0f); // Força a animação "Morte" a começar do início

        // Destrói o objeto especificado, se ele foi atribuído
        if (objetoParaDestruir != null)
        {
            Destroy(objetoParaDestruir);
        }

        // Inicia a corrotina para destruir o boss após a animação de morte
        StartCoroutine(DestroyAfterDeathAnimation());
    }

    private System.Collections.IEnumerator DestroyAfterDeathAnimation()
    {
        // Aguarda até que a animação de morte termine
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Morte") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null; // Aguarda o próximo frame
        }

        // Destroi o boss
        Destroy(gameObject);
    }
}
