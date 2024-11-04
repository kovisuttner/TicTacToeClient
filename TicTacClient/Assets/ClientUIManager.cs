using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClientUIManager : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    private NetworkClient clientInstance;

    void Start()
    {
        clientInstance = FindObjectOfType<NetworkClient>();
        if (clientInstance == null)
        {
            Debug.LogError("NetworkClient instance not found. Please ensure it is in the scene.");
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
        }
    }
}
