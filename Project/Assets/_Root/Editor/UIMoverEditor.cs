using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(UIMover))]
// [CanEditMultipleObjects]
public class UIMoverEditor : Editor
{
    UIMover Target => target as UIMover;

    bool _allowMove = true;
    bool _allowResize = true;
    bool _allowClose = true;

    DrivenRectTransformTracker _tracker;

    private void OnEnable()
    {
        _tracker.Clear();
        _tracker.Add(this, Target.GetComponent<RectTransform>(),
            DrivenTransformProperties.AnchoredPosition |
            DrivenTransformProperties.Anchors |
            DrivenTransformProperties.Pivot |
            DrivenTransformProperties.SizeDeltaX);
        _tracker.Add(this, Target.parent,
            // DrivenTransformProperties.Anchors |
            DrivenTransformProperties.Pivot);
        Target.Resize();

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Target == null) Debug.Log("Target null");
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

