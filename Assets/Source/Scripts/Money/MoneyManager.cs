using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoneyManager : MonoBehaviour
{

    [SerializeField] private Pump pumpReference;

    [SerializeField] private TextMeshProUGUI moneyTMP;

    [SerializeField] private bool includesCent;

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

    private void Start() {
        pumpReference.PumpActivated += OnPumpActivated;
        Balloon.Instance.Exploded += (sender, args) => Restart(); 
        ResetValue();
        DisplayMoney();
    }

    private void ResetValue() {
        _euro = 0;
        _cent = 0;
    }

    private void OnPumpActivated(object sender, EventArgs e) {
        if (includesCent) _cent += _reward;
        else _euro++;
    }

    private void DisplayMoney() {
        Debug.Log("display");
        moneyTMP.text = includesCent ? _label + DataManagerInstance.GetTotalMoney() + "," + DataManagerInstance.GetTotalCent() + "€" 
            : _label + DataManagerInstance.GetTotalMoney() + "€";
    }

    public void CollectMoney() {
        DataManager.Instance.AddTotalMoney(_euro);
        Debug.Log(DataManagerInstance.GetTotalMoney());
        Collected?.Invoke(this, EventArgs.Empty);
        DisplayMoney();
        Invoke("Restart", 3f);
    }

    private void Restart() {
        Debug.Log("Restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}