using UnityEngine;
using System.IO.Ports;

public class PortLister : MonoBehaviour
{
    void Start()
    {
        string[] ports = SerialPort.GetPortNames();
        foreach (string port in ports)
        {
            Debug.Log("Available Port: " + port);
        }
    }
}
