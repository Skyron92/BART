using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the balloon behaviour
/// </summary>
public class Balloon : MonoBehaviour
{
    [Header("Inflation")]
    // The balloon's transform
    [SerializeField, Tooltip("The balloon transform. If empty it will set to the game object this script is attached")] private Transform balloonTransform;
    
    // The inflation speed of the balloon
    [SerializeField, Range(.01f, 1), Tooltip("The inflation speed")] private float speed;

    // The size that makes the balloon explode once reached
    private float _maxSize;
    // The minimal value for maxSize
    [SerializeField, Range(1f, 5f), Tooltip("The minimum value the balloon can explode")] private float minimalMaxSize;
    //The maximal value for maxSize
    [SerializeField, Range(1f, 5f), Tooltip("The maximum value the balloon can explode")] private float maximalMaxSize;
    // The current size of the balloon
    private float CurrentSize => balloonTransform.localScale.magnitude;

    // The initial duration of one inflation
    private const float InitialInflationDuration = 1f;

    // The current duration of one inflation
    private float _currentInflationDuration;
    
    // The current value of the timer
    private float _timer;

    // The animation curve of the balloon inflation
    [SerializeField, Tooltip("The size curve of the balloon")] private AnimationCurve fillCurve;
    
    // True if the balloon is currently inflating
    private bool _isInflating;
    public  bool IsInflating => _isInflating;

    // The empty where the tail must be
    [SerializeField, Tooltip("The target position of the tail")] private Transform tailPositionTransform;
    // The tail model transform
    [SerializeField, Tooltip("The tail's transform")] private Transform tailTransform;
    
    [Header("Assets")]
    // Contains the prefab instantiated when the balloon explodes
    [SerializeField] private GameObject explosionPrefab;

    // All the inflation SFX
    [SerializeField] private List<AudioClip> audioClips;
    private AudioSource _audioSource;

    // The path where all magnitudes are listed
    private string _sizeListPath = "Assets/Source/Scripts/Balloon/SizeList.txt";
    // The index of the inflation is the number of inflation executed
    private int _index;
    
    public delegate void EventHandler(object sender, EventArgs e);

    public event EventHandler Exploded;
    public event EventHandler BlowingEnded;

    public static Balloon Instance;

    private void Awake() {
        Instance = this;
        if (balloonTransform == null) balloonTransform = transform;
        _audioSource = GetComponent<AudioSource>();
        _maxSize = Random.Range(minimalMaxSize, maximalMaxSize);
        _currentInflationDuration = InitialInflationDuration;
        _index = 0;
        if (File.Exists(_sizeListPath)) File.Delete(_sizeListPath);
    }

    void Update() {
        if(_isInflating) ScaleUp();
    }

    /// <summary>
    /// Begin the balloon inflation
    /// </summary>
    public void Blow() {
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
            BlowingEnded?.Invoke(this, EventArgs.Empty);
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
        balloonTransform.localScale = Vector3.Lerp(balloonTransform.localScale, GetTargetSize(), speed);
        // Set the tail at its new position
        tailTransform.position = tailPositionTransform.position;
       
        // Check if the balloon must explode
        if(HasReachedLimit()) Explode();
    }

    /// <summary>
    /// Calculates the next size of the balloon using the animation curve
    /// </summary>
    /// <returns>The next size of the balloon inflating</returns>
    Vector3 GetTargetSize() {
        float value = fillCurve.Evaluate(_timer);
        return balloonTransform.localScale.z >= balloonTransform.localScale.x ? new Vector3(value, value, value) 
            : new Vector3(balloonTransform.localScale.x, balloonTransform.transform.localScale.y, value);
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
        Exploded?.Invoke(this, EventArgs.Empty);
        var explosionInstance = Instantiate(explosionPrefab, transform.position, quaternion.identity);
        Destroy(explosionInstance,3);
        Destroy(gameObject);
    }
}