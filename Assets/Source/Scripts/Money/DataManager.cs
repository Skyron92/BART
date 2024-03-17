using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static int _totalMoney;

    private static int _totalLeftovers;

    private static int m_totalLeftovers {
        get => _totalLeftovers;
        set {
            if (value >= 100) {
                _totalMoney = value / 100;
                _totalLeftovers = value % 100;
            }
            else _totalLeftovers = value;
        }
    }
        
    public static DataManager Instance;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        if(Instance == null) Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void AddTotalMoney(int euro) {
        _totalMoney += euro;
    }

    public void AddTotalMoney(int euro, int cent) {
        _totalMoney = euro;
        m_totalLeftovers = cent;
    }

    public int GetTotalMoney() {
        return _totalMoney;
    }

    public int GetTotalCent() {
        return m_totalLeftovers;
    }

}