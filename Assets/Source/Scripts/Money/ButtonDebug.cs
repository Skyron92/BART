using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonDebug : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.name);
    }

    public void DisplayActivator(SelectEnterEventArgs e) {
        Debug.Log(e.interactorObject.transform.name);
    }
}
