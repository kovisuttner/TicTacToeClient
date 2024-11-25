using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToeGameManager : MonoBehaviour
{
    [HideInInspector]
    public Button[] buttons;
    [HideInInspector]
    public string[] board;
    [HideInInspector]
    public string currentPlayer;

    public TMP_Text gameText;

    private NetworkClient clientInstance;

    void Start()
    {
        clientInstance = FindObjectOfType<NetworkClient>();

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

        clientInstance.SendMove(index, currentPlayer);

        if (CheckForWin())
        {
            gameText.text = currentPlayer + " Wins!";
        }
        else
        {
            currentPlayer = currentPlayer == "X" ? "O" : "X";
            UpdateGameText();
        }
    }


    private void UpdateGameText()
    {
        gameText.text = "Player " + currentPlayer + "'s Turn";
    }

    private bool CheckForWin()
    {
        int[][] winPatterns = new int[][]
        {
            new int[] { 0, 1, 2 },
            new int[] { 3, 4, 5 },
            new int[] { 6, 7, 8 },
            new int[] { 0, 3, 6 },
            new int[] { 1, 4, 7 },
            new int[] { 2, 5, 8 },
            new int[] { 0, 4, 8 },
            new int[] { 2, 4, 6 }
        };

        foreach (var pattern in winPatterns)
        {
            if (board[pattern[0]] == board[pattern[1]] && board[pattern[1]] == board[pattern[2]] && board[pattern[0]] != "")
            {
                return true;
            }
        }

        return false;
    }
}
