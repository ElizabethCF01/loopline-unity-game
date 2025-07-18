using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masterSlider;

    private void Awake()
    {
        masterSlider?.onValueChanged.AddListener((x) => SetVolume("MasterVolume", x));
        sfxSlider?.onValueChanged.AddListener((x) => SetVolume("SfxVolume", x));
        musicSlider?.onValueChanged.AddListener((x) => SetVolume("MusicVolume", x));
    }

    private void SetVolume(string group, float volume)
    {
        if (Mathf.Abs(volume) > Mathf.Epsilon)
            audioMixer.SetFloat(group, Mathf.Log10(volume) * 20);
        else
            audioMixer.SetFloat(group, -80.0f);
    }
}
