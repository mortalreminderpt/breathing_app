using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;

public class BreathingGameController : MonoBehaviour
{
    public int currentStage = 0;
    public int breathCount = 0;
    public int preStateCount = 10;
    public int state1Count = 10;
    public int state2Count = 15;
    public int state3Count = 4;

    public GameObject[] state2List;
    public GameObject[] state3List;


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

    private float breathTimer = 0f;
    public float breathDuration = 5f; // 吸气4秒+保持4秒+呼气4秒+保持4秒

    public Animator MushroomAnimator;

    public Animator DropAnimator;

    public Animator State4DropAnimator;
    
    //public UDPReceiver udpReceiver;
    private float previousIntensity = 0f;

    private bool isStart = false;

    public TextMeshProUGUI breathTxt;
    public TextMeshProUGUI curStateTxt;
    public TextMeshProUGUI breathCountTxt;

    public Slider ProgressSlider;

    void Update()
    {
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
        curStateTxt.text ="stage:" + currentStage ;
        breathCountTxt.text = "breath count:"+breathCount;
        breathTxt.text = "breath timer:"+breathTimer;
        
        ProgressSlider.maxValue = breathDuration;
        ProgressSlider.value = breathTimer;
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
        currentStage = 1;
        guideVideoS1.SetActive(false);
    }

    public void ResetGame()
    {
        isStart = false;
    }
    void HandleBreathingTraining()
    {
        breathTimer += Time.deltaTime;
        if (breathTimer >= breathDuration)
        {
            breathTimer = 0f;
            breathCount++;
            CreateWaterDrop();
            if (breathCount >= preStateCount)
            {
                currentStage = 2;
                breathCount = 0;
                breathTimer = 0;
            }
        }
    }

    void HandleWaterDropFormation()
    {
        if(GuideAudioController.activeSelf == false) GuideAudioController.SetActive(true);
        breathTimer += Time.deltaTime;
        if (breathTimer >= breathDuration)
        {
            breathTimer = 0f;
            breathCount++;
            CreateWaterDrop();
            if (breathCount >= state1Count)
            {
                currentStage = 3;
                breathCount = 0;
                breathTimer = 0;
            }
        }
    }

    void HandleMushroomGrowth()
    {
        breathTimer += Time.deltaTime;
        if (breathTimer >= breathDuration)
        {
            breathTimer = 0f;
            breathCount++;
            if (breathCount >= state2Count)
            {
                ActiveGameObjectList(state2List, false);
                ActiveGameObjectList(state3List, true);
                MushroomAnimator.Play("Grow");
                currentStage = 4;
                breathCount = 0;
                breathTimer = 0;
                GuideAudioController.SetActive(false);
            }
            else
            {
                CreateWaterDrop();
            }
        }
    }


    void HandleMushroomShake()
    {
        breathTimer += Time.deltaTime;
        if (breathTimer >= breathDuration)
        {
            breathTimer = 0f;
            breathCount++;
            // ShakeMushroom();
            State4WaterDrop();
            if (breathCount >= state3Count)
            {
                Debug.Log("Game Finished!");
            }
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
        DropAnimator.SetTrigger("Drop");
        mushroomAS.clip = waterDropSound;
        mushroomAS.Play();
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
    
}
