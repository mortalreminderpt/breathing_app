using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;

public class TCPClient : MonoBehaviour
{
    [SerializeField] private string tcpIp = "127.0.0.1"; // 服务器 IP
    [SerializeField] private int tcpPort = 5555; // 服务器端口
    [SerializeField] private string componentScript = "BreathingDetector";
    [SerializeField] private List<GameObject> gameObjects = new List<GameObject>();

    private Socket clientSocket;
    private Thread receiveThread;
    private bool isRunning;
    private static TCPClient instance;

    void Start()
    {
        Connect();
        StartReceiveThread();
        SendMessageToServer("start");
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // 将当前对象设置为单例
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this.gameObject);
    }


    private void Connect()
    {
        try
        {
            Debug.Log($"发起连接：{tcpIp}:{tcpPort}");
            // 创建 Socket
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 连接指定的 IP 和端口
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(tcpIp), tcpPort));
            isRunning = true;
            Debug.Log($"已连接到服务器：{tcpIp}:{tcpPort}");
        }
        catch (Exception e)
        {
            Debug.LogError($"连接服务器失败: {e.Message}");
        }
    }


    private void StartReceiveThread()
    {
        if (clientSocket == null)
            return;

        receiveThread = new Thread(ReceiveMessages);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }


    private void ReceiveMessages()
    {
        while (isRunning)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = clientSocket.Receive(buffer);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    float? average = ComputeAverage(message);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        if (average.HasValue)
                        {
                            foreach (GameObject obj in gameObjects)
                            {
                                var detector = obj.GetComponent<BreathingDetector>();
                                if (detector != null)
                                {
                                    detector.Distance = average.Value;
                                }
                                else
                                {
                                    Debug.LogWarning($"GameObject {obj.name} 没有找到 BreathingDetector 组件。");
                                }
                            }
                            Debug.Log("平均值: " + average.Value);
                        }
                        else
                        {
                            Debug.Log($"无法解析: {message}");
                        }
                        Debug.Log($"收到来自服务器的数据: {message}");
                    });

                    // if (average.HasValue)
                    // {
                    //     // UnityMain
                    //     
                    //     foreach (GameObject obj in gameObjects)
                    //     {
                    //         obj.GetComponent<BreathingDetector>().Distance = average.Value;
                    //     }
                    //     Debug.Log("平均值: " + average.Value);
                    // }
                    // else
                    // {
                    //     Debug.Log($"无法解析: {message}");
                    // }
                    // Debug.Log($"收到来自服务器的数据: {message}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"接收数据时发生错误: {e.Message}");
                // 如果需要可在此处设置 isRunning = false，结束线程循环
            }
        }
    }


    public void SendMessageToServer(string message)
    {
        if (clientSocket != null && clientSocket.Connected)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                clientSocket.Send(data);
                Debug.Log($"发送消息: {message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"发送数据时发生错误: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("当前未连接到服务器，无法发送消息。");
        }
    }


    private void OnApplicationQuit()
    {
        isRunning = false;

        // 关闭接收线程
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort(); // 注意：Abort() 可能会带来一些问题，生产环境可用更优雅的方式结束线程
        }

        // 关闭 Socket
        if (clientSocket != null)
        {
            if (clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
            }

            clientSocket.Close();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindObjects();
    }

    private void FindObjects()
    {
        gameObjects = new List<GameObject>();
        if (componentScript == null)
        {
            Debug.LogWarning("请在 Inspector 面板中指定一个 MonoScript 对象！");
            return;
        }

        Type componentType = Type.GetType(componentScript);
        if (componentType == null)
        {
            Debug.LogWarning("获取类型失败，请确认拖拽的脚本是否为可用的 Component 类型。");
            return;
        }

        // 2. 在场景中查找所有该类型的对象
        Component[] foundComponents = FindObjectsOfType(componentType) as Component[];
        if (foundComponents == null || foundComponents.Length == 0)
        {
            Debug.Log("场景中没有找到此类型的任何组件");
            return;
        }

        foreach (var comp in foundComponents)
        {
            if (comp != null)
            {
                gameObjects.Add(comp.gameObject);
            }
        }
    }
    
    
    public float? ComputeAverage(string json)
    {
        try
        {
            // 反序列化JSON到RootObject
            RootObject root = JsonConvert.DeserializeObject<RootObject>(json);
            if (root == null)
                return null;

            // 检查returnCode是否为0
            if (root.returnCode != 0 || root.returnData == null)
                return null;

            float sum = 0f;
            int count = 0;

            // 遍历returnData中的所有键
            foreach (KeyValuePair<string, List<List<float>>> entry in root.returnData)
            {
                List<List<float>> dataList = entry.Value;
                foreach (List<float> subArray in dataList)
                {
                    if (subArray.Count < 3)
                        return null; // 子数组元素不足

                    sum += subArray[2];
                    count++;
                }
            }

            if (count == 0)
                return null;

            // 计算平均值
            float average = sum / count;
            return average;
        }
        catch (Exception ex)
        {
            Debug.LogError("解析JSON时发生错误: " + ex.Message);
            return null;
        }
    }

    [Serializable]
    public class RootObject
    {
        public int returnCode;
        public Dictionary<string, List<List<float>>> returnData;
    }
}