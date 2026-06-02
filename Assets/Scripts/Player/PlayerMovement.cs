using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;

    InputAction playerAction;
    Vector2 playerMoveInput;

    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        playerAction = InputSystem.actions.FindAction("Move");
    }
    private void Update()
    {
        //Player Movement
        playerMoveInput = playerAction.ReadValue<Vector2>();
        rb.linearVelocity = playerMoveInput.normalized * movementSpeed;
    }
}
