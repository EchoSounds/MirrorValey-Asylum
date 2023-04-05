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
    [SerializeField] private float range;
    [SerializeField] private float spotAngle;
    [SerializeField] private float intensity;
    [Header("bat Variables")]
    [SerializeField] private float batLifeDuration;
    [SerializeField] bool randomBatReload;
    [SerializeField] private float maxBatReload;
    [SerializeField] private float minBatReload;

    private float maxBat = 100f, curBat = 100f, batDrainPerSecond, batReloadAmount;
    private bool equiped, flashOn = false;
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
        batDrainPerSecond = maxBat / batLifeDuration;
        flashLight.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (equiped)
        {
            if (playerInputs.Player.FlashLight.triggered)
            {
                ToggleFlashLight();
                Debug.Log("pog");
            }

            if (flashOn)
            {
                curBat -= batDrainPerSecond * Time.deltaTime;
            }
        }
    }

    private void ReloadFlashLight()
    {
        batReloadAmount = Random.Range(minBatReload, maxBatReload);
        curBat = Mathf.Clamp(curBat + batReloadAmount, 0, maxBat);
    }
    private void ToggleFlashLight()
    {
        if (flashOn == true)
        {
            flashLight.gameObject.SetActive(false);
            flashOn = false;
        } else if (flashOn == false)
        {
            if(curBat < 0)
            {
                return;
            }
            flashLight.gameObject.SetActive(true);
            flashOn = true;
        }
    }
}
