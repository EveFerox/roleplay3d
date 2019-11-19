using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class BasicPlayerController : MonoBehaviour
{

    public float speed = 0.1f;
    public float mouse = 0.1f;

    public Transform Rotate;

    private CharacterController _controller;

    private Vector2 _move = Vector2.zero;
    private Vector2 _view = Vector2.zero;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        _move = ctx.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext ctx)
    {
        _view = ctx.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        var move = transform.right * _move.x + transform.forward * _move.y;
        _controller.Move(move * speed);

        Rotate.Rotate(Vector3.left * _view.y * mouse);
        transform.Rotate(Vector3.up * _view.x * mouse);
    }
}
