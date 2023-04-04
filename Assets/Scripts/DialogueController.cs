using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueController : MonoBehaviour
{
    public Text textContent;
    public Button btnYes;
    public Button btnNo;
    public float delayTime;

    public Action btnYesOnClickCallback;
    public Action btnNoOnClickCallback;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        btnNo.onClick.AddListener(delegate () { BtnNoBehavior(); });
        btnYes.onClick.AddListener(delegate () { BtnYesBehavior(); });
    }


    public void ShowDialog(string content)
    {
        textContent.text = content;
    }

    private void BtnNoBehavior()
    {
        // return to menu scene
        StartCoroutine(btnNoDoAction());
    }

    private void BtnYesBehavior()
    {   
        // RestartGame
        StartCoroutine(btnYesDoAction());
    }

    IEnumerator btnYesDoAction()
    {
        yield return new WaitForSeconds(delayTime);
        btnYesOnClickCallback();
    }

    IEnumerator btnNoDoAction()
    {
        yield return new WaitForSeconds(delayTime);
        btnNoOnClickCallback();
        SceneManager.LoadScene(0);
    }
}
