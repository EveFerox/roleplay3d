using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIMover))]
// [CanEditMultipleObjects]
public class UIMoverEditor : Editor
{
    UIMover Target => target as UIMover;

    bool _allowMove = true;
    bool _allowResize = true;
    bool _allowClose = true;

    Editor _rectTransform;

    private void OnEnable() {
        var ass = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var rtEditor = ass.GetType("UnityEditor.RectTransformEditor");
        _rectTransform = CreateEditor(Target.GetComponent<RectTransform>(), rtEditor);
        // UnityEditor.AnimatedValues.AnimBool
        
    }


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Validate")) {
            Target.Validate();
        }
        if (_allowMove != Target.allowMove) {
            _allowMove = !_allowMove;
            Target.moveButton.gameObject.SetActive(_allowMove);
            Target.Resize();
        }
        if (_allowResize != Target.allowResize) {
            _allowResize = !_allowResize;
            Target.resizeButton.gameObject.SetActive(_allowResize);
            Target.Resize();
        }
        if (_allowClose != Target.allowClose) {
            _allowClose = !_allowClose;
            Target.closeButton.gameObject.SetActive(_allowClose);
            Target.Resize();
        }        
    }
}

