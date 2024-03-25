using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private float _timer;
    [SerializeField] private Pump pump;
    private string _content;
    private string _dataPath; 
    private string _fileName;
    private string _directoryPath;

    private void Awake() {
        _dataPath = Application.persistentDataPath;
        var dirArray = Directory.GetDirectories(_dataPath);
        List<string> directories = new List<string>();
        foreach (var directory in dirArray) {
            if(directory.Contains("Candidat_")) directories.Add(directory);
        }
        if (directories.Count == 0 ) {
            _directoryPath = _dataPath + Path.DirectorySeparatorChar + "Candidat_0";
            Directory.CreateDirectory(_directoryPath);
        }
        else {
            string lastDirectory = directories[^1];
            var files = Directory.GetFiles(lastDirectory);
            if (files.Length == 0 || !files[^1].Contains('9')) _directoryPath = lastDirectory;
            else {
                _directoryPath = _dataPath + Path.DirectorySeparatorChar + "Candidat_" + directories.Count;
                Directory.CreateDirectory(_directoryPath);
            }
        }
        _fileName = Directory.GetFiles(_directoryPath) == null ? "Data_0" : "Data_" + Directory.GetFiles(_directoryPath).Length + ".txt";
    }

    private void Start()
    {
        Balloon.Instance.Exploded += (sender, args) => SaveData(true);
        _content += "Argent accumulé : " + Money.DataManager.Instance.GetTotalMoney() + "€\n";
        _content += "------------------------------------\n";
        pump.PumpActivated += (sender, args) => AddData();
    }

    private void AddData() {
        _content += "Pompe numéro " + (Balloon.Instance.Index + 1) + " : " + _timer. + " secondes; \n";
        _timer = 0;
    }

    private void Update() {
        _timer += Time.deltaTime;
    }

    public void SaveData(bool exploded) {
        _content += "------------------------------------\n";
        _content += exploded ? "Le ballon a explosé." : "Le ballon n'a pas explosé.";
        string path = _directoryPath + Path.DirectorySeparatorChar + _fileName;
        if (!File.Exists(path)) {
            File.Create(path).Close();
        }
        File.WriteAllText(path, _content);
    }
}
