using TMPro;
using UnityEngine;

public class KeyboardInputHandler : MonoBehaviour
{
    public TMP_InputField hostField;
    public TMP_InputField nonceField;
    public Master master;

    // Diese Methode verbinden wir mit dem Keyboard Submit Event
    public void OnKeyboardSubmit()
    {
        // Lies direkt aus dem InputField
        string hostText = hostField.text;
        string nonceText = nonceField.text;

        master.SetHost(hostText);
        master.SetNunce(nonceText);

        Debug.Log("Host set to: " + hostText);
        Debug.Log("Nonce set to: " + nonceText);
    }
}
