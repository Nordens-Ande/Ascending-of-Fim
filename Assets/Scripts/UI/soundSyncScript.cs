using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class soundSyncScript : MonoBehaviour
{
    [SerializeField] Slider main;
    [SerializeField] Slider sfx;
    [SerializeField] Slider music;

    [SerializeField] TextMeshProUGUI numberMain;
    [SerializeField] TextMeshProUGUI numberSFX;
    [SerializeField] TextMeshProUGUI numberMusic;

    [SerializeField] AudioMixer audioMixer;

    void Start()
    {
        main.SetValueWithoutNotify(75);
        sfx.SetValueWithoutNotify(75);
        music.SetValueWithoutNotify(75);

        UpdateSoundValues();

        //l�gger till lyssnare som lyssnar p� om ifall slidersen �ndras och om de g�r det uppdatera
        main.onValueChanged.AddListener(delegate { UpdateSoundValues(); });
        sfx.onValueChanged.AddListener(delegate { UpdateSoundValues(); });
        music.onValueChanged.AddListener(delegate { UpdateSoundValues(); });
    }

    void UpdateSoundValues()
    {
        //tar nummert fr�n sliders, rundar till en int ch displayar p� nummert brevid
        numberMain.text = Mathf.RoundToInt(main.value).ToString();
        numberSFX.text = Mathf.RoundToInt(sfx.value).ToString();
        numberMusic.text = Mathf.RoundToInt(music.value).ToString();

        setMixerValues("Master", main.value);
        setMixerValues("Background Music", sfx.value);
        setMixerValues("Sound Effects", music.value);

    }


    void setMixerValues(string channelName, float value)
    {
        if(value <= 0.0001f)
        {
            audioMixer.SetFloat(channelName, -80f);
        }
        else
        {
            audioMixer.SetFloat(channelName, Mathf.Log10(value / 100f) * 20f);
        }
    }



}
