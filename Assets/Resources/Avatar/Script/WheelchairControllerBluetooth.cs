using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelchairControllerBluetooth : MonoBehaviour
{
    public float MovementSpeed = 5f; // Velocidade de movimento
    public float RotationSpeed = 50f; // Velocidade de rotação

    private Rigidbody rb;
    private Vector3 movementInput;
    private float rotationInput;

    public Animator animator1; // Referência ao primeiro Animator
    public Animator animator2; // Referência ao segundo Animator

    private float baselineX = 1550f; // Valor padrão inicial
    private bool baselineSet = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    public void ProcessBluetoothData(string data)
    {
        string[] values = data.Split(',');
        if (values.Length < 2)
        {
            return;
        }
        int x = int.Parse(values[0].Trim());
        int y = int.Parse(values[1].Trim());

        Debug.Log($"Received data: x={x}, y={y}");

        // Definir o baseline baseado no primeiro valor recebido
        if (!baselineSet)
        {
            baselineX = x;
            baselineSet = true;
            Debug.Log($"Baseline set to: {baselineX}");
        }

        // Resetar inputs
        movementInput = Vector3.zero;
        rotationInput = 0f;

        // Calcular limites usando o baseline
        float lowerLimit = baselineX - 300;
        float upperLimit = baselineX + 200;

        // Movimento para frente e para trás
        if (x > upperLimit)
        {
            movementInput = Vector3.forward * MovementSpeed;
            SetAnimatorBools("tras", true);
        }
        else
        {
            SetAnimatorBools("tras", false);
        }

        if (x < lowerLimit)
        {
            movementInput = -Vector3.forward * MovementSpeed;
            SetAnimatorBools("frente", true);
        }
        else
        {
            SetAnimatorBools("frente", false);
        }

        // Movimento de rotação
        if (y < lowerLimit)
        {
            rotationInput = -RotationSpeed;
            SetAnimatorBools("esquerda", true);
        }
        else
        {
            SetAnimatorBools("esquerda", false);
        }

        if (y > upperLimit)
        {
            rotationInput = RotationSpeed;
            SetAnimatorBools("direita", true);
        }
        else
        {
            SetAnimatorBools("direita", false);
        }
    }

    void SetAnimatorBools(string boolName, bool value)
    {
        if (animator1 != null && animator1.GetBool(boolName) != value)
        {
            animator1.SetBool(boolName, value);
        }

        if (animator2 != null && animator2.GetBool(boolName) != value)
        {
            animator2.SetBool(boolName, value);
        }
    }

    void FixedUpdate()
    {
        if (movementInput != Vector3.zero)
        {
            Vector3 movement = transform.forward * movementInput.z * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }

        if (rotationInput != 0)
        {
            Quaternion rotation = Quaternion.Euler(0, rotationInput * Time.fixedDeltaTime, 0);
            rb.MoveRotation(rb.rotation * rotation);
        }
    }
}
