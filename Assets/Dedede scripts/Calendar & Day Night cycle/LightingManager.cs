using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour, IDataPersistence
{
    //References
    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    //Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;


    public void LoadData(GameData data)
    {
        this.TimeOfDay = data.timeOfDay;
    }

    public void SaveData(ref GameData data)
    {
        data.timeOfDay = this.TimeOfDay;
    }

    private void Update()
    {
        if(preset != null)
        {
            if(Application.isPlaying)
            {
                TimeOfDay += Time.deltaTime / 116.88850f;
                TimeOfDay %= 24;
                UpdateLighting(TimeOfDay / 24f);
            }
            else
            {
                UpdateLighting(TimeOfDay / 24f);
            }
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.fogColor.Evaluate(timePercent);

        if(directionalLight != null)
        {
            directionalLight.color = preset.directionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0f));
        }    
    }

    //try to find a directional light if one hasn't been set 
    private void OnValidate()
    {
        if(directionalLight != null)
        {
            return;
        }
        //search for lighting tab sun
        if(RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        //search for a light that fits the criteria for directional light
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}
