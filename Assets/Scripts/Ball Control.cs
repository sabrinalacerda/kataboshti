using UnityEngine;
using System.Collections.Generic;

public class BallControl : MonoBehaviour
{
    public float moveSpeedWhileNotSpinning = 5f; // Velocidade de movimento quando não está girando
    public float moveSpeedWhileSpinning = 8f; // Velocidade de movimento quando está girando
    public float rotationSpeed = 100f; // Velocidade de rotação
    public float dashForce = 10f; // Força do dash
    public float dashCooldown = 2f; // Tempo de espera entre dashes
    public float attachmentRadius = 0.3f; // Raio base para posicionar objetos grudados
    public float extraAttachmentDistance = 0.5f; // Distância inicial adicional para novos lixos
    public float distanceIncrement = 0.3f; // Incremento da distância adicional a cada `trashThreshold`
    public int trashThreshold = 10; // Quantidade de lixos para começar a posicionar mais distante
    public GameObject ringPrefab; // Prefab dos objetos que saltam
    public float ringLaunchForce = 5f; // Força aplicada aos objetos que saltam
    public Transform cameraTransform; // Referência à câmera
    public float cameraBaseDistance = 10f; // Distância base da câmera
    public float cameraDistanceIncrement = 2f; // Incremento da distância da câmera por limiar de lixo

    private Rigidbody rb;
    private List<Transform> attachedObjects = new List<Transform>(); // Lista de objetos grudados
    private float nextDashTime = 0f; // Tempo em que o próximo dash estará disponível
    public bool isDashing = false; // Indica se o dash está ativo
    public bool isSpinning = false; // Indica se a bola está girando
    public int trashCount = 0; // Contador de lixos coletados
    private int lastThresholdReached = 0; // Último limiar atingido

    private Animacoes animControl; // Referência ao script Animacoes
    public AudioClip dashSound; // Som do dash
    public AudioClip trashSound; // Som de coleta de lixo
    private AudioSource audioSource; // Referência ao componente AudioSource
    public float maxSpeed = 30f; // Ajuste o valor conforme necessário

    public bool IsSpinning
    {
        get { return isSpinning; }
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        // Obtém o componente Rigidbody para aplicar física
        rb = GetComponent<Rigidbody>();

        // Configura a detecção de colisão contínua para evitar atravessar objetos
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Garante que a bola não comece girando
        isSpinning = false;

        // Obtém a referência ao script Controle das Animacoes
        animControl = GetComponentInChildren<Animacoes>();
    }

    void Update()
    {
        // Limita a velocidade máxima da bola
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Verifica se a barata está em nocaute
        if (animControl != null && animControl.IsKnockedOut())
        {
            // Define a rotação da bola para (0, 0, 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);

            // Zera a velocidade linear e angular para parar o deslize
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            return; // Impede o controle da bola enquanto está em nocaute
        }

        // Verifica se a bola não está girando e está caindo
        if (!isSpinning && rb.linearVelocity.y < 0)
        {
            // Aplica uma força extra para acelerar a queda
            rb.AddForce(Vector3.down * 10f, ForceMode.Acceleration); // Ajuste o valor (10f) conforme necessário
        }

        // Obtém a direção da câmera no plano horizontal
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Remove a componente vertical da direção da câmera
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normaliza os vetores para garantir que tenham magnitude 1
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Captura entrada do jogador para movimento
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movement += cameraForward; // Move para frente na direção da câmera
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement -= cameraForward; // Move para trás na direção oposta à câmera
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement -= cameraRight; // Move para a esquerda em relação à câmera
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += cameraRight; // Move para a direita em relação à câmera
        }

