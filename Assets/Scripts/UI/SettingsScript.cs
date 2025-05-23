using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] Toggle toggleHUDTime;
    [SerializeField] Toggle toggleHUDScore;
    [SerializeField] Toggle toggleHUDAmmo;
    [SerializeField] Toggle toggleHUDMoney;
    [SerializeField] Toggle toggleHUDHealthpoints;
    [SerializeField] Toggle toggleHUDKeycard;
    [SerializeField] Toggle toggleHudLevelIndicator;

    [Space]
    [Header("HUD Elements")]
    [SerializeField] GameObject HUDTime;
    [SerializeField] GameObject HUDScore;
    [SerializeField] GameObject HUDAmmo;
    [SerializeField] GameObject HUDMoney;
    [SerializeField] GameObject HUDHealthpoints;
    [SerializeField] GameObject HUDKeycard;
    [SerializeField] GameObject HUDLevelIndicator;



    void Start()
    {
        // Assign toggle event listeners
        toggleHUDTime.onValueChanged.AddListener(OnHUDTimeToggled);
        toggleHUDScore.onValueChanged.AddListener(OnHUDScoreToggled);
        toggleHUDAmmo.onValueChanged.AddListener(OnHUDAmmoToggled);
        toggleHUDMoney.onValueChanged.AddListener(OnHUDMoneyToggled);
        toggleHUDHealthpoints.onValueChanged.AddListener(OnHUDHealthpointsToggled);
        toggleHUDKeycard.onValueChanged.AddListener(OnHUDAnnouncementToggled);
        toggleHudLevelIndicator.onValueChanged.AddListener(OnHUDLevelIndicatorToggled);

        // Initialize HUD visibility based on current toggle states
        OnHUDTimeToggled(toggleHUDTime.isOn);
        OnHUDScoreToggled(toggleHUDScore.isOn);
        OnHUDAmmoToggled(toggleHUDAmmo.isOn);
        OnHUDMoneyToggled(toggleHUDMoney.isOn);
        OnHUDHealthpointsToggled(toggleHUDHealthpoints.isOn);
        OnHUDAnnouncementToggled(toggleHUDKeycard.isOn);
        OnHUDLevelIndicatorToggled(toggleHudLevelIndicator.isOn);
    }

    void OnHUDTimeToggled(bool isOn) => HUDTime.SetActive(isOn);
    void OnHUDScoreToggled(bool isOn) => HUDScore.SetActive(isOn);
    void OnHUDAmmoToggled(bool isOn) => HUDAmmo.SetActive(isOn);
    void OnHUDMoneyToggled(bool isOn) => HUDMoney.SetActive(isOn);
    void OnHUDHealthpointsToggled(bool isOn) => HUDHealthpoints.SetActive(isOn);
    void OnHUDAnnouncementToggled(bool isOn) => HUDKeycard.SetActive(isOn);
    void OnHUDLevelIndicatorToggled(bool isOn) => HUDLevelIndicator.SetActive(isOn);
}
