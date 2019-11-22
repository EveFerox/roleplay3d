using UnityEngine;
using UnityEngine.InputSystem.UI;

public class UIMover : MonoBehaviour
{
    public RectTransform canvas;
    public RectTransform parent;
    public HoldButton moveButton;

    public bool validate;

    // public bool

    private bool _move = false;
    private Vector2 _point;
    private Vector2 _pointLast;

    private void Awake()
    {
        moveButton.onDown.AddListener(() => { _move = true; _pointLast = _point; });
        moveButton.onUp.AddListener(() => _move = false);

        FindObjectOfType<InputSystemUIInputModule>().point.action.performed += ctx => _point = ctx.ReadValue<Vector2>();
    }

    private void OnValidate()
    {
        parent.pivot = Vector2.zero;
        parent.anchorMin = new Vector2(0.5f, 0.5f);
        parent.anchorMax = new Vector2(0.5f, 0.5f);
    }

    private void Update()
    {
        if (_move)
        {
            Vector3 delta = _pointLast - _point;
            _pointLast = _point;

            var bounds = canvas.sizeDelta - parent.sizeDelta;
            parent.position += delta * -1;
            parent.position = new Vector3(
                Mathf.Clamp(parent.position.x, 0, bounds.x),
                Mathf.Clamp(parent.position.y, 0, bounds.y));
        }
    }
}