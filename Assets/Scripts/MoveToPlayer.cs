using UnityEngine;

public class MoveToPlayer : MonoBehaviour
{
    public AudioClip poop;
    private AudioSource audioSource;
    private float speed = 40.0f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isFalling = false;
    private float moveTime = 0f; // Tempo de movimento

    public float altura = 2.5f; // Altura fixa em relação ao jogador

    public GameObject trash; 
    public string inputID;

    private Vector3 moveDirection; // Direção do movimento

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            moveDirection = (targetPosition - transform.position).normalized; // Calcula a direção do movimento
        }
        isMoving = true; 
    }

    void Update()
    {
        if (isMoving)
        {
            moveTime += Time.deltaTime;

            // Move o objeto na direção calculada
            transform.position += moveDirection * speed * Time.deltaTime;

            // Após 5 segundos, inicia a queda
            if (moveTime >= 5f)
            {
                isMoving = false;
                isFalling = true;
            }

            // Verifica se o objeto chegou ao destino
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                isMoving = false;
                if (inputID == "1")
                {
                    Instantiate(trash, transform.position, Quaternion.identity);
                }
            }
        }

        if (isFalling)
        {
            // Continua se movendo na direção original e adiciona a queda
            transform.position += moveDirection * speed * Time.deltaTime;
            transform.position += Vector3.down * speed * Time.deltaTime;
        }

        if (!isMoving && !isFalling)
        {
            audioSource.PlayOneShot(poop); // Toca o som de queda
            // Destroi o objeto se ele parou de se mover e não está caindo
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se colidiu com o objeto com tag "Ground"
        if (collision.gameObject.CompareTag("Ground"))
        {
            isFalling = false; // Para a queda
            Destroy(gameObject); // Destroi o objeto
        }

        // Verifica se colidiu com o objeto com tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroi o objeto
        }
    }
}
