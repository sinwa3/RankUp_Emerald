using TMPro;
using UnityEngine;

public class Rank_PlayerHUD : MonoBehaviour
{
    #region 인스펙터
    [SerializeField] private Rank_Health _playerHealth;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private string _playerTag = "Player";
    #endregion

    private void Awake()
    {
        if (_playerHealth == null)
        {
            GameObject player = GameObject.FindWithTag(_playerTag);

            if (player != null)
            {
                _playerHealth = player.GetComponent<Rank_Health>();
            }
        }

        if (_playerHealth == null)
        {
            Debug.LogWarning("플레이어 체력 없음 (Rank_PlayerHUD)");
        }

        if (_gameOverUI != null)
        {
            _gameOverUI.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnHpChanged += HandleHpChanged;
            _playerHealth.OnDied += HandleDied;
        }
    }

    private void OnDisable()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnHpChanged -= HandleHpChanged;
            _playerHealth.OnDied -= HandleDied;
        }
    }

    private void HandleHpChanged(int hp, int maxHp, int delta)
    {
        if (_hpText == null)
        {
            return;
        }

        _hpText.text = $"HP {hp} / {maxHp}";
    }

    private void HandleDied()
    {
        Debug.Log("게임 오버");

        if (_gameOverUI != null)
        {
            _gameOverUI.SetActive(true);
        }
    }
}
