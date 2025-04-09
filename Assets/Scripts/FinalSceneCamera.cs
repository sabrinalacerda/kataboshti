using UnityEngine;

public class FinalSceneCamera : MonoBehaviour
{
    public float speed = 0.01f; // Velocidade do movimento
    private Vector3 positionFinal = new Vector3(0, 1.37f, -3.56f); // Posição final
    private Vector3 positionInitial = new Vector3(0, 1.37f, 4.74f); // Posição inicial

    void Start()
    {
        // Define a posição inicial da câmera
        transform.position = positionInitial;
    }

    void Update()
    {
        // Move a câmera em direção à posição final lentamente
        transform.position = Vector3.MoveTowards(transform.position, positionFinal, speed * Time.deltaTime);

        // Opcional: Verifica se a câmera chegou ao destino
        if (transform.position == positionFinal)
        {
            Debug.Log("A câmera chegou à posição final.");
        }
    }
}
