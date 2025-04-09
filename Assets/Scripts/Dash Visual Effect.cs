using UnityEngine;

public class DashVisualEffect : MonoBehaviour
{
    public BallControl ballControl; // Referência ao script BallControl
    private ParticleSystem particleSystem;

    void Start()
    {
        // Obtém o componente ParticleSystem anexado ao objeto
        particleSystem = GetComponent<ParticleSystem>();

        // Garante que o ParticleSystem esteja desativado no início
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }
    }

    void Update()
    {
        if (ballControl != null && particleSystem != null)
        {
            // Ativa o ParticleSystem enquanto a bola estiver fazendo dash
            if (ballControl.isDashing)
            {
                if (!particleSystem.isPlaying)
                {
                    particleSystem.Play();
                }
            }
            else
            {
                if (particleSystem.isPlaying)
                {
                    particleSystem.Stop();
                }
            }
        }
    }
}
