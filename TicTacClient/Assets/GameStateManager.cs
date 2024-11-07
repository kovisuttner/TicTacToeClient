using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState currentState;

    public GameObject mainMenuPanel;
    public GameObject roomPanel;
    public GameObject gamePanel;

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

}
