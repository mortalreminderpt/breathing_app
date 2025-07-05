using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;   // ⭐ 必须引入

public class GameStateHolder : MonoBehaviour
{
    public static GameStateHolder Instance;

    [Header("Difficulty (5=Blue, 4=Red, 3=Yellow)")]
    public int SelectedDifficulty = 5;

    public int MaxHealth = -1;

    [Header("Player Health")] 
    [SerializeField] private int currentHealth;// 当前血量

    #region ★ 新增：计时器
    [Header("⏱ Time Since Difficulty Change")]
    [SerializeField] private float elapsedSinceDifficultyChange = 0f;  // 距上次修改难度已过秒数
    public  float TimeSinceDifficultyChange => elapsedSinceDifficultyChange;    // 只读访问
    #endregion
    
    #region 计时器暂停开关
    private bool timerPaused = false;
    public  bool TimerPaused => timerPaused;                 // 外部可读
    public  void PauseDifficultyTimer()  => timerPaused = true;
    public  void ResumeDifficultyTimer() => timerPaused = false;
    #endregion


    [Header("Post-Processing Profiles")]
    public VolumeProfile blueProfile;
    public VolumeProfile redProfile;
    public VolumeProfile yellowProfile;

    // 缓存
    private Volume     globalVolume;
    private GameObject blueMushroom;
    private GameObject redMushroom;
    private GameObject yellowMushroom;

    #region ★ 生命周期
    private void Awake()
    {
        // 单例
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (MaxHealth < 0)
        {
            MaxHealth = SelectedDifficulty;
        }

        currentHealth = MaxHealth;             // 启动时满血
        elapsedSinceDifficultyChange = 0f;              // 初始化计时器

        // 监听场景加载
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 记得注销
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    // ★ 新增：每帧累加计时
    private void Update()
    {
        if (MaxHealth < 0)
        {
            MaxHealth = SelectedDifficulty;
        }
        if (!timerPaused)
            elapsedSinceDifficultyChange += Time.deltaTime;
    }

    // 第一关载入后，SceneManager 会回调一次 OnSceneLoaded，所以 Start 不再必须
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CacheSceneReferences();     // 🔄 每换场景重新定位对象
        ApplyDifficulty();          // ⚙ 依据当前难度刷新
    }

    #region ★ 外部 API
    // ===== 难度 =====
    public void SetDifficulty(int level)
    {
        SelectedDifficulty = level;
        MaxHealth = SelectedDifficulty;
        currentHealth = MaxHealth;

        elapsedSinceDifficultyChange = 0f;
        timerPaused = false;          // 切换难度时自动重新开始计时

        ApplyDifficulty();          // 立即刷新当前场景
    }

    // ===== 血量 =====
    public int GetHealth() => currentHealth;

    public void SetHealth(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        // TODO：在此处触发 UI 刷新或事件
    }

    public void ChangeHealth(int delta) => SetHealth(currentHealth + delta);

    public bool IsDead => currentHealth <= 0;
    #endregion

    #region ★ 内部逻辑
    private void CacheSceneReferences()
    {
        // 1. 蘑菇（可能某些场景没有）
        blueMushroom   = GameObject.Find("BlueMushroom");
        redMushroom    = GameObject.Find("RedMushroom");
        yellowMushroom = GameObject.Find("YellowMushroom");

        // 2. Global Volume（找场景里第一个 Volume）
        globalVolume = FindObjectOfType<Volume>();
        if (globalVolume == null)
            Debug.LogWarning($"[GameStateHolder] 场景 {SceneManager.GetActiveScene().name} 中找不到 Volume。");
    }

    private void ApplyDifficulty()
    {
        if (SceneManager.GetActiveScene().name == "StartScene") return;
        // === 蘑菇显隐 ===
        blueMushroom?.SetActive(false);
        redMushroom?.SetActive(false);
        yellowMushroom?.SetActive(false);

        switch (SelectedDifficulty)
        {
            case 5: blueMushroom?.SetActive(true);  TrySetProfile(blueProfile);   break;
            case 4: redMushroom ?.SetActive(true);  TrySetProfile(redProfile);    break;
            case 3: yellowMushroom?.SetActive(true);TrySetProfile(yellowProfile); break;
            default: Debug.LogWarning($"未知难度 {SelectedDifficulty}");          break;
        }
    }

    private void TrySetProfile(VolumeProfile profile)
    {
        if (globalVolume == null) return;        // 场景里没 Volume，直接跳过
        if (profile == null)
        {
            Debug.LogWarning("VolumeProfile 未在 Inspector 上绑定！");
            return;
        }
        globalVolume.profile = profile;
    }
    #endregion
}
