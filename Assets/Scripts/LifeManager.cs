using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 场景常驻的生命系统（可挂在空 GameObject 上）
/// </summary>
public class LifeManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int maxLives = 5;
    [SerializeField] private Sprite fullLifeSprite;   // 满血图
    [SerializeField] private Sprite emptyLifeSprite;  // 空血图
    [SerializeField] private Transform livesPanel;    // UI 父节点
    [SerializeField] private GameObject iconPrefab;   // 蘑菇 prefab

    /// <summary>游戏结束事件，Inspector 里可绑 UI、切场景、播动画等</summary>
    public UnityEvent OnGameOver;
    /// <summary>生命值变化事件，若外面要同步别的逻辑</summary>
    public UnityEvent<int> OnLivesChanged;  // 传当前血量

    public static LifeManager Instance { get; private set; }

    private List<Image> lifeIcons = new();
    private int currentLives;

    #region Life API —— 供外部调用
    public void LoseLife(int amount = 1)
    {
        SetLives(currentLives - amount);
    }
    public void AddLife(int amount = 1)
    {
        SetLives(currentLives + amount);
    }
    public int GetLives() => currentLives;
    #endregion

    #region Mono
    private void Awake()
    {
        // 保证场景内唯一
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        print(GameStateHolder.Instance.SelectedDifficulty);
        maxLives = GameStateHolder.Instance.SelectedDifficulty;
        InitIcons();
        SetLives(maxLives, false); // 初始满血
    }
    #endregion

    #region Core
    private void InitIcons()
    {
        for (int i = 0; i < maxLives; i++)
        {
            var icon = Instantiate(iconPrefab, livesPanel).GetComponent<Image>();
            lifeIcons.Add(icon);
        }
    }

    private void SetLives(int newLives, bool triggerEvent = true)
    {
        currentLives = Mathf.Clamp(newLives, 0, maxLives);

        // 刷新 UI
        for (int i = 0; i < lifeIcons.Count; i++)
        {
            lifeIcons[i].sprite = i < currentLives ? fullLifeSprite : emptyLifeSprite;
        }

        if (triggerEvent) OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            // 清一次，防止重复 GameOver
            currentLives = 0;
            OnGameOver?.Invoke();
        }
    }
    #endregion
}