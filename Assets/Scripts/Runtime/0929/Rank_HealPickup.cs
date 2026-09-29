using UnityEngine;

public class Rank_HealPickup : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private int _healAmount = 3;
    [SerializeField] private string _playerTag = "Player";
    #endregion

    #region 내부 변수
    private bool _isUsed = false;
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(_playerTag) && other.CompareTag(_playerTag))
        {
            TryUse(other);
        }
    }

    private void TryUse(Collider other)
    {
        if (_isUsed)
        {
            return;
        }

        Rank_Health health = other.GetComponent<Rank_Health>();

        if (health == null)
        {
            Debug.LogWarning("체력 컴포넌트 없음 (Rank_HealPickup)");

            return;
        }

        if (!health.Heal(_healAmount))
        {
            Debug.Log("회복 불가 (HP 가득 또는 사망 / Rank_HealPickup)");

            return;
        }

        _isUsed = true;

        Debug.Log($"{health.name} 회복 {_healAmount} (HP {health.Hp} / {health.MaxHp})");

        Destroy(this.gameObject);
    }
}