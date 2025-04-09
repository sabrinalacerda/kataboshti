using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject projectilePrefab1; // Prefab do projétil para LaunchProjectileAtPlayer
    public GameObject projectilePrefab2; // Prefab do projétil para SpinAndShoot
    public GameObject projectilePrefab3; // Prefab do projétil para VolcanoAttack
    public GameObject projectilePrefab4; // Prefab do projétil para MachineGunAttack
    public Transform player; // Referência ao jogador
    public int life = 3; // Vida inicial do Boss
    public float attackCooldown = 3f; // Tempo de espera entre ataques

    private bool isAttacking = false;

    void Update()
    {
        if (!isAttacking && life > 0)
        {
            StartCoroutine(AttackPattern());
        }
    }

    IEnumerator AttackPattern()
    {
        isAttacking = true;

        // Escolhe aleatoriamente um ataque com base na vida do Boss
        int attackIndex = Random.Range(0, life == 3 ? 2 : life == 2 ? 3 : 4);

        switch (attackIndex)
        {
            case 0:
                Debug.Log("Boss está realizando o ataque: LaunchProjectileAtPlayer");
                yield return StartCoroutine(LaunchProjectileAtPlayer());
                break;
            case 1:
                Debug.Log("Boss está realizando o ataque: SpinAndShoot");
                yield return StartCoroutine(SpinAndShoot());
                break;
            case 2:
                if (life >= 2)
                {
                    Debug.Log("Boss está realizando o ataque: VolcanoAttack");
                    yield return StartCoroutine(VolcanoAttack());
                }
                break;
            case 3:
                if (life == 1)
                {
                    Debug.Log("Boss está realizando o ataque: MachineGunAttack");
                    yield return StartCoroutine(MachineGunAttack());
                }
                break;
        }

        // Espera 3 segundos antes de realizar o próximo ataque
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator LaunchProjectileAtPlayer()
    {
        Debug.Log("Iniciando ataque: LaunchProjectileAtPlayer");
        int attackCount = 5; // Número de projéteis lançados
        for (int i = 0; i < attackCount; i++)
        {
            if (player != null)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                GameObject projectile = Instantiate(projectilePrefab1, transform.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody>().linearVelocity = direction * 10f; // Ajuste a velocidade conforme necessário
            }
            yield return new WaitForSeconds(0.5f); // Intervalo entre disparos
        }
    }

    IEnumerator SpinAndShoot()
    {
        Debug.Log("Iniciando ataque: SpinAndShoot");
        int projectileCount = 12; // Número de projéteis por giro
        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = i * angleStep;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject projectile = Instantiate(projectilePrefab2, transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody>().linearVelocity = direction * 10f; // Ajuste a velocidade conforme necessário

            // Gira o Boss no eixo Y
            transform.rotation = Quaternion.Euler(0, angle, 0);
            yield return new WaitForSeconds(0.2f); // Intervalo entre disparos
        }
    }

    IEnumerator VolcanoAttack()
    {
        Debug.Log("Iniciando ataque: VolcanoAttack");
        float attackDuration = 5f; // Duração do ataque
        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            // Calcula uma direção com variação horizontal, mas sempre apontando para cima
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)).normalized;

            // Define a posição de spawn no topo central do Boss
            Vector3 spawnPosition = transform.position + Vector3.up * 2f; // Ajuste a altura conforme necessário

            // Instancia o projétil no topo do Boss
            GameObject projectile = Instantiate(projectilePrefab3, spawnPosition, Quaternion.identity);

            // Aplica uma velocidade ao projétil na direção calculada
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.linearVelocity = randomOffset * Random.Range(5f, 10f); // Lança para cima com força aleatória

            elapsedTime += 0.2f;
            yield return new WaitForSeconds(0.2f); // Intervalo entre lançamentos
        }
    }

    IEnumerator MachineGunAttack()
    {
        Debug.Log("Iniciando ataque: MachineGunAttack");
        float attackDuration = 5f; // Duração do ataque
        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            if (player != null)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                GameObject projectile = Instantiate(projectilePrefab4, transform.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody>().linearVelocity = direction * 15f; // Ajuste a velocidade conforme necessário
            }
            elapsedTime += 0.1f;
            yield return new WaitForSeconds(0.1f); // Intervalo curto entre disparos
        }
    }
}


