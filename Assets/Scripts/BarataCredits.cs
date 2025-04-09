using UnityEngine;
using System.Collections;

public class BarataCredits : MonoBehaviour
{
    public float speed = 0.5f;
    public float delayBeforeMoving = 3f; // Tempo de espera antes de começar a se mover

    void Start()
    {
        // Inicia a corrotina para esperar antes de começar a se mover
        StartCoroutine(StartMovingAfterDelay());
    }

    IEnumerator StartMovingAfterDelay()
    {
        // Aguarda o tempo especificado
        yield return new WaitForSeconds(delayBeforeMoving);

        // Começa a se mover
        while (true)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            yield return null; // Aguarda o próximo frame
        }
    }
}
