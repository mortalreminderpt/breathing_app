using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

public class BreathingPhase03Controller : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourceB; // 辅助音频B
    [SerializeField] private AudioSource audioSourceD; // 音频D
    [SerializeField] private GameObject passthroughObject; // Passthrough对象
    [SerializeField] private GameObject virtualNatureObject; // Virtual Nature对象

    private float phase03Timer = 0f; // 阶段03计时器
    private float breathTimer = 0f; // 呼吸计时器
    private bool isInhaling = false; // 是否在吸气
    private int unstableCount = 0; // 不稳定次数
    private List<float> breathingData = new List<float>(); // 呼吸数据
    private bool isPhase03Active = true; // 阶段03是否激活

    // 用于模拟呼吸的按键
    private bool isRightTriggerPressed = false;

    void Start()
    {
        InitializePhase03();
    }

    void InitializePhase03()
    {
        phase03Timer = 0f;
        breathTimer = 0f;
        unstableCount = 0;
        breathingData.Clear();
        isPhase03Active = true;
    }

    void Update()
    {
        if (!isPhase03Active) return;

        // 更新阶段03计时器
        phase03Timer += Time.deltaTime;

        // 模拟呼吸输入（使用右手柄扳机键）
        CheckBreathingInput();

        // 检查阶段03是否完成（2分钟）
        if (phase03Timer >= 120f)
        {
            Debug.Log("阶段03完成");
            return;
        }
    }

    void CheckBreathingInput()
    {
        // 模拟使用右手柄扳机键进行呼吸输入
        bool currentTriggerState = Input.GetKey(KeyCode.Space); // 在Unity编辑器中使用空格键测试
        // 在实际Quest 3中使用：这个写法是XR Interaction Toolkit饿的写法，如果是OVRinput的话需要替换一下
        // var rightHandDevices = new List<InputDevice>();
        // InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, rightHandDevices);
        // if (rightHandDevices.Count > 0)
        // {
        //     rightHandDevices[0].TryGetFeatureValue(CommonUsages.triggerButton, out currentTriggerState);
        // }

        // 检测状态变化
        if (currentTriggerState != isRightTriggerPressed)
        {
            isRightTriggerPressed = currentTriggerState;

            if (currentTriggerState)
            {
                // 开始吸气
                CheckBreathDuration();
                isInhaling = true;
                breathTimer = 0f;
            }
            else
            {
                // 开始呼气
                CheckBreathDuration();
                isInhaling = false;
                breathTimer = 0f;
            }
        }

        breathTimer += Time.deltaTime;
    }

    void CheckBreathDuration()
    {
        if (breathTimer < 4f || breathTimer > 6f)
        {
            HandleUnstableBreathing();
        }

        // 记录呼吸数据
        breathingData.Add(breathTimer);
    }

    void HandleUnstableBreathing()
    {
        unstableCount++;

        switch (unstableCount)
        {
            case 1:
                PlayAudioB("第一次不稳定");
                break;

            case 2:
                PlayAudioB("第二次不稳定");
                break;

            case 3:
                StartCoroutine(TransitionToPhase02());
                break;
        }
    }

    void PlayAudioB(string message)
    {
        Debug.Log(message);
        if (audioSourceB != null)
        {
            audioSourceB.Play();
        }
    }

    IEnumerator TransitionToPhase02()
    {
        Debug.Log("第三次不稳定，开始转换到阶段02");
        isPhase03Active = false;

        // 切换到Passthrough
        if (passthroughObject != null)
            passthroughObject.SetActive(true);
        if (virtualNatureObject != null)
            virtualNatureObject.SetActive(false);

        // 等待5秒
        yield return new WaitForSeconds(5f);

        // 切换到Virtual Nature
        if (passthroughObject != null)
            passthroughObject.SetActive(false);
        if (virtualNatureObject != null)
            virtualNatureObject.SetActive(true);

        // 播放音频D
        if (audioSourceD != null)
        {
            audioSourceD.Play();
        }

        // 通知游戏管理器返回阶段02
        GameManager.Instance.ReturnToPhase02();
    }
}

// 游戏管理器示例
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReturnToPhase02()
    {
        // 实现返回阶段02的逻辑
        Debug.Log("返回阶段02，开始100秒稳定训练");
        // 这里添加具体实现
    }
}
