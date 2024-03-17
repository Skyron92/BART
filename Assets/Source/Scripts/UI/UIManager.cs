using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject uiController;
    [SerializeField] private GameObject gameController;

    private void ActivateUI(object sender, EventArgs e) {
        uiController.SetActive(true);
        gameController.SetActive(false);
    }

    private void Start() {
        uiController.SetActive(false);
    }
}
