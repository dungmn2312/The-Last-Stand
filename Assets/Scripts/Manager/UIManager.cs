using GunSpace;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EnemySpace;
public class UIManager : MonoBehaviour, IObserverGun
{
    public static UIManager Instance { get; set; }

    //private Vector3 floatingDamageTextOffset = new Vector3(0f, 2f, 0f);
    private float HPLerpSpeed = 1f;

    public PlayerController playerController;

    [Header("--- Panel ---")]
    public GameObject weaponPanel;
    public GameObject gameOverPanel;
    public GameObject winningText;
    public GameObject waitForNextLevel;

    [Header("--- Image ---")]
    public Image activeGunImg;
    public Image unActiveGunImg;
    public Image bloodScreen;
    public Image blackScreen, gameOverScreen;

    [Header("--- Text ---")]
    public TextMeshProUGUI currentAmmoTxt;
    public TextMeshProUGUI totalAmmoTxt;
    public TextMeshProUGUI grenadeAmountTxt;
    public TextMeshProUGUI levelNumberTxt, enemyLeftTxt, countDownTxt;

    [Header("--- Button ---")]
    public Button tryAgainBtn;
    public Button mainMenuBtn;

    private Color normalColor = new Color(1, 1, 1);
    private Color redColor = new Color(200 / 255f, 40 / 255f, 40 / 255f);

    private bool isFading;

    private float fadeDuration, darkenDuration = 0.2f, blackOutDuration = 3f, gameOverDuration = 5f;
    private float targetAlphaVal;
    private float minAlphaVal = 0f, maxAlphaVal = 1f;

