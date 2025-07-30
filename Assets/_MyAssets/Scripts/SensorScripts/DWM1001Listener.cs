using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;

public class DWM1001Listener : MonoBehaviour
{
    SerialPort serialPort;
    Thread readThread;
    bool isRunning = false;

    public string portName = "COM3";  // Replace with your port
    public int baudRate = 115200;

    // Store all tags by ID
    public Dictionary<string, Vector3> tagPositions = new Dictionary<string, Vector3>();

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 100;
        serialPort.Open();

        // Start listener mode
        serialPort.WriteLine("les\n");

        isRunning = true;
        readThread = new Thread(ReadSerial);
        readThread.Start();
    }

    void ReadSerial()
    {
        while (isRunning)
        {
            try
            {
                string line = serialPort.ReadLine();
                ParseLocation(line);
            }
            catch { }
        }
    }

    void ParseLocation(string data)
    {
        // Example line: "DIST, ID:0x1234, x:1.23, y:2.34, z:0.45, q:90"
        if (data.StartsWith("DIST") || data.StartsWith("POS"))
        {
            string[] parts = data.Split(',');
            if (parts.Length >= 5)
            {
                string id = parts[1].Split(':')[1].Trim(); // ID:0x1234
                float x = float.Parse(parts[2].Split(':')[1]);
                float y = float.Parse(parts[3].Split(':')[1]);
                float z = float.Parse(parts[4].Split(':')[1]);

                Vector3 position = new Vector3(x, z, y); // Swap to Unity coords

                lock (tagPositions)
                {
                    if (tagPositions.ContainsKey(id))
                        tagPositions[id] = position;
                    else
                        tagPositions.Add(id, position);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (readThread != null) readThread.Abort();
        if (serialPort != null && serialPort.IsOpen) serialPort.Close();
    }

    void Update()
    {
        // Example: Print all tags
        lock (tagPositions)
        {
            foreach (var kvp in tagPositions)
            {
                Debug.Log($"Tag {kvp.Key}: {kvp.Value}");
            }
        }
    }
}
