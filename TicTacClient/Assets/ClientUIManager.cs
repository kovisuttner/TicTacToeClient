using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClientUIManager : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    public NetworkClient clientInstance;

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
}
