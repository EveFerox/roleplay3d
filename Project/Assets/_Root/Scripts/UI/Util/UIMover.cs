using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(LayoutElement))]
public class UIMover : MonoBehaviour
{
    public RectTransform canvas;
    public RectTransform parent;
    public HoldButton moveButton;
    public HoldButton resizeButton;
    public Button closeButton;

    public bool allowMove = true;
    public bool allowResize = true;
    public bool allowClose = true;

    RectTransform _rect;
    public RectTransform rect => _rect != null ? _rect : (_rect = GetComponent<RectTransform>());

    bool _move = false;
    bool _resize = false;

    Vector2 _point;
    Vector2 _pointLast;

    void Awake()
    {
        if (allowMove) {
            moveButton.gameObject.SetActive(allowMove);

            moveButton.onDown.AddListener(() => { _move = true; _pointLast = _point; });
            moveButton.onUp.AddListener(() => _move = false);
        }

        if (allowResize) {
            resizeButton.gameObject.SetActive(allowMove);

            resizeButton.onDown.AddListener(() => { _resize = true; _pointLast = _point; });
            resizeButton.onUp.AddListener(() => _resize = false);
        }

        if (allowClose) {
            closeButton.onClick.AddListener(() => parent.gameObject.SetActive(false));
        }

        FindObjectOfType<InputSystemUIInputModule>().point.action.performed += ctx => _point = ctx.ReadValue<Vector2>();

        Resize();

        Debug.Log(parent.anchoredPosition);
    }

    private void Update()
    {
        if (_move && allowMove) {
            var delta = _pointLast - _point;
            if (delta == Vector2.zero) return;
            _pointLast = _point;
            var bounds = canvas.sizeDelta - parent.sizeDelta;
            parent.anchoredPosition += delta * -1;
            parent.anchoredPosition = new Vector2(
                 Mathf.Clamp(parent.anchoredPosition.x, 0, bounds.x),
                 Mathf.Clamp(parent.anchoredPosition.y, 0, bounds.y));
        }
        if (_resize && allowResize) {
            parent.sizeDelta -= _pointLast - _point;
            _pointLast = _point;
        }
    }

    public void Resize()
    {
        var size = rect.sizeDelta.y * ((allowMove ? 1 : 0) + (allowResize ? 1 : 0) + (allowClose ? 1 : 0));
        rect.pivot = Vector2.zero;
        rect.anchorMin = new Vector2(1.0f, 1.0f);
        rect.anchorMax = new Vector2(1.0f, 1.0f);
        rect.sizeDelta = new Vector2(size, rect.sizeDelta.y);
        rect.anchoredPosition3D = -new Vector3(size, rect.sizeDelta.y);
        LayoutRebuilder.MarkLayoutForRebuild(rect);
        parent.pivot = Vector2.zero;
        // parent.anchorMin = new Vector2(0.0f, 0.0f);
        // parent.anchorMax = new Vector2(0.0f, 0.0f);
        if (GetComponent<LayoutElement>() is LayoutElement elem) elem.ignoreLayout = true;
    }
}

