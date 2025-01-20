using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct CarData {
        public string name;
        public string description;
        public GameObject selectorPrefab;
        public CarController racePrefab;
    }

    [System.Serializable]
    public struct TrackData {
        public string name;
        public string description;
        public GameObject selectorModel;
        public string sceneName;
    }

    public CameraController playerCameraPrefab;
    public RaceHUDController hudPrefab;
    public Canvas pauseMenu;
    public CarData[] carList;
    public TrackData[] trackList;
    public int trackSelectedIndex;
    public int[] playersSelectedCarIndexes;
    public string initialScene;
    public int totalLaps = 3;
    public int numberOfPlayers = 1;

    public bool gamePaused = false;

    public static GameManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }
    }

    void Start()
    {
        pauseMenu = GetComponent<Canvas>();
        LoadInitialScene();
        ResumeGame();
    }

    /// <summary>
    /// Carga la escena inicial
    /// </summary>
    public void LoadInitialScene() {
        // SI el nombre de la escena inicial no está vacío o es nulo
        if (!string.IsNullOrEmpty(initialScene)) {
            SceneManager.LoadScene(initialScene);
        } else {
            //Debug.LogError("[GameManager] InitialScene empty or null");
        }
    }

    public void LoadSceneByName(string name) {
        SceneManager.LoadScene(name);
    }

    public void ReloadCurrentScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadSceneBySelectedTrack() {
        SceneManager.LoadScene(trackList[trackSelectedIndex].sceneName);
    }

    public void SetGamePaused() {
        pauseMenu.gameObject.SetActive(true);
        gamePaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        pauseMenu.gameObject.SetActive(false);
        gamePaused = false;
        Time.timeScale = 1;
    }

    public void SetTrackSelectedIndex(int index) {
        trackSelectedIndex = index;
    }

    public void SetPlayerSelectedCarsSize(int numPlayer) {
        int[] carsSelectedAux = new int[numPlayer];
        playersSelectedCarIndexes = carsSelectedAux;
    }

    public void SetCarSelected(int player, int index) {
        playersSelectedCarIndexes[player] = index;
    }

    public void QuitGame() {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
                Application.Quit();
            }
}
