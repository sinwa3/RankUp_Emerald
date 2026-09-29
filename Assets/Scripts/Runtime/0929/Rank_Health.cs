using System;
using UnityEngine;

public class Rank_Health : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private int _maxHp = 10;
    [SerializeField] private float _invincibleTime = 0.5f;
    #endregion

    #region 내부 변수
    private int _hp = 0;
    private float _invincibleEndTime = 0.0f;

    public int Hp => _hp;
    public int MaxHp => _maxHp;
    public bool IsDead => _hp <= 0;

    // 현재 HP, 최대 HP, 변화량
    public event Action<int, int, int> OnHpChanged;   
    public event Action OnDied;
    #endregion

    private void Awake()
    {
        SetHp(_maxHp);
    }

    private void Start()
    {
        OnHpChanged?.Invoke(_hp, _maxHp, 0);
    }

    public bool TakeDamage(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("데미지가 0 이하");

            return false;
        }

        if (IsDead)
        {
            return false;
        }

        if (Time.time < _invincibleEndTime)
        {
            return false;
        }

        _invincibleEndTime = Time.time + _invincibleTime;
        SetHp(_hp - amount);

        if (IsDead)
        {
            OnDied?.Invoke();
        }

        return true;
    }

    public bool Heal(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("회복량이 0 이하");

            return false;
        }

        if (IsDead)
        {
            return false;
        }

        if (_hp >= _maxHp)
        {
            return false;
        }

        SetHp(_hp + amount);

        return true;
    }

    private void SetHp(int next)
    {
        next = Mathf.Clamp(next, 0, _maxHp);

        if (_hp == next)
        {
            return;
        }

        int delta = next - _hp;
        _hp = next;

        OnHpChanged?.Invoke(_hp, _maxHp, delta);
    }
}
