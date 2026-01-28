using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class DWM1001Listener : MonoBehaviour
{
    UdpClient udp;
    public int listenPort = 5005;

    void Start()
    {
        udp = new UdpClient()
    }

    void Update()
    {
        Debug.Log(udp.Available);
        if (udp.Available > 0)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udp.Receive(ref ep);
            string message = Encoding.UTF8.GetString(data);
            Debug.Log("UWB: " + message);
        }
    }

    void OnApplicationQuit()
    {
        udp.Close();
    }
}