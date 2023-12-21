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
    public Button MenuButton2;
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
    public int GunUpgradeCost_ = 10;
    public int HealthUpgradeCost_ = 10;

    Player player;
    Health SetMaxHealth;

    public static UpgradeMenu instance;

    void Start (){
        //--------Initializes menu buttons-------------------
        Button btn = MenuButton1.GetComponent<Button>();
        Button btn2 = MenuButton2.GetComponent<Button>();
        Button btn3 = GunUpgrade.GetComponent<Button>();
        Button btn4 = HealthUpgrade.GetComponent<Button>();

        //--------Links buttons to functions-------------------
        btn.onClick.AddListener(OnButtonPress);
        btn2.onClick.AddListener(TaskOnClicka);
        btn3.onClick.AddListener(Gun);
        btn4.onClick.AddListener(Health);

        //----------------UI button stuff--------------------------------
        MenuButton2.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.0f);
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

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }

        isMenuOpen = false;
    }

    void Awake(){

        player = FindObjectOfType<Player>();
        SetMaxHealth = FindObjectOfType<Health>();
    }

    public void TaskOnClicka () {
        Debug.Log("MenuButton2 pressed !!!");

        if (isMenuOpen == true){
            MenuButton1.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1.0f);
            MenuButton2.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.0f);
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

    public void OnButtonPress() {
        Debug.Log("MenuButton1 pressed !!!");

        if (isMenuOpen == false){
            MenuButton1.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.0f);
            MenuButton2.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1.0f);
            UpgradePoints.color = new Color(1f, 1f, 1f, 1f);
            GunUpgradeCost.color = new Color(1f, 1f, 1f, 1f);
            HealthUpgradeCost.color = new Color(1f, 1f, 1f, 1f);
            WeaponLevel.color = new Color(1f, 1f, 1f, 1f);
            HealthLevels.color = new Color(1f, 1f, 1f, 1f);

            if (HealthLevel == 4) {
                HealthUpgrade.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3f);
            }
            if (GunLevel == 4) {
                GunUpgrade.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3f);
            }
            if (GunLevel < 3) {
                GunUpgrade.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            if (HealthLevel < 3) {
                HealthUpgrade.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            Image.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.95f);

            Time.timeScale = 0f;

            isMenuOpen = true;
        }
    }

    public void Gun() {
        if (isMenuOpen == true) {
            Debug.Log("Gun Button pressed !!!");
            float upgradeValue = 0.1f;
            int nextUPcost = 20; 

            if (GunLevel <= 3 && GunUpgradeCost_ <= UpgradePoints_) {
                GunLevel = GunLevel + 1;
                player.shootCooldown = player.shootCooldown - upgradeValue;

                UpgradePoints_ = UpgradePoints_ - GunUpgradeCost_;
                GunUpgradeCost_ = GunUpgradeCost_ + nextUPcost;

                SetWeaponText();
                SetUPText();

                Debug.Log("Gun Upgraded!!!");
            }

            if (GunLevel == 4){
                GunUpgrade.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3f);
            }
        }
    }

    public void Health () {
        if (isMenuOpen == true) {
            Debug.Log("Health Button pressed!!!");

            float upgradeValue = 10f;
            int nextUPcost = 20;

            if (HealthLevel <= 3 && HealthUpgradeCost_ <= UpgradePoints_){
                Debug.Log("Health Upgraded!!!");
                HealthLevel++;
                Debug.Log("Health Level ++" + HealthLevel);
                
                //------------Set players new max health and heal player to max health----------------
                player.maxHealth = player.maxHealth + upgradeValue;
                player.currentHealth = player.maxHealth;

                //------------Update Health slyder to new max health value----------------------------
                SetMaxHealth.SetMaxHealth(player.maxHealth);
                SetMaxHealth.UpdateHealthBar(player.currentHealth, player.maxHealth);
                SetMaxHealth.SetHealth(player.currentHealth);

                UpgradePoints_ = UpgradePoints_ - HealthUpgradeCost_;
                HealthUpgradeCost_ = HealthUpgradeCost_ + nextUPcost;

                SetHealthText();
                SetUPText();
                Debug.Log("Health Upgraded!!!");
            }

            if (HealthLevel == 4){
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
            if (GunLevel == 4){
                WeaponLevel.text = "Gun Level: MAX";
                GunUpgradeCost.text = "Next Gun Upgrade Cost: ";
            }
        }
    }

    void SetHealthText() {
        if (HealthLevels != null){
            HealthLevels.text = "Health Level: " + HealthLevel;
            if (HealthLevel == 4){
                HealthLevels.text = "Health Level: MAX";
                HealthUpgradeCost.text = "Next Health Upgrade Cost: ";
            }
        }

        if (HealthUpgradeCost != null) {
            HealthUpgradeCost.text = "Next Health Upgrade Cost: " + HealthUpgradeCost_;
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

        Debug.Log("Upgrades Reset: ");
     }
}