    private int attackCount = 0, prevAttackCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetGunSprite();
        tryAgainBtn.onClick.AddListener(() => GameManager.Instance.LoadScene(GameManager.Instance.currentLevel));
        mainMenuBtn.onClick.AddListener(() => GameManager.Instance.LoadScene(0));
    }

    private void OnEnable()
    {
        weaponPanel.SetActive(true);
        GunController.OnGunShoot += OnNotifyGunShoot;
        GunController.OnHitShoot += OnNotifyHitShoot;
    }

    private void OnDisable()
    {
        GunController.OnGunShoot -= OnNotifyGunShoot;
        GunController.OnHitShoot -= OnNotifyHitShoot;
    }

    #region || --- Observer Method --- ||
    public void OnNotifyGunShoot(GunController gunController)
    {
        UpdateAmmo(gunController.currentAmmo, ref gunController.totalAmmo);
    }

    public void OnNotifyHitShoot(EnemyController enemy, Vector3 hitPos, int damage, bool isCrit)
    {
        //ShowFloatingDamage(enemy.transform, hitPos, damage, isCrit);
        enemy.HPSLider.value = isCrit ? enemy.HP - damage * 2 : enemy.HP - damage;
        EaseHP(enemy.HPSLider, enemy.EaseHPSlider);
    }

    public void OnNotifyEnemyAttack()
    {
        playerController.HPSlider.value = playerController.HP;
        EaseHP(playerController.HPSlider, playerController.EaseHPSlider);

        BloodScreenEffect();
    }

    public void OnNotifyTakeHealUp()
    {
        playerController.EaseHPSlider.value = playerController.HP;
        EaseHP(playerController.EaseHPSlider, playerController.HPSlider);
    }
    #endregion

    #region || --- Set Text --- ||
    public void UpdateGrenadeAmount(int amount)
    {
        grenadeAmountTxt.SetText(amount.ToString());
    }

    public void SetLevelNumber(int number)
    {
        levelNumberTxt.SetText(number.ToString());
    }

    public void UpdateAmmo(int currentAmmo, ref int totalAmmo)
    {
        currentAmmoTxt.SetText($"{currentAmmo}");
        if (totalAmmo > 9000)
        {
            totalAmmo = 9999;
            totalAmmoTxt.SetText("∞");
        }
        else
        {
            totalAmmoTxt.SetText($"{totalAmmo}");
        }
    }

    public void SetEnemyLeft(int quantity)
    {
        string text = quantity >= 10 ? quantity.ToString() : ("0" + quantity.ToString());
        enemyLeftTxt.SetText(text);
    }
    #endregion

    public async void CountDownLevel()
    {
        int level = GameManager.Instance.currentLevel;

        if (level == 3)
        {
            OnPlayerWin();
        }
        else
        {
            int currentTime = 5;
            waitForNextLevel.SetActive(true);
            while (currentTime > 0)
            {
                countDownTxt.SetText(currentTime.ToString());
                await UniTask.WaitForSeconds(1);

                currentTime--;
            }

            GameManager.Instance.LoadScene(++level);
        }
    }

    

    public void SwitchGun(int currentAmmo, int totalAmmo)
    {
        UpdateAmmo(currentAmmo, ref totalAmmo);
        UpdateGunSprite();
    }

    
    public void UpdateGunSprite()
    {
        Sprite temp = activeGunImg.sprite;
        activeGunImg.sprite = unActiveGunImg.sprite;
        unActiveGunImg.sprite = temp;
    }

    public void SetGunSprite()
    {
        GunController gun = GunManager.Instance.gunList[1].GetComponent<GunController>();
        
        Sprite gunSprite = Resources.Load<Sprite>($"UI/{gun.gunName}");
        unActiveGunImg.sprite = gunSprite;
    }

    private void EaseHP(Slider HPSlider, Slider EaseHPSlider)
    {
        LeanTween.value(EaseHPSlider.value, HPSlider.value, HPLerpSpeed)
            .setOnUpdate((float value) =>
            {
                EaseHPSlider.value = value;
            });
    }

    private async void ShowFloatingDamage(Transform enemyTransform, Vector3 hitPos, int damage, bool isCrit)
    {

        TextMesh textMesh = UIPooling.Instance.GetFloatingText();
        textMesh.transform.SetParent(enemyTransform);
        textMesh.transform.position = hitPos;
        textMesh.transform.localRotation = Quaternion.Euler(0, 180, 0);
        string damageText = damage.ToString();
        if (isCrit)
        {
            textMesh.color = redColor;
            damageText = (damage * 2).ToString();
            if (textMesh.fontSize != 20)    textMesh.fontSize = 20;
        }
        else
        {
            textMesh.color = normalColor;
            if (textMesh.fontSize != 15)    textMesh.fontSize = 15;
        }

        textMesh.text = damageText;
        textMesh.gameObject.SetActive(true);

        await UniTask.WaitForSeconds(0.9f);

        UIPooling.Instance.ReturnFloatingText(textMesh);
    }

    public async void OnPlayerDeath()
    {

        weaponPanel.SetActive(false);
        await UniTask.WaitForSeconds(1f);
        AdjustAlphaColor(blackScreen, 0f, 1f, blackOutDuration, false);
        AdjustAlphaColor(gameOverScreen, 0f, 1f, gameOverDuration, false);

        await UniTask.WaitForSeconds(6f);
        GameManager.Instance.LockCursor(false);
        gameOverPanel.SetActive(true);
    }

    public async void OnPlayerWin()
    {
        weaponPanel.SetActive(false);
        await UniTask.WaitForSeconds(1f);
        AdjustAlphaColor(blackScreen, 0f, 1f, blackOutDuration, false);

        await UniTask.WaitForSeconds(3f);
        GameManager.Instance.LockCursor(false);
        gameOverPanel.SetActive(true);
        winningText.SetActive(true);
    }

    public async void BloodScreenEffect()
    {
        attackCount++;

        Color color = bloodScreen.color;
        targetAlphaVal = (color.a >= (200f / 255)) ? maxAlphaVal : color.a + (100f / 255);
        AdjustAlphaColor(bloodScreen, color.a, targetAlphaVal, darkenDuration, true);

        await UniTask.WaitForSeconds(2f);

        prevAttackCount++;
        if (attackCount != prevAttackCount) return;

        color = bloodScreen.color;
        targetAlphaVal = minAlphaVal;

        fadeDuration = (color.a * 255f) / 100f;
        AdjustAlphaColor(bloodScreen, color.a, targetAlphaVal, fadeDuration, false);

        attackCount = 0;
        prevAttackCount = 0;
    }

    private void AdjustAlphaColor(Image image, float startAlphaVal, float targetAlphaVal, float duration, bool isDarken)
    {
        LeanTween.value(startAlphaVal, targetAlphaVal, duration)
            .setOnUpdate((float alpha) =>
            {
                if ((attackCount != prevAttackCount) && !isDarken) return;
                Color c = image.color;
                c.a = alpha;
                image.color = c;
            });
    }

}
