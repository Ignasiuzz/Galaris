using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class UpgradeMenu : MonoBehaviour
{
    public Button MenuButton1;
    public Button GunUpgrade;
    public Button HealthUpgrade;
    public TextMeshProUGUI UpgradePoints;
    public TextMeshProUGUI GunUpgradeCost;
    public TextMeshProUGUI HealthUpgradeCost;
    public TextMeshProUGUI WeaponLevel;
    public TextMeshProUGUI HealthLevels;

    public Image Image;

    public bool isMenuOpen = false;
    public int HealthLevel = 1;
    public int GunLevel = 1;
    public int UpgradePoints_ = 0;
    public int GunUpgradeCost_ = 5;
    public int HealthUpgradeCost_ = 5;
    private const int CostIncrement = 5;
    private const float BasePlayerMaxHealth = 10f;
    private const float HealthUpgradeValue = 10f;
    private const float BaseShootCooldown = 0.5f;
    private const float GunUpgradeValue = 0.1f;

    Player player;
    Health SetMaxHealth;
    private bool hasCarriedHealth = false;
    private float carriedCurrentHealth;

    public static UpgradeMenu instance;

    void Awake(){

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        RefreshSceneReferences();
    }

    void Start (){
        //--------Initializes menu buttons-------------------
        Button btn = MenuButton1.GetComponent<Button>();
        Button btn3 = GunUpgrade.GetComponent<Button>();
        Button btn4 = HealthUpgrade.GetComponent<Button>();

        //--------Links buttons to functions-------------------
        btn.onClick.AddListener(ToggleMenu);
        btn3.onClick.AddListener(Gun);
        btn4.onClick.AddListener(Health);

        //----------------UI button stuff--------------------------------
        Image.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.0f);
        GunUpgrade.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        btn4.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        //----------------UI text stuff-----------------------------------

        UpgradePoints.color = new Color(1f, 1f, 1f, 0f);
        GunUpgradeCost.color = new Color(1f, 1f, 1f, 0f);
        HealthUpgradeCost.color = new Color(1f, 1f, 1f, 0f);
        WeaponLevel.color = new Color(1f, 1f, 1f, 0f);
        HealthLevels.color = new Color(1f, 1f, 1f, 0f);

        SetUPText();
        SetWeaponText();
        SetHealthText();
        Debug.Log("GunLevel: " + GunLevel);
        Debug.Log("HealthLevel: " + HealthLevel);
        Debug.Log("GunUpgradeCost: " + GunUpgradeCost_);
        Debug.Log("HealthUpgradeCost: " + HealthUpgradeCost_);

        isMenuOpen = false;
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ApplyUpgradesAfterSceneLoad());
    }

    private IEnumerator ApplyUpgradesAfterSceneLoad()
    {
        yield return null;
        RefreshSceneReferences();
        ApplyCurrentUpgradesToPlayer(false);
    }

    private void RefreshSceneReferences(){
        player = FindObjectOfType<Player>();
        SetMaxHealth = FindObjectOfType<Health>();
    }

    public void CarryCurrentPlayerHealth()
    {
        if (player == null)
        {
            RefreshSceneReferences();
        }

        if (player == null)
        {
            return;
        }

        carriedCurrentHealth = Mathf.Clamp(player.currentHealth, 0f, player.maxHealth);
        hasCarriedHealth = true;
    }

    public void ApplyCurrentUpgradesToPlayer(bool healToFull)
    {
        if (player == null)
        {
            RefreshSceneReferences();
        }

        if (player == null)
        {
            return;
        }

        player.shootCooldown = BaseShootCooldown - ((GunLevel - 1) * GunUpgradeValue);
        player.maxHealth = BasePlayerMaxHealth + ((HealthLevel - 1) * HealthUpgradeValue);

        if (hasCarriedHealth)
        {
            player.currentHealth = Mathf.Clamp(carriedCurrentHealth, 0f, player.maxHealth);
            hasCarriedHealth = false;
        }
        else if (healToFull || player.currentHealth > player.maxHealth)
        {
            player.currentHealth = player.maxHealth;
        }

        if (SetMaxHealth != null)
        {
            SetMaxHealth.SetMaxHealth(player.maxHealth);
            SetMaxHealth.UpdateHealthBar(player.currentHealth, player.maxHealth);
            SetMaxHealth.SetHealth(player.currentHealth);
        }
    }

    public void ToggleMenu() {
        Debug.Log("MenuButton1 pressed !!!");

        if (isMenuOpen == false){
            UpgradePoints.color = new Color(1f, 1f, 1f, 1f);
            GunUpgradeCost.color = new Color(1f, 1f, 1f, 1f);
            HealthUpgradeCost.color = new Color(1f, 1f, 1f, 1f);
            WeaponLevel.color = new Color(1f, 1f, 1f, 1f);
            HealthLevels.color = new Color(1f, 1f, 1f, 1f);

            if (HealthLevel == 12) {
                HealthUpgrade.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3f);
            }
            if (GunLevel == 12) {
                GunUpgrade.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3f);
            }
            if (GunLevel < 11) {
                GunUpgrade.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            if (HealthLevel < 11) {
                HealthUpgrade.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            Image.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.95f);

            Time.timeScale = 0f;

            isMenuOpen = true;
        }
        else {
            GunUpgrade.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            HealthUpgrade.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            Image.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.0f);
            UpgradePoints.color = new Color(1f, 1f, 1f, 0f);
            GunUpgradeCost.color = new Color(1f, 1f, 1f, 0f);
            HealthUpgradeCost.color = new Color(1f, 1f, 1f, 0f);
            WeaponLevel.color = new Color(1f, 1f, 1f, 0f);
            HealthLevels.color = new Color(1f, 1f, 1f, 0f);

            Time.timeScale = 1f;

            isMenuOpen = false;
        }
    }

    public void Gun() {
        if (isMenuOpen == true) {
            Debug.Log("Gun Button pressed !!!");

            if (GunLevel <= 11 && GunUpgradeCost_ <= UpgradePoints_) {
                GunLevel = GunLevel + 1;
                ApplyCurrentUpgradesToPlayer(false);

                UpgradePoints_ = UpgradePoints_ - GunUpgradeCost_;
                GunUpgradeCost_ = GunUpgradeCost_ + CostIncrement;

                SetWeaponText();
                SetUPText();

                Debug.Log("Gun Upgraded!!!");
            }

            if (GunLevel == 12){
                GunUpgrade.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3f);
            }
        }
    }

    public void Health () {
        if (isMenuOpen == true) {
            Debug.Log("Health Button pressed!!!");

            if (HealthLevel <= 11 && HealthUpgradeCost_ <= UpgradePoints_){
                Debug.Log("Health Upgraded!!!");
                HealthLevel++;
                Debug.Log("Health Level ++" + HealthLevel);

                //------------Set players new max health and heal player to max health----------------
                ApplyCurrentUpgradesToPlayer(true);

                UpgradePoints_ = UpgradePoints_ - HealthUpgradeCost_;
                HealthUpgradeCost_ = HealthUpgradeCost_ + CostIncrement;

                SetHealthText();
                SetUPText();
                Debug.Log("Health Upgraded!!!");
            }

            if (HealthLevel == 12){
                HealthUpgrade.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3f);
            }
        }
    }

    void SetWeaponText() {
        if (WeaponLevel != null){
            WeaponLevel.text = "Weapon Level: " + GunLevel;
        }

        if (GunUpgradeCost != null){
            GunUpgradeCost.text = "Next Weapon Upgrade Cost: " + GunUpgradeCost_;
            if (GunLevel == 12){
                WeaponLevel.text = "Gun Level: MAX";
                GunUpgradeCost.text = "Next Gun Upgrade Cost: ";
            }
        }
    }

    void SetHealthText() {
        if (HealthLevels != null){
            HealthLevels.text = "Health Level: " + HealthLevel;
            if (HealthLevel == 12){
                HealthLevels.text = "Health Level: MAX";
            }
        }

        if (HealthUpgradeCost != null) {
            if (HealthLevel == 12) {
                HealthUpgradeCost.text = "Next Health Upgrade Cost: ";
            } else {
                HealthUpgradeCost.text = "Next Health Upgrade Cost: " + HealthUpgradeCost_;
            }
        }
    }

    public void SetUPText() {
        Debug.Log("Points Added: " + UpgradePoints_);
        
        if (UpgradePoints != null){
            UpgradePoints.GetComponent<TextMeshProUGUI>().text = $"Upgrade Points: {UpgradePoints_}";
        }
    }

     public void ResetUpgrades() {
        UpgradePoints_ = 0;
        GunLevel = 1;
        HealthLevel = 1;
        HealthUpgradeCost_ = 10;
        GunUpgradeCost_ = 10;

        SetUPText();
        SetHealthText();
        SetWeaponText();
        ApplyCurrentUpgradesToPlayer(true);

        Debug.Log("Upgrades Reset: ");
     }
}
