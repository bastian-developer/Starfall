using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private float sceneLoadDelay = 2f;

    private ScoreKeeper _scoreKeeper;

    private void Start()
    {
        _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        Debug.Log("xd1" + _scoreKeeper.GetInstanceID());

    }
    
    void findScoreKeeper()
    {
        if(_scoreKeeper == null)
        {
            _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        }
    }

    public void LoadGame()
    {
        findScoreKeeper();
        _scoreKeeper.ResetScore();
        SceneManager.LoadScene("MainScene");
    }
    
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void LoadGameOver()
    {
        StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
