using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class StringUnityEvent : UnityEvent<string> { }
[RequireComponent(typeof(InputField))]
public class InputFieldSubmit : MonoBehaviour
{
    public StringUnityEvent onSubmit;
    InputField inputField;
    private void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.lineType = InputField.LineType.MultiLineNewline;
    }

    private void OnEnable()
    {
        inputField.onValidateInput += CheckForEnter;
    }

    private void OnDisable()
    {
        inputField.onValidateInput -= CheckForEnter;
    }

    char CheckForEnter(string text, int charIndex, char addedChar)
    {
        if (addedChar == '\n' && onSubmit != null)
        {
            onSubmit.Invoke(text);
            return '\0';
        }
        else
        {
            return addedChar;
        }
    }
}
