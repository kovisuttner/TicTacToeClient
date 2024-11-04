using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject accountCreationPanel;

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        accountCreationPanel.SetActive(false);
    }

    public void ShowAccountCreationPanel()
    {
        loginPanel.SetActive(false);
        accountCreationPanel.SetActive(true);
    }
}
