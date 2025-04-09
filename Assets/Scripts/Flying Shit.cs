using UnityEngine;

public class FlyingShit : MonoBehaviour
{
    public GameObject trashPrefab; // Prefab do objeto de lixo a ser instanciado
    public float destroyTime = 3f; // Tempo para o objeto ser destruído após ser criado
    private bool hasHitGround = false; // Verifica se o anel já colidiu com o chão
    private Rigidbody rb; // Referência ao Rigidbody do objeto

    private float minSpeed = 12f; // Velocidade mínima para a força para cima
    private float maxSpeed = 16f; // Velocidade máxima para a força para cima
    private float maxTorque = 10f; // Torque máximo para rotação

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtém o componente Rigidbody anexado ao objeto

        // Aplica uma força para cima aleatória
        rb.AddForce(RandomForce(), ForceMode.Impulse);

        // Aplica torque aleatório para rotação
        rb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);

        // Inicia a corrotina para destruir o objeto após o tempo especificado
        StartCoroutine(DestroyAfterTime(destroyTime));
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verifica se o anel colidiu com o chão
        if (!hasHitGround && collision.gameObject.CompareTag("Ground"))
        {
            hasHitGround = true; // Marca que o anel já colidiu com o chão
            StartCoroutine(DestroyAndSpawnTrash());
        }
    }

    System.Collections.IEnumerator DestroyAndSpawnTrash()
    {
        // Aguarda 2 segundos antes de destruir o anel
        yield return new WaitForSeconds(1.5f);

        // Instancia o objeto de lixo na posição e rotação do anel
        Instantiate(trashPrefab, transform.position, transform.rotation);

        // Destroi o anel
        Destroy(gameObject);
    }

    System.Collections.IEnumerator DestroyAfterTime(float time)
    {
        // Aguarda o tempo especificado
        yield return new WaitForSeconds(time);

        // Verifica se o objeto ainda não foi destruído
        if (!hasHitGround)
        {
            // Instancia o objeto de lixo na posição e rotação do anel
            Instantiate(trashPrefab, transform.position, transform.rotation);

            // Destroi o anel
            Destroy(gameObject);
        }
    }

    // Gera um vetor de força para cima aleatório
    Vector3 RandomForce()
    {
        return Vector3.up * Random.Range(minSpeed, maxSpeed); // Força para cima aleatória
    }

    // Gera um valor de torque aleatório
    float RandomTorque()
    {
        return Random.Range(-maxTorque, maxTorque); // Torque aleatório
    }
}
