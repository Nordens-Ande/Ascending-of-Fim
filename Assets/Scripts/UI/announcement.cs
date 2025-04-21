using NUnit.Framework;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class announcement : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI announchmentText;
    [SerializeField] private GameObject announchmentObject;



    void Start()
    {
        if (announchmentText != null)
        {
            announchmentText.text = "";
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }

        if(announchmentObject != null)
        {
            announchmentObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("announcement object is missing");
        }
    }

   
    public void SetAnnouncementText(string text, float time)
    {
        if (announchmentText != null)
        {
            announchmentText.text = text;
            announchmentObject.SetActive(true);
            StartCoroutine(hideAferDelay(time));
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }


   //An IEnumerator defines a method that returns an IEnumerator object, which Unity can 
   //then control over multiple frames.This allows your method to "pause" at certain points 
   //and then continue from where it left off after some time has passed.

   //For example, in Unity, the WaitForSeconds class is often used with an IEnumerator to pause 
   //execution for a certain amount of time.
    public IEnumerator hideAferDelay(float time)
    {
        yield return new WaitForSeconds(time);
        announchmentObject.SetActive(false);
    }
}
