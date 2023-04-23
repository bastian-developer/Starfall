using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private float sceneLoadDelay = 1f;

    private ScoreKeeper _scoreKeeper;
    

    private void Start()
    {
        _scoreKeeper = FindObjectOfType<ScoreKeeper>();
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
        

        //SceneManager.LoadScene("MainScene");
        
        
        StartCoroutine(WaitAndLoad("MainScene", sceneLoadDelay));
    }

    
    public void LoadMenu()
    {
        //SceneManager.LoadScene("MainMenu");
        StartCoroutine(WaitAndLoad("MainMenu", sceneLoadDelay));
    }
    
    public void LoadGameOver()
    {
        //StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
        StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
    }
    
    public void QuitGame()
    {
        //Application.Quit();
        StartCoroutine(WaitAndQuit(sceneLoadDelay));
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    
    IEnumerator WaitAndQuit(float delay)
    {
        yield return new WaitForSeconds(delay);
        Application.Quit();
    }
}
