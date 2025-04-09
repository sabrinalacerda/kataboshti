using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    public GameObject particlePrefab;


    // Método público pra disparar a partícula em qualquer lugar
    public void PlayParticles(Vector3 position)
    {
        if (particlePrefab != null)
        {
            GameObject particle = Instantiate(particlePrefab, position, Quaternion.identity);
            Debug.Log("Partícula instanciada na posição: " + position);
           
        }
        else
        {
            Debug.LogWarning("particlePrefab não está atribuído no inspector!");
        }
    }
}
