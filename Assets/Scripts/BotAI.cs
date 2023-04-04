using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random=UnityEngine.Random;

public class BotAI
{
    private BotAI() { }
    private static BotAI _instance = null;
    public static BotAI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BotAI();
            }
            return _instance;
        }
    }

    GameEngine gameEngine = GameEngine.Instance;

    private const string ALPHABETSTRING = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private string GetRandomAlphabet()
    {
        return Char.ToString(ALPHABETSTRING[Random.Range(0,ALPHABETSTRING.Length-1)]);
    }

    private string CheatToGetCharacterFormDataPool()
    {
        try 
        {
        if (gameEngine.GetStringsPool() != null || gameEngine.GetStringsPool().Count() == 0)
        {
            var randomeStr =  gameEngine.GetStringsPool().GetRandom();
            Debug.Log("cheat bot catch: " + randomeStr);
            var _char = (randomeStr.Length > gameEngine.Get_currentOnPlayStringLength()) ? Char.ToString(randomeStr[gameEngine.Get_currentOnPlayStringLength()]) : null;
            if (_char != null ) 
            {
                return _char;
            } 
            else 
            {
                return this.GetRandomAlphabet();
            }
        } 
        } 
        catch (Exception e)
        {
            Debug.Log("Bot Fail to generate a character. Error: " + e.Message);
        }
        return this.GetRandomAlphabet();
    }

    public string ReturnACharacter()
    {
        switch (gameEngine.GetCurrentGameMod())
        {
            case GameMod.BotEasy:
                return this.GetRandomAlphabet();
            case GameMod.BotCheat:
                return this.CheatToGetCharacterFormDataPool();
            default:
                Debug.Log("Bot got error! check on game mod status");
                return this.GetRandomAlphabet();
        }
    }
}

public static class ArrayExtensions 
{
    public static T GetRandom<T>(this T[] array) 
    {
        return array[Random.Range(0, array.Length)];
    }
}
