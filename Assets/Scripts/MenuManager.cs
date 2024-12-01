using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject panel;
    public Button menuButton;
    public Button startHostButton;
    public Button startClientButton;
    public Button closeButton;
    private NetworkManager networkManager;

    void Start()
    {
        // Verificar se todas as referências foram atribuídas
        if (panel == null || menuButton == null || startHostButton == null || startClientButton == null || closeButton == null)
        {
            Debug.LogError("Uma ou mais referências não foram atribuídas no Inspector.");
            return;
        }

        // Esconder o painel no início
        panel.SetActive(false);

        // Obter referência ao NetworkManager
        networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager não encontrado na cena.");
            return;
        }

        // Adicionar listeners aos botões
        menuButton.onClick.AddListener(OnMenuClicked);
        startHostButton.onClick.AddListener(OnStartHostClicked);
        startClientButton.onClick.AddListener(OnStartClientClicked);
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    public void OnMenuClicked()
    {
        // Mostrar o painel quando o menu é clicado
        panel.SetActive(true);
    }

    private void OnStartHostClicked()
    {
        Debug.Log("Start Host Button Clicked");
        networkManager.StartHost(); // Chamando o método StartHost do NetworkManager
        panel.SetActive(false);
    }

    private void OnStartClientClicked()
    {
        Debug.Log("Start Client Button Clicked");
        networkManager.StartClient(); // Chamando o método StartClient do NetworkManager
        panel.SetActive(false);
    }

    private void OnCloseClicked()
    {
        Debug.Log("Close Button Clicked");
        panel.SetActive(false);
    }
}
