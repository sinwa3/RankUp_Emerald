using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Rank_PlayerMove : MonoBehaviour
{
    #region 인스펙터
    [Header("이동 / 회전")]
    [SerializeField] private float _moveSpeed = 6.0f;
    [SerializeField] private float _rotateSharpness = 15.0f;

    [Header("중력")]
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _groundStick = -2.0f;

    [SerializeField] private Transform _camTr;
    #endregion

    #region 내부 변수
    private CharacterController _controller;
    private Rank_Health _health;

    private Vector2 _moveInput;
    private float _verticalVelocity;
    private bool _isDead;
    #endregion

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        if (_controller == null)
        {
            Debug.LogWarning("컨트롤러 없음 (Rank_PlayerMove)");
            enabled = false;

            return;
        }

        _health = GetComponent<Rank_Health>();

        if (_health == null)
        {
            Debug.LogWarning("체력 컴포넌트 없음 (Rank_PlayerMove)");
        }

        if (_camTr == null && Camera.main != null)
        {
            _camTr = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDied += HandleDied;
        }
    }

    private void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDied -= HandleDied;
        }
    }

    private void Update()
    {
        ReadInput();
        TickGravity();

        Vector3 moveDir = BuildMoveDirection(_moveInput);

        Vector3 velocity = moveDir * _moveSpeed;
        velocity.y = _verticalVelocity;
        _controller.Move(velocity * Time.deltaTime);

        TickRotate(moveDir);
    }

    private void ReadInput()
    {
        if (_isDead)
        {
            _moveInput = Vector2.zero;

            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(h, v);

        _moveInput = Vector2.ClampMagnitude(dir, 1.0f);
    }

    private void TickGravity()
    {
        if (_controller.isGrounded)
        {
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = _groundStick;
            }
        }

        _verticalVelocity += _gravity * Time.deltaTime;
    }

    private Vector3 BuildMoveDirection(Vector2 input)
    {
        if (input.sqrMagnitude < 0.0001f)
        {
            return Vector3.zero;
        }

        if (_camTr == null)
        {
            Vector3 dir = new Vector3(input.x, 0.0f, input.y);

            return dir.normalized;
        }

        Vector3 camF = Vector3.ProjectOnPlane(_camTr.forward, Vector3.up).normalized;
        Vector3 camR = Vector3.ProjectOnPlane(_camTr.right, Vector3.up).normalized;

        Vector3 result = camF * input.y + camR * input.x;

        return result.normalized;
    }

    private void TickRotate(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);

        transform.rotation = Quaternion.Slerp
            (
            transform.rotation,
            target,
            1.0f - Mathf.Exp(-_rotateSharpness * Time.deltaTime)
            );
    }

    private void HandleDied()
    {
        _isDead = true;
    }
}