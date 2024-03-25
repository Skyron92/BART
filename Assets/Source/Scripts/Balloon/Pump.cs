﻿using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Manages the balloon inflation trigger
/// </summary>
public class Pump : MonoBehaviour {
    
    [SerializeField] private Balloon balloonReference;
    [SerializeField] private ParticleSystem moneyParticleSystem;
    [SerializeField] private AudioSource moneyAudioSource;

    public delegate void EventHandler(object sender, EventArgs e);

    public event EventHandler PumpActivated;

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
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Pump")) OnPumpActivated(this);
    }

    private void EmitParticles() {
        moneyParticleSystem.gameObject.SetActive(true);
        if(moneyParticleSystem.isPlaying) moneyParticleSystem.Stop();
        moneyParticleSystem.Play();
    }

    protected virtual void OnPumpActivated(object sender) {
        balloonReference.Blow();
        PumpActivated?.Invoke(sender, EventArgs.Empty);
    }
}