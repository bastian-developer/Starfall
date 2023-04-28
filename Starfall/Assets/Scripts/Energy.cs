using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Energy : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 100;
    [SerializeField] private int currentEnergy = 100;
    [SerializeField] private int passiveEnergyRestoration = 1;
    [SerializeField] private float passiveRestorationDelay = 1;


    private void Start()
    {
        //StartCoroutine(AddEnergyPerSecond());
    }

    private void Update()
    {
        if (currentEnergy >= maxEnergy)
        {
           // StopCoroutine(AddEnergyPerSecond());
        }
    }

    IEnumerator AddEnergyPerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(passiveRestorationDelay);
            currentEnergy += passiveEnergyRestoration;
            Debug.Log(maxEnergy);
            Debug.Log(currentEnergy);
        }
    }

    public int GetPassiveEnergyRestoration()
    {
        return passiveEnergyRestoration;
    }
    
    public float GetPassiveRestorationDelay()
    {
        return passiveRestorationDelay;
    }
    
    public int GetCurrentEnergy()
    {
        return currentEnergy;
    }
    
    public int GetMaxEnergy()
    {
        return maxEnergy;
    }
    
    public bool PayEnergyCost(int energyCost)
    {

        if (currentEnergy <= energyCost)
        {
            return false;
        }
        else
        {
            currentEnergy -= energyCost;
            return true;
        }
    }
    
    public void AddEnergy(int energyAmount)
    {
        
        currentEnergy += energyAmount;

    }
    
    public bool ShouldRestoreEnergy()
    {

        if (currentEnergy < maxEnergy)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    
}
