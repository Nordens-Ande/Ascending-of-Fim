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

    void Awake()
    {
        setMixerValues("MasterVolym", 30f);
        setMixerValues("SoundEffectVolume", 100f);
        setMixerValues("BackgroundVolume", 25f);
        
    }

    private void Start()
    {
        main.SetValueWithoutNotify(30);
        sfx.SetValueWithoutNotify(100);
        music.SetValueWithoutNotify(25);

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

        setMixerValues("MasterVolym", main.value);
        setMixerValues("BackgroundVolume", music.value);
        setMixerValues("SoundEffectVolume", sfx.value);

        //Debug.Log($"Master: {main.value}, SFX: {sfx.value}, Music: {music.value}");

    }


    void setMixerValues(string channelName, float value)
    {
        if(value <= 0.0001f)
        {
            audioMixer.SetFloat(channelName, -80f);
        }
        else
        {
            audioMixer.SetFloat(channelName, Mathf.Log10(value / 100f) * 30f);
        }
    }



}
