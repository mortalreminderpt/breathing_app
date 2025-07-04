using UnityEngine;

public class GameStateHolder : MonoBehaviour
{
    public static GameStateHolder Instance;

    public int SelectedDifficulty = 5; // 默认 Beginner

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}