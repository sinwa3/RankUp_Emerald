using UnityEngine;

[RequireComponent(typeof(Rank_Health))]
public class Rank_HitFlash : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _hitColor = Color.red;
    [SerializeField] private float _flashTime = 0.15f;
    #endregion

    #region 내부 변수
    private Rank_Health _health;
    private Color _originColor;
    private float _flashEndTime = 0.0f;
    private bool _isFlashing = false;
    #endregion

    private void Awake()
    {
        _health = GetComponent<Rank_Health>();

        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }

        if (_renderer == null)
        {
            Debug.LogWarning($"렌더러 없음 (Rank_HitFlash)");
            enabled = false;

            return;
        }

        _originColor = _renderer.material.color;
    }

    private void OnEnable()
    {
        _health.OnHpChanged += HandleHpChanged;
    }

    private void OnDisable()
    {
        _health.OnHpChanged -= HandleHpChanged;
    }

    private void Update()
    {
        if (!_isFlashing)
        {
            return;
        }

        if (Time.time < _flashEndTime)
        {
            return;
        }

        _renderer.material.color = _originColor;
        _isFlashing = false;
    }

    private void HandleHpChanged(int hp, int maxHp, int delta)
    {
        if (delta >= 0)
        {
            return;
        }

        _renderer.material.color = _hitColor;
        _flashEndTime = Time.time + _flashTime;
        _isFlashing = true;
    }
}