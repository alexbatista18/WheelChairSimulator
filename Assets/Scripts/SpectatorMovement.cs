using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
    public Camera camera1;  // Atribua a câmera que seguirá o Leader
    public Camera camera2;  // Atribua a câmera que seguirá o Joystick

    private GameObject leader;
    private GameObject joystick;

    public Vector3 offsetLeader = new Vector3(0, 5, -10); // Offset para o Leader
    public Vector3 offsetJoystick = new Vector3(0, 5, -10); // Offset para o Joystick

    public float followSpeed = 5f;

    void Start()
    {
        // Procura objetos com as tags "Leader" e "Joystick"
        leader = GameObject.FindWithTag("Leader");
        joystick = GameObject.FindWithTag("Joystick");

        if (leader == null || joystick == null)
        {
            Debug.LogError("Leader or Joystick not found!");
        }

        // Configura o viewport para dividir a tela
        // A câmera 1 ocupará a metade esquerda da tela
        camera1.rect = new Rect(0, 0, 0.5f, 1); 

        // A câmera 2 ocupará a metade direita da tela
        camera2.rect = new Rect(0.5f, 0, 0.5f, 1);
    }

    void Update()
    {
        if (leader != null)
        {
            // Calcula a posição desejada para a câmera 1 com base no Leader
            Vector3 targetPositionLeader = leader.transform.position + offsetLeader;
            // Movimenta a câmera 1 suavemente para seguir o Leader
            camera1.transform.position = Vector3.Lerp(camera1.transform.position, targetPositionLeader, followSpeed * Time.deltaTime);
            camera1.transform.LookAt(leader.transform); // Faz a câmera olhar para o Leader
        }

        if (joystick != null)
        {
            // Calcula a posição desejada para a câmera 2 com base no Joystick
            Vector3 targetPositionJoystick = joystick.transform.position + offsetJoystick;
            // Movimenta a câmera 2 suavemente para seguir o Joystick
            camera2.transform.position = Vector3.Lerp(camera2.transform.position, targetPositionJoystick, followSpeed * Time.deltaTime);
            camera2.transform.LookAt(joystick.transform); // Faz a câmera olhar para o Joystick
        }
    }
}
