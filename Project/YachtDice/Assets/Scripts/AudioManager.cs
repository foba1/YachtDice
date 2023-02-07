using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioClip button;
    public AudioClip record;
    public AudioClip dice;
    public AudioClip win;
    public AudioClip lose;
    public AudioSource audiosource;
    public AudioSource audiosource_bgm;
    public GameObject bgm_volume;
    public GameObject effect_volume;

    public void playsound(int num)
    {
        if (num == 1)
        {
            audiosource.clip = button;
        }
        else if (num == 2)
        {
            audiosource.clip = record;
        }
        else if (num == 3)
        {
            audiosource.clip = dice;
        }
        else if (num == 4)
        {
            audiosource.clip = win;
        }
        else
        {
            audiosource.clip = lose;
        }
        audiosource.Play();
    }

    public void setvolume(int num)
    {
        if (num == 0)
        {
            audiosource_bgm.volume = bgm_volume.GetComponent<Slider>().value;
            PlayerPrefs.SetFloat("bgm", bgm_volume.GetComponent<Slider>().value);
        }
        else
        {
            audiosource.volume = effect_volume.GetComponent<Slider>().value;
            PlayerPrefs.SetFloat("effect", effect_volume.GetComponent<Slider>().value);
        }
        PlayerPrefs.Save();
    }

    void Awake()
    {
        if (!PlayerPrefs.HasKey("bgm"))
        {
            PlayerPrefs.SetFloat("bgm", 1f);
            PlayerPrefs.SetFloat("effect", 1f);
            PlayerPrefs.SetInt("admob", 1);
            PlayerPrefs.Save();
        }
        else
        {
            if (bgm_volume != null)
            {
                bgm_volume.GetComponent<Slider>().value = PlayerPrefs.GetFloat("bgm");
            }
            audiosource_bgm.volume = PlayerPrefs.GetFloat("bgm");
            if (effect_volume != null)
            {
                effect_volume.GetComponent<Slider>().value = PlayerPrefs.GetFloat("effect");
            }
            audiosource.volume = PlayerPrefs.GetFloat("effect");
        }
    }
}
