using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIMover))]
public class UIMoverEditor : Editor
{
    UIMover Target => target as UIMover;

    bool _allowMove = true;
    bool _allowResize = true;
    bool _allowClose = true;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Validate")) {
            Target.Validate();
        }
        if (_allowMove != Target.allowMove) {
            _allowMove = !_allowMove;
            Target.moveButton.gameObject.SetActive(_allowMove);
        }
        if (_allowResize != Target.allowResize) {
            _allowResize = !_allowResize;
            Target.resizeButton.gameObject.SetActive(_allowResize);
        }
        if (_allowClose != Target.allowClose) {
            _allowClose = !_allowClose;
            Target.closeButton.gameObject.SetActive(_allowClose);
        }
    }
}

