using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public UnityAction OnConneting;
    public NetworkMessages.NetworkDeviceType DeviceType;

    private ConcurrentDictionary<NetworkMessages.NetworkDeviceType, NetworkConnection> DeviceConnections =
        new ConcurrentDictionary<NetworkMessages.NetworkDeviceType, NetworkConnection>();

    // Start is called before the first frame update
    void Start()
    {
        
#if UNITY_WSA && !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#else
        Debug.unityLogger.logEnabled = true;
#endif
        
        if (DeviceType == NetworkMessages.NetworkDeviceType.Tablet)
        {
            StartClient();
        }

        if (DeviceType == NetworkMessages.NetworkDeviceType.Master)
        {
            StartServer();

            NetworkServer.RegisterHandler<NetworkMessages.RegisterDeviceType>((connection, type) =>
            {
                DeviceConnections[type.type] = connection;
            });
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        if (DeviceType == NetworkMessages.NetworkDeviceType.Hololens ||
            DeviceType == NetworkMessages.NetworkDeviceType.Tablet)
        {
            StartCoroutine(WaitForReconnect());
            IEnumerator WaitForReconnect()
            {
                yield return new WaitForSeconds(1f);
                Debug.Log("Reconnecting");
                StartClient();
            }
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        foreach (var valuePair in DeviceConnections)
        {
            if (valuePair.Value == conn)
            {
                DeviceConnections.TryRemove(valuePair.Key,out _);
            }
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Connected!");
        base.OnClientConnect(conn);
        NetworkClient.Send(new NetworkMessages.RegisterDeviceType()
        {
            type = DeviceType
        });
        OnConneting?.Invoke();
    }

    private MasterManager _masterManager = null;

    public bool IsHololensConnected()
    {
        return DeviceConnections.ContainsKey(NetworkMessages.NetworkDeviceType.Hololens) &&
               DeviceConnections[NetworkMessages.NetworkDeviceType.Hololens] != null;
    }

    public NetworkConnection GetHololensConnection()
    {
        if (IsHololensConnected())
        {
            return DeviceConnections[NetworkMessages.NetworkDeviceType.Hololens];
        }

        return null;
    }

    public bool IsTabletConnected()
    {
        return DeviceConnections.ContainsKey(NetworkMessages.NetworkDeviceType.Tablet) &&
               DeviceConnections[NetworkMessages.NetworkDeviceType.Tablet] != null;
    }

    public NetworkConnection GetTabletConnection()
    {
        if (IsTabletConnected())
        {
            return DeviceConnections[NetworkMessages.NetworkDeviceType.Tablet];
        }

        return null;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        if (DeviceType == NetworkMessages.NetworkDeviceType.Hololens ||
            DeviceType == NetworkMessages.NetworkDeviceType.Tablet)
        {
            if (NetworkClient.connection != null)
            {
                NetworkClient.connection.Disconnect();
            }
            NetworkClient.Shutdown();
        }

        if (DeviceType == NetworkMessages.NetworkDeviceType.Master)
        {
            foreach (var pair in NetworkServer.connections)
            {
                pair.Value.Disconnect();
            }
            NetworkServer.DisconnectAllExternalConnections();
            NetworkServer.Shutdown();
        }
        Debug.Log("Cleaning");
    }
}