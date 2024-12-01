using UnityEngine;

public class MovementKeypress : MonoBehaviour
{
    // Variáveis para controlar a velocidade de movimento e rotação
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    void Update()
    {
        // Movimento para frente e para trás (W e S)
        float moveDirection = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveDirection);

        // Rotação para os lados (A e D)
        float rotationDirection = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * rotationDirection);
    }
}
