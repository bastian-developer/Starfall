using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Characters;

public class Energy : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 100;
    private int _currentEnergy;
    [SerializeField] private int passiveEnergyRestoration = 1;
    [SerializeField] private float passiveEnergyRestorationDelay = 1;
    
    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _currentEnergy = maxEnergy;

    }

    public int GetPassiveEnergyRestoration()
    {
        return passiveEnergyRestoration;
    }
    
    public float GetPassiveRestorationDelay()
    {
        return passiveEnergyRestorationDelay;
    }
    
    public int GetCurrentEnergy()
    {
        return _currentEnergy;
    }
    
    public int GetMaxEnergy()
    {
        return maxEnergy;
    }
    
    public bool PayEnergyCost(int energyCost, String source)
    {

        if (_currentEnergy <= energyCost)
        {
            if (source == "Shielding")
            {
                _player.StopShield();
            }
            return false;
        }
        else
        {
            _currentEnergy -= energyCost;
            Debug.Log("Paid" + energyCost + " on " + source);
            Debug.Log("Total reamining energy " + _currentEnergy);

            return true;
        }
    }
    
    public void AddEnergy(int energyAmount)
    {
        
        _currentEnergy += energyAmount;

        Debug.Log("Add Energy " + energyAmount);

    }
    
    public bool ShouldRestoreEnergy()
    {

        if (_currentEnergy < maxEnergy)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    
}
