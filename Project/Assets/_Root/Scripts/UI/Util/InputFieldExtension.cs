﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldExtension : MonoBehaviour
{
    [SerializeField]
    bool _isRefocusOnSubmit = true;

    public UnityEvent Submit;
    
    bool _isPrevFrameFocus = false;
    InputField _inputField;

    void Awake()
    {
        _inputField = GetComponent<InputField>();
    }

    void Update()
    {
        if (_isPrevFrameFocus && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))) {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) 
                Submit.Invoke();

            if (_isRefocusOnSubmit) 
                Focus();
        }

        _isPrevFrameFocus = _inputField.isFocused;
    }

    public void Focus()
    {
        _inputField.Select();
        _inputField.ActivateInputField();
    }
}
