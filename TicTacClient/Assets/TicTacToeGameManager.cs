using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToeGameManager : MonoBehaviour
{
    private Button[] buttons;
    private string[] board;
    private string currentPlayer;

    public TMP_Text gameText; 

    void Start()
    {
        buttons = new Button[9];
        board = new string[9];
        currentPlayer = "X"; 

        for (int i = 0; i < 9; i++)
        {
            buttons[i] = GameObject.Find("Button" + i).GetComponent<Button>();
            int index = i; 
            buttons[i].onClick.AddListener(() => MakeMove(index));
            board[i] = ""; 
        }

        UpdateGameText(); 
    }

    public void MakeMove(int index)
    {
        if (board[index] != "") return;

        board[index] = currentPlayer;
        TMP_Text buttonText = buttons[index].GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = currentPlayer;
        }

        if (CheckForWinner())
        {
            gameText.text = $"{currentPlayer} Wins!";
            return;
        }

        if (IsBoardFull())
        {
            gameText.text = "It's a Draw!";
            return; 
        }

        currentPlayer = (currentPlayer == "X") ? "O" : "X";
        UpdateGameText(); 
    }

    bool CheckForWinner()
    {
        int[,] winConditions = new int[,]
        {
            { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 },
            { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 },
            { 0, 4, 8 }, { 2, 4, 6 }              
        };

        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            int a = winConditions[i, 0];
            int b = winConditions[i, 1];
            int c = winConditions[i, 2];

            if (board[a] == currentPlayer && board[b] == currentPlayer && board[c] == currentPlayer)
            {
                return true;
            }
        }

        return false;
    }

    bool IsBoardFull()
    {
        foreach (string cell in board)
        {
            if (cell == "")
                return false;
        }
        return true;
    }

    public void ResetGame()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i] = "";
            TMP_Text buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "";
            }
        }
        currentPlayer = "X"; 
        UpdateGameText(); 
    }

    private void UpdateGameText()
    {
        gameText.text = $"Player {currentPlayer}'s Turn";
    }
}
