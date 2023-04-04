using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameEngine gameEngine = GameEngine.Instance;
    BotAI botAI = BotAI.Instance;
    private string BotResponseStr;
    private bool waitFlag;
    public float botWaitTime = 0.8f;

    public Text onPlayText;
    public Text gameMessage;
    public Text playerTurnMessage;
    public Text inputCharacter;
    public GameObject CustomKeyboard;
    ScaleLerpingAnimator inputTextScaleLerpingAnimator;
    public GameObject Dialogue;
    DialogueController dialogueController;

    //on test variable
    private string _inputChar;
    public string inputChar
    {
        get { return _inputChar; }
        set
        {
            _inputChar = value;
            inputCharacter.text = _inputChar;
            inputTextScaleLerpingAnimator.DoScaleAnimator();
            gameEngine.currentOnPlayString += _inputChar;
            onPlayText.text = gameEngine.currentOnPlayString;
        }
    }

    void Awake()
    {
        Application.targetFrameRate = 30;
        GameEngine.OnGameStateChange += GameManagerOnOnGameStateChanged;
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        GameEngine.OnGameStateChange -= GameManagerOnOnGameStateChanged;
    }

    // chứa các method cần trigger khi game state change
    private void GameManagerOnOnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Player1Turn:
                ToggleCustomKeyboard(true);
                playerTurnMessage.text = "Player 1's Turn!";
                break;
            case GameState.Player2Turn:
                this.TakeCarePlayer2Turn();
                break;
            case GameState.EndMatch:
                ShowDialog("Congratulations " + (gameEngine.TurnTag == PlayerTurnTag.Player1 ? "Player1!" : gameEngine.Player2orBotStr) + "! You won this game.\nDo you want to play the game again?");
                break;
            case GameState.Draw:
                ShowDialog("Oops! " + (gameEngine.TurnTag == PlayerTurnTag.Player1 ? "Player1" : gameEngine.Player2orBotStr) + " was unable to complete the longest word. This game ended in a draw.\nDo you want to play the game again?");
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.BotResponseStr = null;

        // gameEngine.LoadDictionatyFromJsonFile();
        gameEngine.UpdateOnPlayStringCallback = (value) => { onPlayText.text = value; };
        gameEngine.UpdateMessageCallback = (value) => { gameMessage.text = value; };
        gameEngine.UpdateOpponentStringCall();
        this.waitFlag = false;

        inputTextScaleLerpingAnimator = inputCharacter.GetComponent<ScaleLerpingAnimator>();

        DialogueSetup();

        gameEngine.UpdateGameState(GameState.Player1Turn);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        this.waitFlag = !(this.BotResponseStr == null);
    }


    public void ReceiveInputCharacter(string _char)
    {
        this.inputChar = _char;
    }

    public void ShowDialog(string content)
    {
        Dialogue.SetActive(true);
        dialogueController.ShowDialog(content);
        this.ToggleCustomKeyboard(false);

    }

    private void RestartGame()
    {
        inputCharacter.text = "";
        gameMessage.text = "";
        gameEngine.RestartGame();
        this.ToggleCustomKeyboard(true);
    }

    private void DialogueSetup()
    {
        Dialogue.SetActive(false);
        dialogueController = Dialogue.GetComponent<DialogueController>();
        dialogueController.btnYesOnClickCallback = () => { Dialogue.SetActive(false); this.RestartGame(); };
        dialogueController.btnNoOnClickCallback = () => { this.RestartGame(); };
    }

    private void TakeCarePlayer2Turn()
    {
        playerTurnMessage.text = gameEngine.Player2orBotStr + "'s Turn!";
        // không phải dual mod thì bot sẽ handle lượt chơi của player 2, disable bàn phím lun cho chắc
        if (gameEngine.GetCurrentGameMod() != GameMod.Dual) 
        {
            this.ToggleCustomKeyboard(false);
            StartCoroutine(botPlay());
        }
    }

    IEnumerator botPlay()
    {
        while (gameEngine.State == GameState.Player2Turn)
        {
            yield return new WaitForSeconds(botWaitTime);
            this.BotResponseStr = botAI.ReturnACharacter();
            yield return new WaitUntil(() => this.waitFlag);
            ReceiveInputCharacter(this.BotResponseStr);
            this.BotResponseStr = null;
            yield return null;
        }
    }

    private void ToggleCustomKeyboard(bool onOff)
    {
        if (CustomKeyboard.activeSelf != onOff)
        {
            CustomKeyboard.SetActive(!CustomKeyboard.activeSelf);
        }
    }
}

