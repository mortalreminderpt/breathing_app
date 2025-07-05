using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("TMP Text References (Optional)")]
    [SerializeField] private TMP_Text levelText;   // 难度
    [SerializeField] private TMP_Text liveText;    // 血量

    private int cachedDifficulty = -1;
    private int cachedHealth     = -1;

    #region ★ 生命周期
    private void Awake()
    {
        // 若未在 Inspector 指定，则按约定名称在子层级中查找
        if (levelText == null)
        {
            var t = transform.Find("Level");
            if (t != null) levelText = t.GetComponent<TMP_Text>();
        }
        if (liveText == null)
        {
            var t = transform.Find("Live");
            if (t != null) liveText = t.GetComponent<TMP_Text>();
        }
    }

    private void Start() => Refresh(force: true);

    private void Update() => Refresh();
    #endregion

    #region ★ 内部逻辑
    private void Refresh(bool force = false)
    {
        var gs = GameStateHolder.Instance;
        if (gs == null) return;    // 游戏尚未就绪

        // ===== 难度 =====
        if (force || gs.SelectedDifficulty != cachedDifficulty)
        {
            cachedDifficulty = gs.SelectedDifficulty;
            if (levelText != null)
                levelText.text = $"Level: {cachedDifficulty}";
        }

        // ===== 血量 =====
        int health = gs.GetHealth();
        if (force || health != cachedHealth)
        {
            cachedHealth = health;
            if (liveText != null)
                liveText.text = $"Live: {cachedHealth}";
        }
    }
    #endregion
}