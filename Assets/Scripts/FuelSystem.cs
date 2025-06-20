using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FuelSystem : MonoBehaviour
{
    public float fuel;
    private float dash;
    public float maxDash = 100;
    public float lerpTimer;
    public float maxFuel = 100;
    public float chipSpeed = 2;
    public Image frontFuelBar;
    public Image backFuelBar;
    public Image frontDashBar;
    public Image backDashBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fuel = maxFuel;
        dash = maxDash;
    }

    // Update is called once per frame
    void Update()
    {
        fuel = Mathf.Clamp(fuel, 0, maxFuel);
        dash = Mathf.Clamp(dash, 0, maxDash);
        UpdateFuelUI();
        UpdateDashUI();
    }

    public void UpdateFuelUI()
    {
        float fillF = frontFuelBar.fillAmount;
        float fillB = backFuelBar.fillAmount;
        float fFraction = fuel / maxFuel;
        if (fillB > fFraction)
        {
            frontFuelBar.fillAmount = fFraction;
            backFuelBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backFuelBar.fillAmount = Mathf.Lerp(fillB, fFraction, percentComplete);
        }
        if (fillF < fFraction)
        {
            backFuelBar.color = Color.green;
            backFuelBar.fillAmount = fFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontFuelBar.fillAmount = Mathf.Lerp(fillF, backFuelBar.fillAmount, percentComplete);   

        }
    }

    public void UpdateDashUI()
    {
        float fillF = frontDashBar.fillAmount;
        float fillB = backDashBar.fillAmount;
        float dFraction = dash / maxDash;
        if (fillB > dFraction)
        {
            frontDashBar.fillAmount = dFraction;
            backDashBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backDashBar.fillAmount = Mathf.Lerp(fillB, dFraction, percentComplete);
        }
        if (fillF < dFraction)
        {
            backDashBar.color = Color.green;
            backDashBar.fillAmount = dFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontDashBar.fillAmount = Mathf.Lerp(fillF, backDashBar.fillAmount, percentComplete);

        }
    }

    
    public void ConsumeFuel(float usage)
    {
        fuel -= usage;
        lerpTimer = 0;
    }

    public void RefillFuel(float fuelAmount)
    {
        fuel += fuelAmount;
        lerpTimer = 0;
    }

    public void UseDash(float use)
    {
        dash -= use;
        lerpTimer = 0;
    }

    public void DashCooldown(float duration)
    {
        StartCoroutine(RefillDashBar(duration));
    }

    private IEnumerator RefillDashBar(float duration)
    {
        float elapsed = 0f;
        float startDash = dash;
        float targetDash = maxDash;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            dash = Mathf.Lerp(startDash, targetDash, elapsed / duration);
            yield return null;
        }

        dash = maxDash;
    }

    public float GetCurrentFuel()
    {
        return fuel;
    }

    public float GetCurrentDashAmount()
    {
        return dash;
    }
}
