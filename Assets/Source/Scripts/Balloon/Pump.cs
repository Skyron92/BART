using UnityEngine;

public class Pump : MonoBehaviour
{
    [SerializeField] private Balloon ballonReference;

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Pump")) Activate();
    }

    private void Activate() {
        ballonReference.Blow();
    }
}