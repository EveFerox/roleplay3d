using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

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

    // public bool

    private bool _move = false;
    private bool _resize = false;

    private Vector2 _point;
    private Vector2 _pointLast;

    private void Awake() {
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
    }

    public void Validate() {
        parent.pivot = Vector2.zero;
        parent.anchorMin = new Vector2(0.0f, 0.0f);
        parent.anchorMax = new Vector2(0.0f, 0.0f);
    }

    private void Update() {
        if (_move && allowMove) {
            Vector3 delta = _pointLast - _point;
            _pointLast = _point;

            var bounds = canvas.sizeDelta - parent.sizeDelta;
            parent.position += delta * -1;
            parent.position = new Vector3(
                Mathf.Clamp(parent.position.x, 0, bounds.x),
                Mathf.Clamp(parent.position.y, 0, bounds.y));
        }
        if (_resize && allowResize) {
            parent.sizeDelta -= _pointLast - _point;
            _pointLast = _point;
        }
    }
}
