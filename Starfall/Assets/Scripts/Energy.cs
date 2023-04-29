using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Characters;
using Powers;

public class Energy : MonoBehaviour
{
    [SerializeField] private int maxEnergy = 100;
    [SerializeField] private int passiveEnergyRestoration = 1;
    [SerializeField] private float passiveEnergyRestorationDelay = 1;
    
    private Player _player;
    private ShieldManager _shieldManager;
    private int _currentEnergy;
    
    private Coroutine _restoreEnergyCoroutine;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _shieldManager = FindObjectOfType<ShieldManager>();
        _currentEnergy = maxEnergy;
    }

    private void Update()
    {
        ManageEnergyRestoration();
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
                _shieldManager.StopShield();
            }
            return false;
        }
        else
        {
            _currentEnergy -= energyCost;
            //Debug.Log("Paid" + energyCost + " on " + source);
            //Debug.Log("Total reamining energy " + _currentEnergy);

            return true;
        }
    }
    
    private void AddEnergy(int energyAmount)
    {
        
        _currentEnergy += energyAmount;

        Debug.Log("Add Energy " + energyAmount);

    }
    
    private bool ShouldRestoreEnergy()
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
    
    private void ManageEnergyRestoration()
    {
        if (ShouldRestoreEnergy() && _restoreEnergyCoroutine == null)
        {
            _restoreEnergyCoroutine = StartCoroutine(AddEnergyOverTime());
        }
        else if (!ShouldRestoreEnergy() && _restoreEnergyCoroutine != null)
        {
            StopCoroutine(_restoreEnergyCoroutine);
            _restoreEnergyCoroutine = null;
        }
    }
    
    private IEnumerator AddEnergyOverTime()
    {
        while (_player)
        {
            yield return new WaitForSeconds(passiveEnergyRestorationDelay);
            AddEnergy(passiveEnergyRestoration);
        }
    }
    
}
