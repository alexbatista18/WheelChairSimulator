using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO.Ports;

public class MovementJoystick : MonoBehaviourPun
{
    public float MovementSpeed = 5f;
    public float RotationSpeed = 50f;
    private Rigidbody rb;
    private Vector3 movementInput;
    private float rotationInput;

    private float baselineX = 1550f;
    private bool baselineSet = false;

    private SerialPort serialPort;
    public string portName = "COM10";
    public int baudRate = 9600;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        if (photonView.IsMine)
        {
            // Abre a porta serial para o jogador local
            try
            {
                serialPort = new SerialPort(portName, baudRate);
                serialPort.Open();
                serialPort.ReadTimeout = 1000;

                if (serialPort.IsOpen)
                {
                    Debug.Log($"Port {portName} opened successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to open port {portName}.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to open port {portName}: {ex.Message}");
            }
        }
        else
        {
            // Desativa a câmera principal se não for o jogador local
            if (Camera.main != null)
            {
                Camera.main.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    if (serialPort.BytesToRead > 0)
                    {
                        string data = serialPort.ReadExisting();
                        Debug.Log($"Data received on port {portName}: {data}");
                        if (!string.IsNullOrEmpty(data))
                        {
                            ProcessBluetoothData(data);
                        }
                    }
                }
                catch (System.TimeoutException)
                {
                    Debug.LogWarning($"Read timed out on port {portName}.");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error reading from port {portName}: {ex.Message}");
                }
            }
        }
    }

    public void ProcessBluetoothData(string data)
    {
        string[] values = data.Split(',');
        if (values.Length < 2)
        {
            Debug.LogWarning("Invalid data received: " + data);
            return;
        }

        try
        {
            int x = int.Parse(values[0].Trim());
            int y = int.Parse(values[1].Trim());

            Debug.Log($"Processed data: x={x}, y={y}");

            if (!baselineSet)
            {
                baselineX = x;
                baselineSet = true;
                Debug.Log($"Baseline set to: {baselineX}");
            }

            movementInput = Vector3.zero;
            rotationInput = 0f;

            float lowerLimit = baselineX - 300;
            float upperLimit = baselineX + 200;

            if (x > upperLimit)
            {
                movementInput = Vector3.forward * MovementSpeed;
            }

            if (x < lowerLimit)
            {
                movementInput = -Vector3.forward * MovementSpeed;
            }

            if (y < lowerLimit)
            {
                rotationInput = -RotationSpeed;
            }

            if (y > upperLimit)
            {
                rotationInput = RotationSpeed;
            }

            // Envia posição e rotação para os outros jogadores
            photonView.RPC("SyncMovement", RpcTarget.AllBuffered, transform.position, transform.rotation, movementInput.z);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse data: {data}. Error: {ex.Message}");
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

    [PunRPC]
    void SyncMovement(Vector3 position, Quaternion rotation, float velocity)
    {
        // Interpolação para suavizar os movimentos e rotações
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 40f);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 40f);
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log($"Port {portName} closed.");
        }
    }

    public Vector3 MovementInput
    {
        get { return movementInput; }
    }
    
}

