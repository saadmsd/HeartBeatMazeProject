using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Globalization;

public class UDPListener : MonoBehaviour
{
    private Thread udpThread;
    private UdpClient udpClient;
    public int listenPort = 5005;
    private bool isRunning = true;
    
    public float HR_1min { get; private set; }
    public float HRV_1min { get; private set; }
    public float HR_10s { get; private set; }
    public float HRV_10s { get; private set; }
    
    void Start()
    {
        udpThread = new Thread(new ThreadStart(ReceiveData));
        udpThread.IsBackground = true;
        udpThread.Start();
    }

    void ReceiveData()
    {
        udpClient = new UdpClient(listenPort);
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
        
        while (isRunning)
        {
            try
            {
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                string receivedText = Encoding.UTF8.GetString(receivedBytes).Trim();
                
                string[] values = receivedText.Split(',');
                if (values.Length == 4)
                {
                    HR_1min = float.Parse(values[0], CultureInfo.InvariantCulture);
                    HRV_1min = float.Parse(values[1], CultureInfo.InvariantCulture);
                    HR_10s = float.Parse(values[2], CultureInfo.InvariantCulture);
                    HRV_10s = float.Parse(values[3], CultureInfo.InvariantCulture);
                    
                    Debug.Log($"HR(1min): {HR_1min} BPM, HRV(1min): {HRV_1min} ms, HR(10s): {HR_10s} BPM, HRV(10s): {HRV_10s} ms");
                }
                else
                {
                    Debug.LogError("Données reçues incorrectes: " + receivedText);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Erreur UDP: " + e.Message);
            }
        }
    }
    
    void OnApplicationQuit()
    {
        isRunning = false;
        udpClient?.Close();
        udpThread?.Abort();
    }
}
