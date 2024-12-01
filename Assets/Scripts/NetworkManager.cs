using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject[] playerPrefabs; // Array dos prefabs dos jogadores (capsules coloridas)
    private string roomName = "MyRoom"; // Nome da sala

    void Start()
    {
        Debug.Log("Conectando ao Photon Master Server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao Photon Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Entrou no Lobby");
    }

    public void StartHost()
    {
        Debug.Log("Iniciando como Host...");
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 4 });
        }
        else
        {
            Debug.LogError("PhotonNetwork não está conectado e pronto.");
        }
    }

    public void StartClient()
    {
        Debug.Log("Tentando entrar na sala...");
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.LogError("PhotonNetwork não está conectado e pronto.");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou na sala. Nome da sala: " + PhotonNetwork.CurrentRoom.Name);
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // Índice do jogador baseado no número de ator local

        Debug.Log($"PlayerIndex: {playerIndex}, PlayerPrefabs Length: {playerPrefabs.Length}");

        if (playerIndex < 0 || playerIndex >= playerPrefabs.Length)
        {
            Debug.LogError("Índice de jogador fora dos limites ou prefabs de jogador não definidos corretamente.");
            return;
        }

        GameObject prefab = playerPrefabs[playerIndex];
        if (prefab == null)
        {
            Debug.LogError("Prefab do jogador não está definido.");
            return;
        }

        Vector3[] spawnPositions = new Vector3[]
        {
            new Vector3(10.39f, 0.35f, -5.09f),
            new Vector3(11.448f, 0.35f, -5.09f),
            new Vector3(12.54f, 3.0f, -8.0f),
            new Vector3(13.66f, 0.35f, -5.09f)
        };

        Quaternion[] spawnRotations = new Quaternion[]
        {
            Quaternion.Euler(0, 0, 0),
            Quaternion.Euler(0, 0, 0),
            Quaternion.Euler(22, 0, 0),
            Quaternion.Euler(0, 0, 0)
        };

        if (playerIndex >= spawnPositions.Length || playerIndex >= spawnRotations.Length)
        {
            Debug.LogError("Índice de spawn fora dos limites.");
            return;
        }

        // Instanciar o jogador na posição e rotação definidas
        GameObject player = PhotonNetwork.Instantiate(prefab.name, spawnPositions[playerIndex], spawnRotations[playerIndex]);

        // Caso precise inicializar ou sincronizar algum componente específico, faça isso aqui
    }
}
