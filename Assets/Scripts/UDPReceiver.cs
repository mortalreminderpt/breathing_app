using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPReceiver : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;
    public int port = 12345; // 设置UDP接收的端口号
    public float breathingIntensity = 0f; // 接收的呼吸强度
    private bool isStartGame = false;
    public BreathingGameController BreathController;

    void Start()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.ASCII.GetString(data);
                breathingIntensity = float.Parse(message);
                if (breathingIntensity > 0 && !isStartGame)
                {
                    BreathController.StartGame();
                    isStartGame = true;
                }
                Debug.Log("Received breath intensity: " + breathingIntensity);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error receiving UDP data: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null)
            receiveThread.Abort();
        udpClient.Close();
    }
}