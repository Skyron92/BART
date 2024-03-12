using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject uiController;
    [SerializeField] private GameObject gameController;
    [SerializeField] private GameObject restartCanvas;

    private void ActivateUI(object sender, EventArgs e) {
        uiController.SetActive(true);
        restartCanvas.SetActive(true);
        gameController.SetActive(false);
    }

    private void Start() {
        restartCanvas.SetActive(false);
        uiController.SetActive(false);
        Balloon.Instance.Exploded += ActivateUI;
        MoneyManager.Instance.Collected += ActivateUI;
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
