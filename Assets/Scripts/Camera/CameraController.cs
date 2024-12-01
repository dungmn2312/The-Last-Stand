using Cysharp.Threading.Tasks;
using EnemySpace;
using GunSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, IObserverGrenade, IObserverGun
{
    public static CameraController Instance { get; set; }

    public PlayerMovement player;

    [SerializeField] float mouseSensitivity;

    internal Vector2 lookInput;
    float xRotation;
    internal float mouseX, mouseY;

    internal Vector3 aimCameraPos, normalCameraPos;

    private float aimTransformTime = 0.5f;

    private float shakeMagnetude = 0.05f, shakeTime = 3f;
    private Vector3 cameraInipos;
    private float elapsedTime;

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

    // Start is called before the first frame update
    void Start()
    {
        aimCameraPos = new Vector3(0f, 7f, -3f);
        normalCameraPos = new Vector3(0f, 8f, -4f);
    }

    private void OnEnable()
    {
        GrenadeController.OnGrenadeExplode += OnNotifyGrenadeExplode;
        GunController.OnGunShoot += OnNotifyGunShoot;
    }

    private void OnDisable()
    {
        GrenadeController.OnGrenadeExplode -= OnNotifyGrenadeExplode;
        GunController.OnGunShoot -= OnNotifyGunShoot;
    }

    // Update is called once per frame
    void Update()
    {
        lookInput = player.lookInput;
        Look(lookInput);
    }

    #region || --- ObserverMethod --- ||
    public void OnNotifyGrenadeExplode(GrenadeController grenade)
    {
        StartShake();
    }

    public void OnNotifyGunShoot(GunController gunController)
    {
        ShootCamera(gunController.shootTransformTime, gunController.shootCameraPos);
    }

    public void OnNotifyHitShoot(EnemyController enemy, Vector3 hitPos, int damage, bool isCrit) { }
    #endregion 

    private void Look(Vector2 lookInput)
    {
        mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        //mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        //xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, -20f, 40f);

        //transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.gameObject.transform.Rotate(Vector3.up * mouseX);
    }

    public void SetAimCamera(bool isAiming)
    {
        Vector3 pos = isAiming ? aimCameraPos : normalCameraPos;
        
        LeanTween.moveLocal(gameObject, pos, aimTransformTime)
            .setEase(LeanTweenType.easeOutQuad);
    }

    public async void ShootCamera(float shootTransformTime, Vector3 shootCameraPos)
    {
   
        Vector3 pos = aimCameraPos + shootCameraPos;
        LeanTween.moveLocal(gameObject, pos, shootTransformTime);

        await UniTask.WaitForSeconds(shootTransformTime);

        LeanTween.moveLocal(gameObject, aimCameraPos, shootTransformTime);
    }

    public void StartShake()
    {
        cameraInipos = transform.localPosition;
        elapsedTime = 0f;
        InvokeRepeating("CameraShaking", 0f, 0.005f);
        Invoke("StopShake", shakeTime);
    }

    private void CameraShaking()
    {
        elapsedTime += 0.01f;
        float dampingFactor = Mathf.Clamp01(1 - (elapsedTime / shakeTime));
        float cameraShakingOffsetX = (Random.value * 2 - 1) * shakeMagnetude * dampingFactor;
        float cameraShakingOffsetY = (Random.value * 2 - 1) * shakeMagnetude * dampingFactor;

        Vector3 cameraTempPos = transform.localPosition;
        cameraTempPos.x += cameraShakingOffsetX;
        cameraTempPos.y += cameraShakingOffsetY;
        transform.localPosition = cameraTempPos;
    }

    private void StopShake()
    {
        CancelInvoke("CameraShaking");
        LeanTween.moveLocal(gameObject, cameraInipos, 0.5f);
    }

}
