using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClientUIManager : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    private NetworkClient clientInstance;
    private GameStateManager gameStateManager;

    public GameObject waitingUI;

    public GameObject observerPanel;

    void Start()
    {
        clientInstance = FindObjectOfType<NetworkClient>();
        gameStateManager = FindObjectOfType<GameStateManager>();

        if (clientInstance == null)
        {
            Debug.LogError("NetworkClient instance not found. Please ensure it is in the scene.");
        }

        if (gameStateManager == null)
        {
            Debug.LogError("GameStateManager instance not found. Please ensure it is in the scene.");
        }
    }

    public void OnLoginButtonClicked()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        Debug.Log($"Login Button Clicked! Username: {username}, Password: {password}");
        clientInstance.SendLoginRequest(username, password);
    }

    public void OnCreateAccountButtonClicked()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        Debug.Log($"Create Account Button Clicked! Username: {username}, Password: {password}");
        clientInstance.SendCreateAccountRequest(username, password);
    }

    public void HandleServerResponse(string response)
    {
        switch (response)
        {
            case "LOGIN_SUCCESS":
                Debug.Log("Login successful!");
                if (gameStateManager != null)
                {
                    gameStateManager.ChangeState(GameState.Room);  
                }
                break;
            case "LOGIN_FAILED":
                Debug.Log("Login failed. Please check your credentials.");
                break;
            case "ACCOUNT_CREATION_SUCCESS":
                Debug.Log("Account created successfully!");
                break;
            case "ACCOUNT_CREATION_FAILED":
                Debug.Log("Account creation failed. Username may already exist.");
                break;
            default:
                Debug.Log("Unknown response: " + response);
                break;
        }
    }

    public void ShowWaitingForOpponentUI()
    {
        waitingUI.SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        clientInstance.LeaveRoom();

        gameStateManager.ChangeState(GameState.Room);
    }

    public void ShowObserverUI()
    {
        if (observerPanel != null)
        {
            observerPanel.SetActive(true);
        }
    }
}
