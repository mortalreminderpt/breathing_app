using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class BreathingGameController : MonoBehaviour
{
    public int currentStage = 0;
    public int preStateCount = 10;
    public int state1Count = 10;
    public int state2Count = 15;
    public int state3Count = 4;

    public GameObject[] state2List;
    public GameObject[] state3List;

    public BreathingDetector breathingDetector;


    public AudioClip waterDropSound;
    public AudioClip mushroomGrowSound;
    public AudioSource mushroomAS;

    public GameObject guideVideoS1;
    public VideoPlayer guideVideoPlayer1;

    public AudioClip guideAudioS2;
    public AudioClip guideAudioS3;
    public AudioClip guideAudioS4;
    private bool isWatchingVideo;
    public GameObject GuideAudioController;
    
    public Animator MushroomAnimator;

    public Animator DropAnimator;

    public Animator State4DropAnimator;

    //public UDPReceiver udpReceiver;
    private float previousIntensity = 0f;

    public bool isStart = false;

    public TextMeshProUGUI breathTxt;
    public TextMeshProUGUI curStateTxt;
    public TextMeshProUGUI breathCountTxt;

    public Slider ProgressSlider;
    
    public UnityEvent onGuideFinished;

    private int numUnstable = 0;
    private int numBreathing = 0;

    void Update()
    {
        if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger) 
            || OVRInput.GetUp(OVRInput.RawButton.LHandTrigger)
            || Input.GetKeyUp(KeyCode.Return)
            || Input.GetMouseButtonUp(0))
        {
            OnVideoFinished(null);
        }
        if (isStart && currentStage == 0)
        {
            HandleGuideVideo();
        }
        return;
        if (!isStart) return;
        switch (currentStage)
        {
            case 0:
                HandleGuideVideo();
                break;
            case 1:
                HandleBreathingTraining();
                break;
            case 2:
                HandleWaterDropFormation();
                break;
            case 3:
                HandleMushroomGrowth();
                break;
            case 4:
                HandleMushroomShake();
                break;
        }

        if (currentStage != 0)
        {
            if (breathingDetector.GetLastBreathStability() == false && breathingDetector.GetBreathingCount() != numBreathing)
            {
                numUnstable += 1;
                Debug.Log("第" + numUnstable + "次不稳定");
                if (numUnstable < 3)
                {
                    // if (GuideAudioController.activeSelf == false) 
                    GuideAudioController.SetActive(false);
                    GuideAudioController.SetActive(true);
                }
                else
                {
                    currentStage = 1;
                }
            }
        }
        
        numBreathing = breathingDetector.GetBreathingCount();
        curStateTxt.text = "stage:" + currentStage;
        breathCountTxt.text = "breath count:" + breathingDetector.GetBreathingCount();
        breathTxt.text = "breath timer:" + breathingDetector.GetAverageBreathRate();
        ProgressSlider.maxValue = 10;
        ProgressSlider.value = breathingDetector.GetAverageBreathRate();
    }

    public void StartGame()
    {
        isStart = true;
    }

    public void HandleGuideVideo()
    {
        if (!isWatchingVideo)
        {
            guideVideoS1.SetActive(true);
            isWatchingVideo = true;
            // 添加视频播放结束的回调
            guideVideoPlayer1.loopPointReached += OnVideoFinished;

            // 开始播放视频
            guideVideoPlayer1.Play();
        }
    }


    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("视频播放完毕！");
        // 在这里执行你想要的行为
        isWatchingVideo = false;
        // LoadScene("Stage2Scene");
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
        currentStage = 1;
        guideVideoS1.SetActive(false);
    }

    public void ResetGame()
    {
        isStart = false;
    }

    void HandleBreathingTraining()
    {
        if (GuideAudioController.activeSelf == false) GuideAudioController.SetActive(true);


        // breathCount++;
        CreateWaterDrop();
        if (breathingDetector.GetBreathingCount() >= preStateCount)
        {
            currentStage = 2;
            breathingDetector.ResetBreathingCount();
        }
    }

    void HandleWaterDropFormation()
    {
        // if(GuideAudioController.activeSelf == false) GuideAudioController.SetActive(true);
        // breathCount++;
        CreateWaterDrop();
        if (breathingDetector.GetBreathingCount() >= state1Count)
        {
            currentStage = 3;
            breathingDetector.ResetBreathingCount();
        }
    }

    void HandleMushroomGrowth()
    {
        // breathCount++;
        if (breathingDetector.GetBreathingCount() >= state2Count)
        {
            ActiveGameObjectList(state2List, false);
            ActiveGameObjectList(state3List, true);
            MushroomAnimator.Play("Grow");
            currentStage = 4;
            breathingDetector.ResetBreathingCount();

            GuideAudioController.SetActive(false);
        }
        else
        {
            CreateWaterDrop();
        }
    }


    void HandleMushroomShake()
    {
        // breathCount++;
        // ShakeMushroom();
        // State4WaterDrop();
        if (breathingDetector.GetBreathingCount() >= state3Count)
        {
            Debug.Log("Game Finished!");
        }
    }

    void State4WaterDrop()
    {
        State4DropAnimator.SetTrigger("DropState4");
        MushroomAnimator.Play("DropNew");
        mushroomAS.clip = waterDropSound;
        mushroomAS.Play();
    }

    void CreateWaterDrop()
    {
        // DropAnimator.SetTrigger("Drop");
        // mushroomAS.clip = waterDropSound;
        // mushroomAS.Play();
    }

    void GrowMushroom(float progress)
    {
        Debug.Log("progress:" + progress);
        MushroomAnimator.Play("Base Layer.Grow", 0, progress); // 归一化时间
        // mushroomAS.clip = mushroomGrowSound;
        // mushroomAS.Play();
    }


    void ActiveGameObjectList(GameObject[] list, bool active)
    {
        for (int i = 0; i < list.Length; i++)
        {
            list[i].SetActive(active);
        }
    }

    void ShakeMushroom()
    {
        MushroomAnimator.Play("DropNew");
        // mushroom.GetComponent<Animator>().SetTrigger("Shake");
        // mushroomShakeSound.Play();
    }
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}