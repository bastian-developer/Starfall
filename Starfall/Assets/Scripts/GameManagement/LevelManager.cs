using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private float sceneLoadDelay = 1f;
        [SerializeField] private Animator animator;
        
        private ScoreKeeper _scoreKeeper;

        private void Start()
        {
            _scoreKeeper = FindObjectOfType<ScoreKeeper>();
        }
    
        private void FindScoreKeeper()
        {
            if(_scoreKeeper == null)
            {
                _scoreKeeper = FindObjectOfType<ScoreKeeper>();
            }
        }

        public void LoadGame()
        {
            FindScoreKeeper();
            _scoreKeeper.ResetScore();
            StartCrossfadeTransition();
            StartCoroutine(WaitAndLoad("MainScene", sceneLoadDelay));
        }

    
        public void LoadGameOver()
        {
            StartCrossfadeTransition();
            StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
        }
    
        public void LoadMenu()
        {
            StartCrossfadeTransition();
            StartCoroutine(WaitAndLoad("MainMenu", sceneLoadDelay));
        }
    
        public void QuitGame()
        {
            StartCrossfadeTransition();
            StartCoroutine(WaitAndQuit(sceneLoadDelay));
        }

        private void StartCrossfadeTransition()
        {
            animator.SetTrigger("Start");
        }

        private static IEnumerator WaitAndLoad(string sceneName, float delay)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(sceneName);
        }
    
        private static IEnumerator WaitAndQuit(float delay)
        {
            yield return new WaitForSeconds(delay);
            Application.Quit();
        }
    }
}
