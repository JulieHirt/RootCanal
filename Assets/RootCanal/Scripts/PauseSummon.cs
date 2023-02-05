using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RootCanal
{
    public class PauseSummon : MonoBehaviour
    {
        [SerializeField] public GameObject pauseMenu;
        [SerializeField] public MonoBehaviour CameraMovementBehaviour;
        [SerializeField] public MonoBehaviour TileSelectBehaviour;
        

        public void Pause()
        {
            
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            CameraMovementBehaviour.enabled = false;
            TileSelectBehaviour.enabled = false;
            
        }

        public void Resume()
        {
            Debug.Log("Return Button was Pressed");
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            CameraMovementBehaviour.enabled = true;
            TileSelectBehaviour.enabled = true;
            
        }

        public void Home(string sceneName)
        {
            Debug.Log("Exit Button was Pressed");
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            CameraMovementBehaviour.enabled = true;
            TileSelectBehaviour.enabled = true;
            
            SceneManager.LoadScene(sceneName);
        }

        
        // Update is called once per frame 
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Debug.Log("Escape key was Pressed");
                Pause();
            }
        }
    }
}
