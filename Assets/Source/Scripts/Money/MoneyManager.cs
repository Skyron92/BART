using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Money
{
    public class MoneyManager : MonoBehaviour
    {

        [SerializeField] private Pump pumpReference;

        [SerializeField] private TextMeshProUGUI moneyTMP;

        [SerializeField] private bool includesCent;
        private bool _hasAlreadyCollected;

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
                else _cent = value;
            }
        }

        private int _reward = 5;
        

        private void Start() {
            pumpReference.PumpActivated += OnPumpActivated;
            ResetValue();
            DisplayMoney();
            Balloon.Instance.Exploded += (sender, args) => Invoke("Restart", 3f);
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
            moneyTMP.text = includesCent
                ? _label + DataManagerInstance.GetTotalMoney() + "," + DataManagerInstance.GetTotalCent() + "€"
                : _label + DataManagerInstance.GetTotalMoney() + "€";
        }

        public void CollectMoney() {
            if(_hasAlreadyCollected) return;
            _hasAlreadyCollected = true;
            DataManager.Instance.AddTotalMoney(_euro);
            DisplayMoney();
            Invoke("Restart", 3f);
        }

        private void Restart() {
            Debug.Log("Restart");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}