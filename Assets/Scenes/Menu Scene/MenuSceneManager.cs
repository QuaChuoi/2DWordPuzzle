using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSceneManager : MonoBehaviour
{
    public float delayTime = 0.8f;
    GameEngine gameEngine = GameEngine.Instance;

    public Button btnPlayDual;
    public Button btnPlayVsBot;
    public Button btnPlayVsBotEasy;
    public Button btnPlayVsBotHard;
    public GameObject difficultField;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        Application.targetFrameRate = 30;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameEngine.LoadDictionatyFromJsonFile();
        difficultField.SetActive(false);
    }

    private void LoadScreen(){
        StartCoroutine(LoadMainScreen());
    }

    IEnumerator LoadMainScreen()
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(1);
    }

    public void BtnDualBehavior()
    {
        gameEngine.UpdateGameMod(GameMod.Dual);
        this.LoadScreen();
    }

    public void BtnVsBotBehavior()
    {
        difficultField.SetActive(true);
    }

    public void BtnVsBotEasyBehavior()
    {
        gameEngine.UpdateGameMod(GameMod.BotEasy);
        this.LoadScreen();
    }

    public void BtnVsBotHardBehavior()
    {
        gameEngine.UpdateGameMod(GameMod.BotCheat);
        this.LoadScreen();
    }
}
