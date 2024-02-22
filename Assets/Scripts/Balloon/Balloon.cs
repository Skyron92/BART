using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the balloon behaviour
/// </summary>
public class Balloon : MonoBehaviour {
    
    [Header("Inflation")]
    // The inflation speed of the balloon
    [SerializeField, Range(0, 1)] private float speed;

    // The size that makes the balloon explode once reached
    private float _maxSize;
    // The minimal value for maxSize
    [SerializeField, Range(1f, 5f)] private float minimalMaxSize;
    //The maximal value for maxSize
    [SerializeField, Range(1f, 5f)] private float maximalMaxSize;
    // The current size of the balloon
    private float CurrentSize => transform.localScale.magnitude;

    // The initial duration of one inflation
    private const float InitialInflationDuration = 1f;

    // The current duration of one inflation
    private float _currentInflationDuration;
    
    // The current value of the timer
    private float _timer;

    // The animation curve of the balloon inflation
    [SerializeField] private AnimationCurve fillCurve;
    
    // True if the balloon is currently inflating
    private bool _isInflating;
    
    [Header("Assets")]
    // Contains the prefab instantiated when the balloon explodes
    [SerializeField] private GameObject explosionPrefab;

    // All the inflation SFX
    [SerializeField] private List<AudioClip> audioClips;
    private AudioSource _audioSource;

    // The path where all magnitudes are listed
    private string _sizeListPath = "Assets/Scripts/Balloon/SizeList.txt";
    // The index of the inflation is the number of inflation executed
    private int _index;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
        _maxSize = Random.Range(minimalMaxSize, maximalMaxSize);
        _currentInflationDuration = InitialInflationDuration;
        _index = 0;
        if (File.Exists(_sizeListPath)) File.Delete(_sizeListPath);
    }

    void Update() {
        if (Input.GetButtonDown("Fire1")) Blow();
        if(_isInflating) ScaleUp();
    }

    /// <summary>
    /// Begin the balloon inflation
    /// </summary>
    private void Blow() {
        _isInflating = true;
        _audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
        _audioSource.Play();
    }

    /// <summary>
    /// Balloon inflation
    /// </summary>
    private void ScaleUp() {
        // Increase timer
        _timer += Time.deltaTime;
       
        // Stop the inflation
        if (_timer >= _currentInflationDuration) {
            _isInflating = false;
            _index++;
            _currentInflationDuration += CurrentSize >= 1f ? InitialInflationDuration+ CurrentSize / 10f : InitialInflationDuration + 0.1f;
            // Generate a file with the magnitudes list of the balloon at each state
            #if UNITY_EDITOR
            string content = "";
            if (File.Exists(_sizeListPath)){
                content += File.ReadAllText(_sizeListPath);
                File.Delete(_sizeListPath);
                File.Create(_sizeListPath).Close();
            }
            File.WriteAllText(_sizeListPath,content + "\n"+ _index + " : " + CurrentSize + "\n");
            #endif
            return;
        }
        
        // Inflation
        transform.localScale = Vector3.Lerp(transform.localScale, GetTargetSize(), speed);  
       
        // Check if the balloon must explode
        if(HasReachedLimit()) Explode();
    }

    /// <summary>
    /// Calculates the next size of the balloon using the animation curve
    /// </summary>
    /// <returns>The next size of the balloon inflating</returns>
    Vector3 GetTargetSize() {
        float value = fillCurve.Evaluate(_timer);
        return transform.localScale.y >= transform.localScale.x ? new Vector3(value, value, value) 
            : new Vector3(transform.localScale.x, value, transform.localScale.z);
    }

    /// <summary>
    /// Check if the balloon must explode
    /// </summary>
    /// <returns>True if the balloon must explode</returns>
    private bool HasReachedLimit() {
        return CurrentSize >= _maxSize;
    }

    /// <summary>
    /// Begin the balloon explosion
    /// </summary>
    private void Explode() {
        var explosionInstance = Instantiate(explosionPrefab, transform.position, quaternion.identity);
        Destroy(explosionInstance,3);
        Destroy(gameObject);
    }
}
