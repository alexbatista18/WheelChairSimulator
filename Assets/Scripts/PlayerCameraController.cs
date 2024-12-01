using Photon.Pun;
using UnityEngine;

public class PlayerCameraController : MonoBehaviourPun
{
    public Camera playerCamera;

    void Start()
    {
        // Verifica se este objeto pertence ao jogador local
        if (photonView.IsMine)
        {
            // Ativa a câmera apenas para o jogador local
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            // Desativa a câmera para outros jogadores
            playerCamera.gameObject.SetActive(false);
        }
    }
}
