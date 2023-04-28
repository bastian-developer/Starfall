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
    
    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();

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
    
    public bool PayEnergyCost(int energyCost, String source)
    {

        if (currentEnergy <= energyCost)
        {
            if (source == "Shielding")
            {
                _player.StopShield();
            }
            return false;
        }
        else
        {
            currentEnergy -= energyCost;
            Debug.Log("Paid" + currentEnergy + " on " + source);
            return true;
        }
    }
    
    public void AddEnergy(int energyAmount)
    {
        
        currentEnergy += energyAmount;

        Debug.Log("Add Energy" + currentEnergy);

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
