using UnityEngine;

public class ReloadCounter : MonoBehaviour
{
    private static ReloadCounter instance;
    public int reloadCount = 0;
    public int maxReloadCount = 2;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // 将当前对象设置为单例
        instance = this;
        // SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Reload()
    {
        reloadCount += 1;
    }

    public bool CanReload()
    {
        if (reloadCount >= maxReloadCount)
        {
            return false;
        }
        return true;
    }
}