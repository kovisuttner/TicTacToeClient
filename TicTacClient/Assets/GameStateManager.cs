using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateManager : MonoBehaviour
{
    public GameState currentState;

    public GameObject mainMenuPanel;
    public GameObject roomPanel;
    public GameObject gamePanel;

    public TMP_InputField roomNameInputField;
    public GameObject waitingForOpponentPanel;

    void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        UpdateUI();
    }

    private void UpdateUI()
    {
        mainMenuPanel.SetActive(currentState == GameState.MainMenu);
        roomPanel.SetActive(currentState == GameState.Room);
        gamePanel.SetActive(currentState == GameState.Game);

    }

    public void OnPlayButtonClicked()
    {
        ChangeState(GameState.Game);  
    }

    public void OnCreateOrJoinRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log("Room name is required!");
            return;
        }

        NetworkClient clientInstance = FindObjectOfType<NetworkClient>();
        if (clientInstance != null)
        {
            clientInstance.SendJoinOrCreateRoomRequest(roomName);
        }
        else
        {
            Debug.LogError("NetworkClient not found!");
        }

        ChangeState(GameState.Room);
    }

}
