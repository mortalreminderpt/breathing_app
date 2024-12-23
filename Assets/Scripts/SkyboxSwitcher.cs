using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    public Material daySkybox;
    public Material nightSkybox;

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        ToNight();
    }
    
    public void ToDay()
    {
        RenderSettings.skybox = daySkybox;
        DynamicGI.UpdateEnvironment();
    }

    public void ToNight()
    {
        RenderSettings.skybox = nightSkybox;
        DynamicGI.UpdateEnvironment();
    }
}
