using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction = Vector2.down;
    public float speed = 5f;
    public Joystick movermentJoystick;

    [Header("Sprites")]
    public AnimatedSpriteRenderer spriteRendererUp;
    public AnimatedSpriteRenderer spriteRendererDown;
    public AnimatedSpriteRenderer spriteRendererLeft;
    public AnimatedSpriteRenderer spriteRendererRight;
    public AnimatedSpriteRenderer spriteRendererDeath;
    private AnimatedSpriteRenderer activeSpriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        activeSpriteRenderer = spriteRendererDown;
    }

    private void Update()
    {
        Vector2 inputDirection = new Vector2(movermentJoystick.Horizontal, movermentJoystick.Vertical).normalized;

        if (inputDirection != Vector2.zero)
        {
            if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
            {
                if (inputDirection.x > 0)
                {
                    SetDirection(Vector2.right, spriteRendererRight);
                }
                else
                {
                    SetDirection(Vector2.left, spriteRendererLeft);
                }
            }
            else
            {
                if (inputDirection.y > 0)
                {
                    SetDirection(Vector2.up, spriteRendererUp);
                }
                else
                {
                    SetDirection(Vector2.down, spriteRendererDown);
                }
            }
        }
        else
        {
            SetDirection(Vector2.zero, activeSpriteRenderer);
        }

        direction = inputDirection;
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        Vector2 translation = speed * Time.fixedDeltaTime * direction;

        rb.MovePosition(position + translation);
    }

    private void SetDirection(Vector2 newDirection, AnimatedSpriteRenderer spriteRenderer)
    {
        direction = newDirection;

        spriteRendererUp.enabled = spriteRenderer == spriteRendererUp;
        spriteRendererDown.enabled = spriteRenderer == spriteRendererDown;
        spriteRendererLeft.enabled = spriteRenderer == spriteRendererLeft;
        spriteRendererRight.enabled = spriteRenderer == spriteRendererRight;

        activeSpriteRenderer = spriteRenderer;
        activeSpriteRenderer.idle = direction == Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            DeathSequence();
        }
    }

    private void DeathSequence()
    {
        enabled = false;
        GetComponent<BombController>().enabled = false;

        spriteRendererUp.enabled = false;
        spriteRendererDown.enabled = false;
        spriteRendererLeft.enabled = false;
        spriteRendererRight.enabled = false;
        spriteRendererDeath.enabled = true;

        Invoke(nameof(OnDeathSequenceEnded), 1.25f);
    }

    private void OnDeathSequenceEnded()
    {
        gameObject.SetActive(false);
        LoadGameOverUI();
        //GameManager.Instance.CheckWinState();
    }
    public GameObject gameOverUI;
    protected void LoadGameOverUI()
    {
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
    }
}
