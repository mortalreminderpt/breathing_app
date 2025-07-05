using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("TMP Text References (Optional)")]
    [SerializeField] private TMP_Text levelText;   // 难度
    [SerializeField] private TMP_Text liveText;    // 血量
    #region ★ 新增：计时文本
    [SerializeField] private TMP_Text timeText;    // 距上次改难度已过时间
    #endregion

    private int cachedDifficulty = -1;
    private int cachedHealth     = -1;
    #region
    private int cachedSeconds    = -1;
    #endregion

    #region
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
        #region ★ 新增：自动查找 Time
        if (timeText == null)
        {
            var t = transform.Find("Time");
            if (t != null) timeText = t.GetComponent<TMP_Text>();
        }
        #endregion
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
            {
                switch (cachedDifficulty)
                {
                    case 3: levelText.text = "Expert";        break;
                    case 4: levelText.text = "Intermediate";  break;
                    case 5: levelText.text = "Beginner";      break;
                }
            }
        }

        // ===== 血量 =====
        int health = gs.GetHealth();
        if (force || health != cachedHealth)
        {
            cachedHealth = health;
            if (liveText != null)
                liveText.text = $"{cachedHealth}";
        }

        #region
        GameStateHolder.Instance.PauseDifficultyTimer();
        int seconds = Mathf.FloorToInt(gs.TimeSinceDifficultyChange);
        if (force || seconds != cachedSeconds)
        {
            cachedSeconds = seconds;

            if (timeText != null)
            {
                int mm = seconds / 60;
                int ss = seconds % 60;
                timeText.text = $"{mm:00}:{ss:00}";
            }
        }
        #endregion
    }
    #endregion
}