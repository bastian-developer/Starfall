using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{

    private int _score;
    
    private static ScoreKeeper _scoreKeeperInstance;

    public ScoreKeeper GetInstance()
    {
        return _scoreKeeperInstance;
    }
    
    private void Awake()
    {
        ManageSingleton();
    }

    void ManageSingleton()
    {
        if(_scoreKeeperInstance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            _scoreKeeperInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public int GetScore()
    {
        return _score;
    }

    public void ModifyScore(int value)
    {
        _score += value;
        Mathf.Clamp(_score, 0, int.MaxValue);
    }

    public void ResetScore()
    {
        _score = 0;
    }
}
