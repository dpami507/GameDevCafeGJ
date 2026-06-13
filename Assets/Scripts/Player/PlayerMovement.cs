using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed;

    InputAction playerAction;
    Vector2 playerMoveInput;

    Rigidbody2D rb;
    Vector2 dir;

    // Visual
    [Header("Visual")]
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] bool flipSprite;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        playerAction = InputSystem.actions.FindAction("Move");
    }
    private void Update()
    {
        //Player Movement
        playerMoveInput = playerAction.ReadValue<Vector2>();
        rb.linearVelocity = playerMoveInput.normalized * movementSpeed * UpgradeManager.instance.GetUpgradeValue(UpgradeTypes.MovementSpeed);

        if (GameManager.instance.gameOver)
            rb.linearVelocity = Vector2.zero;

        // Get direction and hold if not moving
        if(Mathf.Abs(playerMoveInput.x) > 0.05f)
            dir = playerMoveInput;

        // Animation
        animator.SetBool("moving", (playerMoveInput != Vector2.zero));

        // Sprite Renderer
        if(dir.x > 0.1)
            spriteRenderer.flipX = true ^ flipSprite;
        else if (dir.x < -0.1)
            spriteRenderer.flipX = false ^ flipSprite;
        else
            spriteRenderer.flipX = spriteRenderer.flipX;
    }
}
