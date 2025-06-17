using UnityEngine;
using UnityEngine.UI;

//this script is for the settings tab in the manu, so you can disable certain hud elements
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
    [SerializeField] Toggle toggleHudGrenades;

    [Space]
    [Header("HUD Elements")]
    [SerializeField] GameObject HUDTime;
    [SerializeField] GameObject HUDScore;
    [SerializeField] GameObject HUDAmmo;
    [SerializeField] GameObject HUDMoney;
    [SerializeField] GameObject HUDHealthpoints;
    [SerializeField] GameObject HUDKeycard;
    [SerializeField] GameObject HUDLevelIndicator;
    [SerializeField] GameObject HUDGrendaes;



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
        toggleHudGrenades.onValueChanged.AddListener(OnHUDGrenadesToggled);

        // Initialize HUD visibility based on current toggle states
        OnHUDTimeToggled(toggleHUDTime.isOn);
        OnHUDScoreToggled(toggleHUDScore.isOn);
        OnHUDAmmoToggled(toggleHUDAmmo.isOn);
        OnHUDMoneyToggled(toggleHUDMoney.isOn);
        OnHUDHealthpointsToggled(toggleHUDHealthpoints.isOn);
        OnHUDAnnouncementToggled(toggleHUDKeycard.isOn);
        OnHUDLevelIndicatorToggled(toggleHudLevelIndicator.isOn);
        OnHUDGrenadesToggled(toggleHudGrenades.isOn);
    }

    void OnHUDTimeToggled(bool isOn) => HUDTime.SetActive(isOn);
    void OnHUDScoreToggled(bool isOn) => HUDScore.SetActive(isOn);
    void OnHUDAmmoToggled(bool isOn) => HUDAmmo.SetActive(isOn);
    void OnHUDMoneyToggled(bool isOn) => HUDMoney.SetActive(isOn);
    void OnHUDHealthpointsToggled(bool isOn) => HUDHealthpoints.SetActive(isOn);
    void OnHUDAnnouncementToggled(bool isOn) => HUDKeycard.SetActive(isOn);
    void OnHUDLevelIndicatorToggled(bool isOn) => HUDLevelIndicator.SetActive(isOn);
    void OnHUDGrenadesToggled(bool isOn) => HUDGrendaes.SetActive(isOn);
}
