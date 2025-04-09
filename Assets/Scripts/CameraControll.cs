using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public GameObject player;
    public float sensitivity = 5f; // Sensibilidade do movimento do mouse
    public LayerMask collisionMask; // Máscara para detectar colisões (ex.: terreno e paredes)
    private Vector3 baseOffset = new Vector3(0, 2, -5); // Posição base da câmera em relação ao jogador
    private Vector3 offset; // Offset ajustado dinamicamente
    private float pitch = 15f; // Ângulo vertical inicial
    private float yaw = 0f; // Ângulo horizontal inicial

    void Start()
    {
        // Esconde o cursor
        Cursor.visible = false;
    
    // Trava o cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
        // Define o offset inicial
        offset = new Vector3(0, 1, -5);

        // Posiciona a câmera na posição inicial em relação à barata
        transform.position = player.transform.position + offset;

        // Faz a câmera olhar para o jogador
        transform.LookAt(player.transform.position);
    }

    void LateUpdate()
    {
        // Captura o movimento do mouse
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Atualiza os ângulos de rotação
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // Limita o ângulo vertical para evitar giros extremos

        // Calcula a nova posição da câmera
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = player.transform.position + rotation * offset;

        // Verifica colisões com o terreno ou paredes usando um raycast
        RaycastHit hit;
        if (Physics.Linecast(player.transform.position, desiredPosition, out hit, collisionMask))
        {
            // Ajusta a posição da câmera para o ponto de colisão
            transform.position = hit.point;
        }
        else
        {
            // Atualiza a posição da câmera para a posição desejada
            transform.position = desiredPosition;
        }

        // Atualiza a rotação da câmera para olhar para o jogador
        transform.LookAt(player.transform.position);
    }
}