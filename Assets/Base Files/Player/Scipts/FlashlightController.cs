using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    public PlayerInputs playerInputs;
    [SerializeField] GameObject flashLight;

    [Header("Torch Variables")]
    [SerializeField] private float range, spotAngle, intensity;
    [Header("bat Variables")]
    [SerializeField] private float batLifeDuration;
    [SerializeField] bool randomBatReload;
    [SerializeField] private float maxBatReload, minBatReload;

    private float maxBat = 100f, curBat, batDrainPerSecond, batReloadAmount;
    private bool hasTorch, flashOn;
    private Light flash;

    private void Awake()
    {
        playerInputs = new PlayerInputs();
    }
    private void OnEnable()
    {
        playerInputs.Enable();
    }
    private void OnDisable()
    {
        playerInputs.Disable();
    }

    private void Start()
    {
        flash = flashLight.gameObject.GetComponent<Light>();
        batDrainPerSecond = maxBat / batLifeDuration;
    }

    private void Update()
    {
        if (playerInputs.Player.FlashLight.WasPressedThisFrame())
        {
            ToggleFlashLight();
        }

        if (flashOn)
        {
            curBat -= batDrainPerSecond * Time.deltaTime;
        }
    }

    private void ReloadFlashLight()
    {
        batReloadAmount = Random.Range(minBatReload, maxBatReload);
        curBat = Mathf.Clamp(curBat + batReloadAmount, 0, maxBat);
    }
    private void ToggleFlashLight()
    {
        if (flashOn)
        {
            flash.enabled = false;
            flashOn = false;
        } else
        {
            if(curBat <= 0)
            {
                return;
            }

            flash.enabled = true;
            flashOn = true;
        }
    }
}
