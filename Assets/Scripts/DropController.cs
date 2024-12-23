using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    public Animator DropAnimator;
    public AudioSource mushroomAS;
    public AudioClip waterDropSound;
    public GameObject Mushroom;
    public GameObject Skybox;
    public float DropInterval = 5f;
    public float TimeScale = 1f;
    public float _dropTimer = 0f;
    public int SpwanAmount = 4;
    public int GrowAmount = 8;
    public int SkyboxAmount = 14;
    public int _dropCount = 0;

    public void Reset()
    {
        _dropCount = 0;
        _dropTimer = 0;
        Skybox.GetComponent<SkyboxSwitcher>().Reset();
        Mushroom.GetComponent<MushroomController>().Reset();
    }

    void Update()
    {
        _dropTimer -= TimeScale * Time.deltaTime;
        if (_dropTimer <= 0f)
        {
            // 触发 Drop 动画
            DropAnimator.SetTrigger("Drop");
            // 播放水滴声音
            mushroomAS.clip = waterDropSound;
            mushroomAS.Play();
            _dropCount++;
            // 重置计时器
            _dropTimer = DropInterval;
        }
        
        if (_dropCount == SpwanAmount)
        {
            Mushroom.GetComponent<MushroomController>().ToSmall();
        }
        
        if (_dropCount == GrowAmount)
        {
            Mushroom.GetComponent<MushroomController>().ToBig();
        }

        if (_dropCount == SkyboxAmount)
        {
            Skybox.GetComponent<SkyboxSwitcher>().ToDay();
        }
    }
}
