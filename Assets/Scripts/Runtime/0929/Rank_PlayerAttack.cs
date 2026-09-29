using UnityEngine;

public class Rank_PlayerAttack : MonoBehaviour
{
    #region 인스펙터
    [Header("공격")]
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private float _attackRange = 1.0f;
    [SerializeField] private float _attackRadius = 0.8f;
    [SerializeField] private LayerMask _enemyLayer = 1 << 7;

    [Header("공격 표시")]
    [SerializeField] private GameObject _attackVisual;
    [SerializeField] private float _visualTime = 0.1f;
    #endregion

    #region 내부 변수
    private Rank_Health _health;

    private float _nextAttackTime = 0.0f;
    private float _visualEndTime = 0.0f;
    #endregion

    private void Awake()
    {
        _health = GetComponent<Rank_Health>();

        if (_health == null)
        {
            Debug.LogWarning("체력 컴포넌트 없음 (Rank_PlayerAttack)");
        }

        if (_attackVisual != null)
        {
            _attackVisual.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

        if (_attackVisual != null && _attackVisual.activeSelf && Time.time >= _visualEndTime)
        {
            _attackVisual.SetActive(false);
        }
    }

    private void TryAttack()
    {
        if (_health != null && _health.IsDead)
        {
            return;
        }

        if (Time.time < _nextAttackTime)
        {
            return;
        }

        Debug.Log("공격 시도");

        _nextAttackTime = Time.time + _attackCooldown;


        if (_attackVisual != null)
        {
            _attackVisual.SetActive(true);
            _visualEndTime = Time.time + _visualTime;
        }

        DealDamage();
    }

    private void DealDamage()
    {
        Vector3 attackPos = GetAttackCenter();
        Collider[] enemies = Physics.OverlapSphere(attackPos, _attackRadius, _enemyLayer);

        for (int i = 0; i < enemies.Length; i++)
        {
            Rank_Health enemyHealth = enemies[i].GetComponent<Rank_Health>();

            if (enemyHealth == null)
            {
                continue;
            }

            if (!enemyHealth.TakeDamage(_damage))
            {
                continue;
            }

            Debug.Log($"{enemies[i].name}의 체력 {enemyHealth.Hp} / {enemyHealth.MaxHp}");
        }
    }

    private Vector3 GetAttackCenter()
    {
        return transform.position + transform.forward * _attackRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 attackPos = GetAttackCenter();
        Gizmos.DrawWireSphere(attackPos, _attackRadius);
    }
}