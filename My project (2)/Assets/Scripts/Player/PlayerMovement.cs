using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX;
    [SerializeField] private float wallJumpY;
    [SerializeField] private float wallJumpLockTime = 0.2f;
    [SerializeField] private float wallSlideSpeed   = 2f;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    [Header("Physics")]
    [SerializeField] private float airGravityScale = 7f;

    private static readonly int RunHash      = Animator.StringToHash("run");
    private static readonly int GroundedHash = Animator.StringToHash("grounded");
    private static readonly int JumpHash     = Animator.StringToHash("jump");

    private Rigidbody2D   _body;
    private Animator      _anim;
    private BoxCollider2D _boxCollider;
    private Vector3       _originalScale;

    private float _wallJumpCooldown = 999f;
    private float _horizontalInput;
    private float _facingSign = 1f;
    private bool  _played;
    private bool  _isJumping;

    private InputAction _moveAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _body          = GetComponent<Rigidbody2D>();
        _anim          = GetComponent<Animator>();
        _boxCollider   = GetComponent<BoxCollider2D>();

        _moveAction = new InputAction(type: InputActionType.Value);
        _moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d");
        _moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/rightArrow");

        _jumpAction = new InputAction(type: InputActionType.Button);
        _jumpAction.AddBinding("<Keyboard>/space");
        _jumpAction.AddBinding("<Keyboard>/upArrow");
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _jumpAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _jumpAction.Disable();
    }

    private void Update()
    {
        _horizontalInput = _moveAction.ReadValue<float>();

        bool grounded     = IsGrounded();
        bool touchingWall = OnWall();

        // Landing detection — clear jump flag only when fully settled
        if (grounded && _body.linearVelocity.y <= 0)
        {
            _isJumping = false;
        }

        // Coyote + jump counter — only reset when grounded and not in a jump
        if (grounded && !_isJumping)
        {
            coyoteCounter = coyoteTime;
            jumpCounter   = extraJumps;
            _played       = false;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        // Flip
        if (_horizontalInput > 0.01f)
        {
            _facingSign = 1f;
            transform.localScale = new Vector3(Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);
        }
        else if (_horizontalInput < -0.01f)
        {
            _facingSign = -1f;
            transform.localScale = new Vector3(-Mathf.Abs(_originalScale.x), _originalScale.y, _originalScale.z);
        }

        // Animations
        _anim.SetBool(RunHash,      Mathf.Abs(_horizontalInput) > 0.01f);
        _anim.SetBool(GroundedHash, grounded);
        _anim.SetBool("Onwall",     touchingWall && !grounded);

        if (touchingWall && !grounded)
            _anim.SetTrigger("Wall");

        if (_wallJumpCooldown > wallJumpLockTime)
        {
            // Movement
            if (touchingWall && !grounded)
                _body.linearVelocity = new Vector2(0, _body.linearVelocity.y);
            else
                _body.linearVelocity = new Vector2(_horizontalInput * speed, _body.linearVelocity.y);

            // Gravity + wall slide
            if (touchingWall && !grounded)
            {
                _body.gravityScale = 1f;
                if (_body.linearVelocity.y < -wallSlideSpeed)
                    _body.linearVelocity = new Vector2(_body.linearVelocity.x, -wallSlideSpeed);
            }
            else
            {
                _body.gravityScale = airGravityScale;
            }

            // Jump input
            if (_jumpAction.WasPressedThisFrame())
                Jump(grounded, touchingWall);

            // Variable jump height on release
            if (_jumpAction.WasReleasedThisFrame() && _body.linearVelocity.y > 0)
                _body.linearVelocity = new Vector2(_body.linearVelocity.x, _body.linearVelocity.y / 2);
        }
        else
        {
            _wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump(bool grounded, bool touchingWall)
    {
        if (touchingWall && !grounded)
        {
            WallJump();
            return;
        }

        if (coyoteCounter <= 0 && jumpCounter <= 0) return;

        _isJumping    = true;
        coyoteCounter = 0;

        _anim.SetTrigger(JumpHash);

        if (!_played)
        {
            SoundManager.instance.PlaySound(jumpSound);
            _played = true;
        }

        if (jumpCounter > 0 && !grounded)
        {
            // Extra jump in air
            _body.linearVelocity = new Vector2(_body.linearVelocity.x, jumpPower);
            jumpCounter--;
        }
        else
        {
            // Normal or coyote jump
            _body.linearVelocity = new Vector2(_body.linearVelocity.x, jumpPower);
        }
    }

    private void WallJump()
    {
        _anim.SetTrigger(JumpHash);
        _body.AddForce(new Vector2(-_facingSign * wallJumpX, wallJumpY));
        _wallJumpCooldown = 0;
        _isJumping        = true;
        _played           = false;

        SoundManager.instance.PlaySound(jumpSound);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(
            _boxCollider.bounds.center,
            _boxCollider.bounds.size,
            0, Vector2.down, 0.1f, groundLayer).collider != null;
    }

    private bool OnWall()
    {
        return Physics2D.BoxCast(
            _boxCollider.bounds.center,
            _boxCollider.bounds.size,
            0, new Vector2(_facingSign, 0), 0.1f, wallLayer).collider != null;
    }

    public bool CanAttack()
    {
        return Mathf.Abs(_horizontalInput) < 0.01f && IsGrounded() && !OnWall();
    }
}