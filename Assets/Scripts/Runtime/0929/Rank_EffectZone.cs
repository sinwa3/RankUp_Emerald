using UnityEngine;

public class Rank_EffectZone : MonoBehaviour
{
    public enum EZoneType
    {
        Damage,
        Heal
    }

    #region 인스펙터
    [Header("효과")]
    [SerializeField] private EZoneType _zoneType = EZoneType.Damage;
    [SerializeField] private int _amount = 1;
    [SerializeField] private float _tickInterval = 1.0f;
    [SerializeField] private string _playerTag = "Player";

    [Header("표시")]
    [SerializeField] private Renderer _zoneRenderer;
    [SerializeField] private Color _idleColor = new Color(1.0f, 1.0f, 1.0f, 0.3f);
    [SerializeField] private Color _activeColor = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    #endregion

    #region 내부 변수
    private Rank_Health _target;
    private float _nextTickTime = 0.0f;
    #endregion

    private void Awake()
    {
        if (_zoneRenderer == null)
        {
            _zoneRenderer = GetComponent<Renderer>();
        }

        if (_zoneRenderer == null)
        {
            Debug.LogWarning($"렌더러 없음 (Rank_EffectZone)");
        }

        SetColor(_idleColor);
    }

    private void Update()
    {
        if (_target == null)
        {
            return;
        }

        if (_target.IsDead)
        {
            _target = null;

            return;
        }

        if (Time.time < _nextTickTime)
        {
            return;
        }

        _nextTickTime = Time.time + _tickInterval;

        ApplyEffect();
    }

    private void ApplyEffect()
    {
        bool applied = false;

        switch (_zoneType)
        {
            case EZoneType.Damage:
                applied = _target.TakeDamage(_amount);
                break;

            case EZoneType.Heal:
                applied = _target.Heal(_amount);
                break;
        }

        if (!applied)
        {
            return;
        }

        Debug.Log($"[{name}] {_zoneType} {_amount}만큼 체력 변경 (HP {_target.Hp} / {_target.MaxHp})");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(_playerTag) && other.CompareTag(_playerTag))
        {
            Rank_Health health = other.GetComponent<Rank_Health>();

            if (health == null)
            {
                return;
            }

            _target = health;
            _nextTickTime = Mathf.Max(_nextTickTime, Time.time);

            Debug.Log($"[{name}] 진입 → {_zoneType} 효과 시작");
            SetColor(_activeColor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_target != null && _target == other.GetComponent<Rank_Health>())
        {
            _target = null;

            Debug.Log($"[{name}] 이탈 → {_zoneType} 효과 중단");
            SetColor(_idleColor);
        }
    }

    private void SetColor(Color color)
    {
        if (_zoneRenderer == null)
        {
            return;
        }

        _zoneRenderer.material.color = color;
    }
}