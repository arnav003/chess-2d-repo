using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    //Reference from Unity IDE
    public GameObject chesspiece;
    public bool loadFromFenString = false;
    public string FenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 l";
    public string AIColour = "black";


    //Matrices needed, positions of each of the GameObjects
    //Also separate arrays for the players in order to easily keep track of them all
    //Keep in mind that the same objects are going to be in "positions" and "playerBlack"/"playerWhite"
    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    private int w = 0;
    private int b = 0;

    //current turn
    private string currentPlayer = "white";

    //Game Ending
    private bool gameOver = false;

    //Unity calls this right when the game starts, there are a few built in functions
    //that Unity can call for you
    public void Start()
    {
        if (loadFromFenString)
        {
            LoadPositionFromFen(FenString);

            //Set all piece positions on the positions board
            for (int i = 0; i < b; i++)
            {
                SetPosition(playerBlack[i]);
            }
            for (int i = 0; i < w; i++)
            {
                SetPosition(playerWhite[i]);
            }
        }
        else
        {
            playerWhite = new GameObject[] {
            Create("white_rook", 0, 0),
            Create("white_knight", 1, 0),
            Create("white_bishop", 2, 0),
            Create("white_queen", 3, 0),
            Create("white_king", 4, 0),
            Create("white_bishop", 5, 0),
            Create("white_knight", 6, 0),
            Create("white_rook", 7, 0),
            Create("white_pawn", 0, 1),
            Create("white_pawn", 1, 1),
            Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1),
            Create("white_pawn", 4, 1),
            Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1),
            Create("white_pawn", 7, 1) };

            playerBlack = new GameObject[] {
            Create("black_rook", 0, 7),
            Create("black_knight",1,7),
            Create("black_bishop",2,7),
            Create("black_queen",3,7),
            Create("black_king",4,7),
            Create("black_bishop",5,7),
            Create("black_knight",6,7),
            Create("black_rook",7,7),
            Create("black_pawn", 0, 6),
            Create("black_pawn", 1, 6),
            Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6),
            Create("black_pawn", 4, 6),
            Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6),
            Create("black_pawn", 7, 6) };

            //Set all piece positions on the positions board
            for (int i = 0; i < 16; i++)
            {
                SetPosition(playerBlack[i]);
                SetPosition(playerWhite[i]);
            }
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>(); //We have access to the GameObject, we need the script
        cm.name = name; //This is a built in variable that Unity has, so we did not have to declare it before
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate(); //It has everything set up so it can now Activate()
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();

        //Overwrites either empty space or whatever was there
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "white";
        }
    }

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            //Using UnityEngine.SceneManagement is needed here
            SceneManager.LoadScene("Game"); //Restarts the game by loading the scene over again
        }
    }

    public void Winner(string playerWinner)
    {
        gameOver = true;

        //Using UnityEngine.UI is needed here
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }

    public void LoadPositionFromFen(string fen)
    {
        var pieceTypeFromSymbol = new Dictionary<char, string> () {
            {'k', "king"},
            {'p', "pawn"},
            {'n', "knight"},
            {'b', "bishop"},
            {'r', "rook"},
            {'q', "queen"}
        };

        string fenBoard = fen.Split(' ')[0];
        int file = 0, rank = 7;

        foreach (char symbol in fenBoard)
        {
            if (symbol == '/')
            {
                file = 0;
                rank--;
            }
            else
            {
                if (char.IsDigit (symbol))
                {
                    file += (int) char.GetNumericValue (symbol);
                }
                else
            {
                    string pieceColour = (char.IsUpper (symbol)) ? "white" : "black";
                    string pieceType;
                    pieceTypeFromSymbol.TryGetValue(char.ToLower(symbol), out pieceType);
                    //pieceType = pieceTypeFromSymbol[char.ToLower(symbol)];
                    int xCoord = file;
                    int yCoord = rank;
                    string piece = pieceColour + "_" + pieceType;
                    if (pieceColour == "white")
                    {
                        playerWhite[w++] = Create(piece, xCoord, yCoord);
                    }
                    else
                    {
                        playerBlack[b++] = Create(piece, xCoord, yCoord);
                    }
                    file++;
                }
            }
        }
    }
}
/*
00, 10, ..... , 70
01, 11, ...
.
.
.
.
06, 16, ...
07, 17, ..... , 77
*/