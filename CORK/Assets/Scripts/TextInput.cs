using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TextInput : MonoBehaviour
{
    public InputField inputField;

    GameController controller;

    void Awake()
    {
        controller = GetComponent<GameController>();
        inputField.onEndEdit.AddListener(AcceptStringInput);
    }

    void Start()
    {
        inputField.text = null;
        inputField.Select();
        inputField.ActivateInputField();
    }

    void AcceptStringInput(string userInput)
    {
        // Only accept input when Enter is pressed. Ignore mouse clicks or other triggers.
        if (Keyboard.current == null || !Keyboard.current.enterKey.wasPressedThisFrame)
        {
            return;
        }

        // Prevent empty input submissions.
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return;
        }

        userInput = userInput.ToLower();
        string coloredInput = "<color=#00ff00>" + userInput + "</color>";

        controller.LogRawStringWithReturn("");
        controller.LogRawStringWithReturn(coloredInput);
        controller.LogRawStringWithReturn("");

        char[] delimiterCharacters = { ' ' };
        string[] separatedInputWords = userInput.Split(delimiterCharacters);

        bool handled = false;
        for (int i = 0; i < controller.inputActions.Length; i++)
        {
            InputAction inputAction = controller.inputActions[i];
            if (inputAction.keyWord == separatedInputWords[0])
            {
                inputAction.RespondToInput(controller, separatedInputWords);
                handled = true;
                break;
            }
        }

        if (!handled && separatedInputWords.Length > 0 && !string.IsNullOrWhiteSpace(separatedInputWords[0]))
        {
            controller.LogStringWithReturn("I don't know what you mean by \"" + separatedInputWords[0] + "\"");
        }

        InputComplete();

    }

    void InputComplete()
    {
        controller.DisplayLoggedText();
        inputField.ActivateInputField();
        inputField.text = null;
    }
}
