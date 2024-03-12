using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MoneyManager : MonoBehaviour
{

    [SerializeField] private Pump pumpReference;

    [SerializeField] private TextMeshProUGUI moneyTMP;

    [SerializeField] private bool includesCent;

    [SerializeField] private GameObject restartCanvas;

    private DataManager DataManagerInstance => DataManager.Instance;

    private string _label = "Total : ";

    private int _euro;
    private int _cent;

    private int Cent {
        get => _cent;
        set {
            if (value >= 100) {
                _cent = value % 100;
                _euro = value / 100;
            }
            else {
                _cent = value;
            }
        }
    }

    private int _reward = 5;

    public delegate void EventHandler(object sender, EventArgs e);

    public event EventHandler Collected;

    public static MoneyManager Instance;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        pumpReference.PumpActivated += OnPumpActivated;
        Balloon.Instance.Exploded += OnExploded;
        DontDestroyOnLoad(gameObject);
        ResetValue();
    }

    private void ResetValue() {
        _euro = 0;
        _cent = 0;
    }

    private void OnExploded(object sender, EventArgs e) {
        restartCanvas.SetActive(true);
        moneyTMP.text = "Le ballon a éclaté !";
    }

    private void OnPumpActivated(object sender, EventArgs e) {
        if (includesCent) _cent += _reward;
        else _euro++;
    }

    private void DisplayMoney() {
        moneyTMP.text = includesCent ? _label + DataManagerInstance.GetTotalMoney() + "," + DataManagerInstance.GetTotalCent() + "€" 
            : _label + DataManagerInstance.GetTotalMoney() + "€";
    }

    public void CollectMoney() {
        DataManager.Instance.AddTotalMoney(_euro);
        Collected?.Invoke(this, EventArgs.Empty);
        DisplayMoney();
    }
}