        // Normaliza o vetor de movimento para evitar velocidades maiores ao pressionar múltiplas teclas
        if (movement.magnitude > 0)
        {
            movement.Normalize();

            // Aplica a velocidade de movimento com base no estado de giro
            float currentMoveSpeed = isSpinning ? moveSpeedWhileSpinning : moveSpeedWhileNotSpinning;
            rb.AddForce(movement * currentMoveSpeed);

            // Se a bola está girando, aplica rotação
            if (isSpinning)
            {
                Vector3 rotation = new Vector3(movement.z, 0.0f, -movement.x);
                transform.Rotate(rotation * rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Ajusta a rotação da bola para olhar na direção do movimento
                Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }

        // Se a bola não está girando, mantém a rotação fixa "de pé" e desativa o deslize
        if (!isSpinning)
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            rb.linearDamping = 10f; // Aumenta o arrasto linear para evitar deslize
            rb.angularDamping = 10f; // Aumenta o arrasto angular para evitar rotação
        }
        else
        {
            rb.linearDamping = 0f; // Reduz o arrasto linear para permitir deslize
            rb.angularDamping = 0.05f; // Reduz o arrasto angular para permitir rotação
        }

        // Verifica se a tecla espaço foi pressionada
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isSpinning)
            {
                // Ativa o estado de giro
                isSpinning = true;
            }
            else if (Time.time >= nextDashTime)
            {
                // Realiza o dash apenas se a bola estiver girando
                Vector3 dashDirection = movement.normalized; // Direção do movimento atual
                rb.AddForce(dashDirection * dashForce, ForceMode.Impulse); // Aplica força de impulso
                nextDashTime = Time.time + dashCooldown; // Define o próximo tempo disponível para o dash
                isDashing = true; // Dash está ativo
                audioSource.PlayOneShot(dashSound); // Toca o som do dash
            }
        }

        // Verifica se a tecla I foi pressionada para desativar o giro
        if (Input.GetKeyDown(KeyCode.I))
        {
            isSpinning = false;
        }

        // Verifica se o cooldown do dash acabou
        if (Time.time >= nextDashTime && isDashing)
        {
            isDashing = false; // Dash não está mais ativo
        }
    }

    void FixedUpdate()
    {
        // Aplica uma força extra para acelerar a queda se o jogador estiver em knockout
        if (animControl != null && animControl.IsKnockedOut())
        {
            rb.AddForce(Vector3.down * 100f, ForceMode.Acceleration); // Ajuste o valor (100f) conforme necessário
        }
    }

    public void UpdateTrashCount()
    {
        // Recalcula o número de objetos com a tag "Trash" associados ao jogador
        trashCount = 0;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Trash"))
            {
                trashCount++;
            }
        }

        Debug.Log($"Trash count atualizado: {trashCount}");
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verifica se o objeto colidido tem a tag "Trash"
        if (collision.gameObject.CompareTag("Trash"))
        {
            if (isSpinning)
            {
                collision.transform.SetParent(transform);
                attachedObjects.Add(collision.transform);
                PositionNewTrash(collision.transform);
                UpdateTrashCount();
                audioSource.PlayOneShot(trashSound); // Toca o som de coleta de lixo

                Rigidbody trashRb = collision.gameObject.GetComponent<Rigidbody>();
                if (trashRb != null)
                {
                    trashRb.isKinematic = true; // Garante que o Rigidbody do lixo seja cinemático
                    //trashRb.useGravity = false; // Desativa a gravidade do lixo
                }
            }
        }

        // Verifica se o objeto colidido tem a tag "Shit"
        if (collision.gameObject.CompareTag("Shit"))
        {
            Animacoes animControl = GetComponentInChildren<Animacoes>();
            if (animControl != null)
            {
                animControl.Knockout();
            }

            isDashing = false;
            GenerateRings();

            foreach (Transform child in transform)
            {
                if (child.CompareTag("Trash") && !child.CompareTag("Barata"))
                {
                    Destroy(child.gameObject);
                }
            }

            attachedObjects.Clear();
            UpdateTrashCount();
            isSpinning = false;

            rb.isKinematic = false;
            rb.useGravity = true;

            // Aplica uma força extra para acelerar a queda
            rb.AddForce(Vector3.down * 50f, ForceMode.Impulse); // Ajuste o valor (50f) conforme necessário
        }
    }

    System.Collections.IEnumerator BlinkPlayer(int blinkCount, float blinkInterval)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            for (int i = 0; i < blinkCount; i++)
            {
                renderer.enabled = false; // Torna o jogador invisível
                yield return new WaitForSeconds(blinkInterval);
                renderer.enabled = true; // Torna o jogador visível novamente
                yield return new WaitForSeconds(blinkInterval);
            }
        }
    }

    void PositionNewTrash(Transform trash)
    {
        // Calcula a distância para posicionar o novo objeto
        float distance = attachmentRadius + extraAttachmentDistance;

        // Gera ângulos aleatórios para posicionar o lixo em torno da bola
        float theta = Random.Range(0, Mathf.PI * 2); // Ângulo em torno do eixo Y (longitude)
        float phi = Random.Range(0, Mathf.PI); // Ângulo em relação ao eixo vertical (latitude)

        // Converte coordenadas esféricas para cartesianas
        float x = distance * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = distance * Mathf.Cos(phi);
        float z = distance * Mathf.Sin(phi) * Mathf.Sin(theta);

        // Define a posição do lixo em relação ao centro da bola
        trash.localPosition = new Vector3(x, y, z);

        // Opcional: Adiciona uma rotação aleatória ao lixo
        trash.localRotation = Random.rotation;
    }

    void GenerateRings()
    {
        foreach (Transform trash in attachedObjects)
        {
            // Instancia o prefab em uma posição próxima ao jogador
            GameObject ring = Instantiate(ringPrefab, trash.position, Quaternion.identity);

            // Obtém o Rigidbody do objeto para aplicar física
            Rigidbody ringRb = ring.GetComponent<Rigidbody>();
            if (ringRb != null)
            {
                // Calcula uma direção aleatória para o lançamento
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f), // Direção horizontal aleatória
                    Random.Range(0.8f, 1.2f), // Direção vertical (prioriza ir para cima)
                    Random.Range(-1f, 1f)  // Direção horizontal aleatória
                ).normalized;

                // Aplica uma força na direção aleatória
                ringRb.AddForce(randomDirection * ringLaunchForce, ForceMode.Impulse);
            }
        }
    }
}