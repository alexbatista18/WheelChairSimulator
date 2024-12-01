using Photon.Pun;
using UnityEngine;

public class PlayerCameraControllerSpectator : MonoBehaviourPun
{
    public Camera playerCamera1; // Câmera do jogador 1 (Leader)
    public Camera playerCamera2; // Câmera do jogador 2 (Joystick)
    
    private GameObject leaderPlayer;
    private GameObject joystickPlayer;
    
    public Vector3 cameraOffset = new Vector3(0, 2, -4); // Offset da câmera

    void Start()
    {
        // Ativa as câmeras apenas para o espectador (photonView.IsMine)
        if (photonView.IsMine)
        {
            playerCamera1.gameObject.SetActive(true);
            playerCamera2.gameObject.SetActive(true);

            // Encontra os jogadores por tag
            leaderPlayer = GameObject.FindWithTag("Leader");
            joystickPlayer = GameObject.FindWithTag("Joystick");

            // Verifica se os objetos foram encontrados
            if (leaderPlayer == null || joystickPlayer == null)
            {
                Debug.LogError("Leader or Joystick not found!");
            }
        }
        else
        {
            playerCamera1.gameObject.SetActive(false);
            playerCamera2.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Faz com que a Câmera 1 siga o jogador "Leader"
        if (leaderPlayer != null)
        {
            playerCamera1.transform.position = leaderPlayer.transform.position + cameraOffset;
            playerCamera1.transform.LookAt(leaderPlayer.transform);
        }

        // Faz com que a Câmera 2 siga o jogador "Joystick"
        if (joystickPlayer != null)
        {
            playerCamera2.transform.position = joystickPlayer.transform.position + cameraOffset;
            playerCamera2.transform.LookAt(joystickPlayer.transform);
        }
    }
}
