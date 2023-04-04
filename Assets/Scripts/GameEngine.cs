using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
using Newtonsoft.Json;

public class GameEngine
{
    private const string DICTIONARY_PATH = "/Dictionary/Dictionary.json";
    private GameEngine() { }
    private static GameEngine _instance = null;
    public static GameEngine Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameEngine();
            }
            return _instance;
        }
    }

    public GameState State;
    public PlayerTurnTag TurnTag;
    private GameMod playMod;
    DataModel dataModel = DataModel.Instance;

    // các callback từ game engine
    public Action<string> UpdateOnPlayStringCallback;
    public Action<string> UpdateMessageCallback;

    public string Player2orBotStr;

    private bool isOnePlayerFailed = false;

    public static event Action<GameState> OnGameStateChange;
    private string[] stringsPool;
    private string _currentOnPlayString = "";
    public string currentOnPlayString
    {
        get { return _currentOnPlayString; }
        set
        {
            // variable tạm chứa currentOPStr, _currentOnPlayString sẽ dc cập nhật sau khi các condition hợp lệ
            var tempOnPlayString = value;

            //TODO: (tính điểm) & check condition cho state tiếp theo
            HandleGamePlayWhenGotNewInput(tempOnPlayString);
        }
    }

    private void HandleGamePlayWhenGotNewInput(string tempOnPlayString)
    {
        if ((this._currentOnPlayString != null && this._currentOnPlayString.Length > 0) && (tempOnPlayString != null && tempOnPlayString.Length > 0))
        {
            // update pool mỗi khi update OnPlayString
            var regex = new Regex(@"^" + tempOnPlayString);
            string[] newPool = this.stringsPool != null ? this.stringsPool.Where(c => regex.IsMatch(c)).ToArray() : null;
            if (newPool == null || newPool.Count() == 0)
            {
                // newPool rỗng -> player hiện tại lose & không update _currentOnPlayString, không update pool mới, player còn lại tự do hoàn thành từ
                HandleGotFailedPlayer();
            }
            else
            {
                HandleValidCaseAndCheckWinCondition(tempOnPlayString, newPool);
            }
        }
        // _currentOnPlayString trống -> get first theo chữ cái đầu tiên
        else
        {
            GetFirstPool(tempOnPlayString);
        }
        if (tempOnPlayString != null && tempOnPlayString.Length > 0)
        {
            SwitchPlayerTurn();
        }
    }

    private void HandleValidCaseAndCheckWinCondition(string tempOnPlayString, string[] newPool)
    {
        // nếu pool vẫn còn nhiều hơn 1 từ -> update pool đến khi nào pool chỉ còn lại 1 từ thì k update tiếp
        if (this.stringsPool.Count() > 1)
        {
            this.stringsPool = newPool;
        }
        // còn lại duy nhất 1 từ trong pool -> ktra win condition
        if (this.stringsPool.Count() == 1)
        {
            // tempOnPlayString match với từ duy nhất còn lại trong pool -> player hiện tại ghi điểm -> add từ vào remove pool
            if (tempOnPlayString == this.stringsPool[0])
            {
                // player hiện tại win ván đấu -> clear pool, _currentOnPlayString & bắt đầu lại ván với player thua đi trước (update to show trước r mới clear)
                UpdateGameState(GameState.EndMatch);
                this.isOnePlayerFailed = true;
            }
            // chưa match -> update _currentOnPlayString, k cần update pool và tiếp tục game bình thường
        }
        Update_currentOnPlayString(tempOnPlayString);
        // pool còn nhiều hơn 1 kết quả -> update _currentOnPlayString, pool(đã dc update ở trên) và tiếp tục game bình thường
    }

    private void HandleGotFailedPlayer()
    {
        if (!this.isOnePlayerFailed)
        {
            UpdateMessageCallback("Oops! " + (this.TurnTag == PlayerTurnTag.Player1 ? "Player 1" : Player2orBotStr) + " picked wrong charater!!! " + (this.TurnTag == PlayerTurnTag.Player1 ? Player2orBotStr : "Player 1") + " please complete longest word to win.");
            SwitchPlayerTurn();
            this.isOnePlayerFailed = true;
        }
        // player còn lại k thể hoàn thành -> game draw; end match call
        else
        {
            UpdateGameState(GameState.Draw);
        }
    }

    private void GetFirstPool(string tempOnPlayString)
    {
        this.stringsPool = (tempOnPlayString.Length == 1 ? dataModel.dictionary[tempOnPlayString] : null);
        Update_currentOnPlayString(tempOnPlayString);
        Debug.LogFormat(">>>>> Got [{0}] dictionary pool", _currentOnPlayString);
    }

    private void Update_currentOnPlayString(string value)
    {
        _currentOnPlayString = value;
        UpdateOnPlayStringCallback(_currentOnPlayString);
    }

    public void LoadDictionatyFromJsonFile()
    {
        try
        {
            if (dataModel.dictionary == null) 
            {
                string jsonPath = Application.dataPath + DICTIONARY_PATH;
                string jsonStr = File.ReadAllText(jsonPath);
                dataModel.dictionary = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(jsonStr);
                Debug.Log("Dictionary loaded");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Fail to load file. Error: " + e.Message);
        }
    }

    public void UpdateOpponentStringCall()
    {
        Player2orBotStr = (this.playMod == GameMod.Dual) ? "Player 2" : "Bot";
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Pause:
                HandleGamePause();
                break;
            case GameState.EndMatch:
                HandleMatchEnd();
                break;
            case GameState.Player1Turn:
                HandlePlayer1Turn();
                break;
            case GameState.Player2Turn:
                HandlePlayer2Turn();
                break;
            case GameState.Draw:
                HandleDraw();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChange?.Invoke(newState);
    }

    private bool CheckGameState(GameState state)
    {
        return state == this.State;
    }

    private void SwitchPlayerTurn()
    {

        if ((!this.isOnePlayerFailed) && (State == GameState.Player1Turn || State == GameState.Player2Turn))
        {
            if (this.TurnTag == PlayerTurnTag.Player1)
            {
                UpdateGameState(GameState.Player2Turn);
            }
            else
            {
                UpdateGameState(GameState.Player1Turn);
            }
        }
    }

    public void RestartGame()
    {
        this.stringsPool = null;
        currentOnPlayString = "";
        this.isOnePlayerFailed = false;
        UpdateGameState(TurnTag == PlayerTurnTag.Player1 ? GameState.Player2Turn : GameState.Player1Turn);
    }

    public void UpdateGameMod(GameMod mod)
    {
        this.playMod = mod;
    }

    public GameMod GetCurrentGameMod()
    {
        return this.playMod;
    }

    public string[] GetStringsPool()
    {
        return this.stringsPool;
    }

    public int Get_currentOnPlayStringLength()
    {
        return this._currentOnPlayString.Length;
    }

    // ---------- nhóm các method handle game state ------------------
    private void HandleGamePause()
    {
        Debug.Log(" ---- [on pause state]");
    }

    private void HandleMatchEnd()
    {
        Debug.Log(" ---- [on match end state]");
    }

    private void HandlePlayer1Turn()
    {
        Debug.Log(" ---- [on player 1 state]");
        this.TurnTag = PlayerTurnTag.Player1;

    }

    private void HandlePlayer2Turn()
    {
        Debug.Log(" ---- [on player 2 state]");
        this.TurnTag = PlayerTurnTag.Player2;
    }

    private void HandleDraw()
    {
        Debug.Log(" ---- [on draw state]");
    }
    // --------------------------------------------------------------------------
}

public enum GameState
{
    Pause,
    EndMatch,
    Player1Turn,
    Player2Turn,
    Draw
}

public enum PlayerTurnTag
{
    Player1,
    Player2
}

public enum GameMod
{
    Dual,
    BotEasy,
    BotCheat
}