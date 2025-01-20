using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarAndTrackSelectorController : MonoBehaviour
{
    public GameObject numberOfPlayerSelector;
    public GameObject trackSelector;
    public GameObject carSelector;
    public Text selectedTrackText;

    private void Start() {
        LoadNumberOfPlayerSelector();
    }

    public void LoadNumberOfPlayerSelector() {
        trackSelector.SetActive(false);
        carSelector.SetActive(false);
        numberOfPlayerSelector.SetActive(true);
    }

    public void LoadTrackSelector() {
        numberOfPlayerSelector.SetActive(false);
        carSelector.SetActive(false);
        trackSelector.SetActive(true);
    }

    public void LoadCarSelector() {
        numberOfPlayerSelector.SetActive(false);
        trackSelector.SetActive(false);
        carSelector.SetActive(true);
    }
    
    public void SetSlectedTrackText(string text) {
        selectedTrackText.text = "Selected Track: " + text;
    }
    
    public void LoadTrackScene() {
        SceneManager.LoadScene(GameManager.instance.trackList[GameManager.instance.trackSelectedIndex].sceneName);
    }

    public void SetTrackIndex(int index) {
        GameManager.instance.SetTrackSelectedIndex(index);
    }
}
