using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO.Ports;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    private Camera playerCamera;
    private SerialPort serialPort;
    public string portName = "COM8";
    public int baudRate = 115200;

    // Variáveis para desaceleração suave
    private float velocidadeAtualDireita = 0f;
    private float velocidadeAtualEsquerda = 0f;
    private float velocidadeAlvoDireita = 0f;
    private float velocidadeAlvoEsquerda = 0f;
    public float tempoDesaceleracao = 1f; // Tempo para desacelerar até 0 (em segundos)

    void Start()
    {
        if (photonView.IsMine)
        {
            // Atribua a câmera do jogador local
            playerCamera = Camera.main;
            if (playerCamera != null)
            {
                playerCamera.transform.SetParent(transform);
                playerCamera.transform.localPosition = new Vector3(0, 5, -10);
                playerCamera.gameObject.SetActive(true); // Ativa a câmera para o jogador local
            }

            // Abre a porta serial para o jogador local
            try
            {
                serialPort = new SerialPort(portName, baudRate);
                serialPort.Open();
                serialPort.ReadTimeout = 100;
                Debug.Log($"Port {portName} opened successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to open port {portName}: {ex.Message}");
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
                string data = serialPort.ReadLine();
                if (!string.IsNullOrEmpty(data))
                {
                    ProcessUSBData(data);
                }
            }
            catch (System.TimeoutException) { }
        }

        // Verifica se uma das rodas está parada para ajustar o tempo de desaceleração
        if (velocidadeAlvoDireita == 0f || velocidadeAlvoEsquerda == 0f)
        {
            // Usa 0,5 segundos para desaceleração se apenas uma roda estiver se movendo
            tempoDesaceleracao = 0.3f;
        }
        else
        {
            // Usa 1 segundo para desaceleração quando ambas as rodas estiverem se movendo
            tempoDesaceleracao = 1f;
        }

        // Suavização da desaceleração
        velocidadeAtualDireita = Mathf.Lerp(velocidadeAtualDireita, velocidadeAlvoDireita, Time.deltaTime / tempoDesaceleracao);
        velocidadeAtualEsquerda = Mathf.Lerp(velocidadeAtualEsquerda, velocidadeAlvoEsquerda, Time.deltaTime / tempoDesaceleracao);

        // Aplica o movimento com base nas velocidades suavizadas
        AplicarMovimento();
    }
}

    void ProcessUSBData(string data)
    {
        try
        {
            var jsonData = JsonUtility.FromJson<USBData>(data);
            velocidadeAlvoDireita = float.Parse(jsonData.direita[0]);
            velocidadeAlvoEsquerda = float.Parse(jsonData.esquerda[0]);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to process USB data: {ex.Message}");
        }
    }

    void AplicarMovimento()
    {
        // Verifica se uma das rodas está parada e ajusta a rotação
        if (velocidadeAtualDireita == 0f || velocidadeAtualEsquerda == 0f)
        {
            if (velocidadeAtualDireita == 0f && velocidadeAtualEsquerda != 0f)
            {
                float rotationSpeed = velocidadeAtualEsquerda * 2f;
                Quaternion rotation = Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
                transform.rotation = transform.rotation * rotation;
            }
            else if (velocidadeAtualEsquerda == 0f && velocidadeAtualDireita != 0f)
            {
                float rotationSpeed = velocidadeAtualDireita * 2f;
                Quaternion rotation = Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0);
                transform.rotation = transform.rotation * rotation;
            }
        }
        else
        {
            float velocidadeFrente = -(velocidadeAtualDireita + velocidadeAtualEsquerda) / 2f;
            float diferencaVelocidade = velocidadeAtualEsquerda - velocidadeAtualDireita;

            Vector3 move = transform.forward * velocidadeFrente * moveSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);

            float rotationSpeed = diferencaVelocidade * 2f;
            Quaternion rotation = Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
            transform.rotation = transform.rotation * rotation;
        }
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log($"Port {portName} closed.");
        }
    }

    [System.Serializable]
    public class USBData
    {
        public string[] direita;
        public string[] esquerda;
    }
}
