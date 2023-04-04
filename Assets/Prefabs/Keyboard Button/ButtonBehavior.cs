using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour
{
    private Animator animator;
    KeyboardController keyboardController;
    // Start is called before the first frame update
    void Start()
    {
        GameObject keyboard = GameObject.FindGameObjectWithTag("KeyboardController");
        keyboardController = keyboard.GetComponent<KeyboardController>();

        animator = gameObject.GetComponent<Animator>();
    }


    public void OnClick()
    {
        string _char = this.GetComponentInChildren<Text>().text;
        // Debug.Log("button " + _char + " on click!");

        animator.Play("OnClickAnimation");

        keyboardController.ReceiveInputCharacter(_char);
    }
}
