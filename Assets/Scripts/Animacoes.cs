using UnityEngine;

public class Animacoes : MonoBehaviour
{
    private Animator animator; // Referência ao Animator
    private BallControl playerControl; // Referência ao script BallControl
    private Rigidbody playerRigidbody; // Referência ao Rigidbody do jogador
    private bool isKnockedOut = false; // Indica se o jogador está em estado de nocaute
    private bool isInCutscene = false; // Indica se está no modo cutscene

    void Start()
    {
        // Obtém o componente Animator
        animator = GetComponent<Animator>();

        // Encontra o objeto Player e obtém o script BallControl e Rigidbody
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerControl = player.GetComponent<BallControl>();
            playerRigidbody = player.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        // Se está em nocaute ou em cutscene, não atualiza as animações normais
        if (isKnockedOut || isInCutscene) return;

        if (playerControl is not null && playerRigidbody is not null)
        {
            // Verifica se o jogador está girando
            if (playerControl.IsSpinning)
            {
                PlayAnimation("Rolando Fixed"); // Toca a animação de girar
            }
            else
            {
                // Verifica se o jogador está parado ou andando
                if (playerRigidbody.linearVelocity.magnitude < 0.1f)
                {
                    PlayAnimation("Idle_001"); // Toca a animação Idle
                }
                else
                {
                    PlayAnimation("Walking_001"); // Toca a animação de andar
                }
            }
        }
    }

    public void Knockout()
    {
        // Define o estado de nocaute
        isKnockedOut = true;
        PlayAnimation("Nocaute"); // Toca a animação de nocaute
        Debug.Log("Knockout ativado!");

        // Redefine o estado da bola
        if (playerControl != null)
        {
            playerControl.isDashing = false; // Desativa o dash
            playerControl.isSpinning = false; // Desativa o giro

            // Ativa a gravidade e desativa o estado cinemático
            if (playerRigidbody != null)
            {
                playerRigidbody.isKinematic = false; // Permite que a física atue
                playerRigidbody.useGravity = true; // Ativa a gravidade
                playerRigidbody.linearVelocity = Vector3.zero; // Zera a velocidade para evitar deslize
                playerRigidbody.angularVelocity = Vector3.zero; // Zera a rotação
            }

            Debug.Log("Estado da bola redefinido para o inicial e gravidade ativada.");
        }
    }

    public void OnKnockoutAnimationEnd()
    {
        // Sai do estado de nocaute e volta para a animação Idle
        isKnockedOut = false;
        PlayAnimation("Idle_001");
        Debug.Log("Knockout finalizado.");
    }

    public bool IsKnockedOut()
    {
        return isKnockedOut; // Retorna o estado atual de nocaute
    }

    public void SetCutscene(bool isCutscene)
    {
        isInCutscene = isCutscene; // Atualiza o estado de cutscene
        if (animator is not null)
        {
            animator.SetBool("isCutscene", isCutscene); // Define o parâmetro "isCutscene" no Animator
        }
    }

    public void PlayIdle()
    {
        if (animator != null)
        {
            animator.Play("Idle_001"); // Substitua "Idle_001" pelo nome exato da animação Idle no Animator
        }
        else
        {
            Debug.LogError("Animator não encontrado no objeto!");
        }
    }

    public void PlayWalking()
    {
        if (animator != null)
        {
            animator.Play("Walking_001"); // Substitua "Walking_001" pelo nome exato da animação Walking no Animator
        }
        else
        {
            Debug.LogError("Animator não encontrado no objeto!");
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (animator != null)
        {
            animator.Play(animationName); // Toca a animação especificada
        }
    }
}
