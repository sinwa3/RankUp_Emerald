using UnityEngine;

public class Rank_Enemy : MonoBehaviour
{
    #region 인스펙터
    [Header("접촉 데미지")]
    [SerializeField] private int _contactDamage = 1;
    [SerializeField] private float _damageInterval = 1.0f;
    [SerializeField] private string _playerTag = "Player";

    [Header("사망")]
    [SerializeField] private float _destroyDelay = 0.3f;
    #endregion

    #region 내부 변수
    private Rank_Health _health;
    private Collider _col;
    private Rank_Health _target;
    private float _nextDamageTime = 0.0f;
    #endregion

    private void Awake()
    {
        _health = GetComponent<Rank_Health>();
        _col = GetComponent<Collider>();

        if (_col == null)
        {
            Debug.LogWarning($"콜라이더 없음 / (Rank_Enemy : {name})");
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
        if (_target == null)
        {
            return;
        }

        if (_target.IsDead)
        {
            _target = null;

            return;
        }

        if (Time.time < _nextDamageTime)
        {
            return;
        }

        _nextDamageTime = Time.time + _damageInterval;

        if (!_target.TakeDamage(_contactDamage))
        {
            return;
        }

        Debug.Log($"{name} → {_target.name} 데미지 {_contactDamage} (HP {_target.Hp} / {_target.MaxHp})");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(_playerTag) && other.CompareTag(_playerTag))
        {
            if (_target == null)
            {
                _target = other.GetComponent<Rank_Health>();
            }

            _nextDamageTime = Mathf.Max(_nextDamageTime, Time.time);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_target != null && _target == other.GetComponent<Rank_Health>())
        {
            _target = null;
        }
    }

    private void HandleDied()
    {
        Debug.Log($"{name} 처치");

        _target = null;

        if (_col != null)
        {
            _col.enabled = false;
        }

        enabled = false;

        Destroy(gameObject, _destroyDelay);
    }
}
