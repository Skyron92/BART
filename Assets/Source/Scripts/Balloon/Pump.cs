using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Manages the balloon inflation trigger
/// </summary>
public class Pump : MonoBehaviour {
    
    [SerializeField] private Balloon balloonReference;
    [SerializeField] private ParticleSystem moneyParticleSystem;
    [SerializeField] private AudioSource moneyAudioSource;

    private bool _balloonIsExploded;

    private void Awake() {
        balloonReference.Exploded += OnExploded;
        balloonReference.BlowingEnded += OnBlowingEnded;
    }

    private void OnBlowingEnded(object sender, EventArgs e) {
        EmitParticles();
        moneyAudioSource.Play();
    }

    private void OnExploded(object sender, EventArgs e) {
        balloonReference.BlowingEnded -= OnBlowingEnded;
        Destroy(moneyParticleSystem.gameObject);
        _balloonIsExploded = true;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Pump")) Activate();
    }

    private void Activate() {
        balloonReference.Blow();
    }

    private void EmitParticles() {
        moneyParticleSystem.gameObject.SetActive(true);
        if(moneyParticleSystem.isPlaying) moneyParticleSystem.Stop();
        moneyParticleSystem.Play();
    }
}