using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private float sceneLoadDelay = 1f;
    
    [SerializeField] private Animator animator;
    
    
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

        StartCrossfadeTransition();
        StartCoroutine(WaitAndLoad("MainScene", sceneLoadDelay));
    }

    
    public void LoadGameOver()
    {
        //StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
        StartCrossfadeTransition();
        StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
    }
    
    public void LoadMenu()
    {
        //SceneManager.LoadScene("MainMenu");
        StartCrossfadeTransition();
        StartCoroutine(WaitAndLoad("MainMenu", sceneLoadDelay));
    }
    
    public void QuitGame()
    {
        //Application.Quit();
        StartCrossfadeTransition();
        StartCoroutine(WaitAndQuit(sceneLoadDelay));
    }

    public void StartCrossfadeTransition()
    {
        animator.SetTrigger("Start");
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
