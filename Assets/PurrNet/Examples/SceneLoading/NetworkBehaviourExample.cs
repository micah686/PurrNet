using System;
using System.Collections;
using JetBrains.Annotations;
using PurrNet;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkBehaviourExample : NetworkBehaviour
{
    [SerializeField] private NetworkIdentity someRef;

    [SerializeField]
    private SyncVar<int> _testChild; 

    [SerializeField]
    private SyncVar<int> _testChild2 = new (70);

    [SerializeField]
    private SyncVar<int> _testChild3;

    protected override void OnInitializeModules()
    {
        _testChild = new SyncVar<int>(69);
    }

    protected override void OnSpawned(bool asServer)
    {
        if (!asServer)
        {
            Test("Test");
        }
    }
    
    private void Update()
    {
        if (isSpawned && isServer)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _testChild.value = Random.Range(0, 100);
                _testChild2.value = Random.Range(0, 100);
                _testChild3.value = Random.Range(0, 100);
                // ObserversRPCTest(Time.time, someRef);
            }
        }
    }
    
    [ServerRPC(requireOwnership: false)]
    private void Test(string test)
    {
        Debug.Log(test);
    }

    [ObserversRPC(bufferLast: true)]
    private static void ObserversRPCTest<T>(T data, NetworkIdentity someNetRef, RPCInfo info = default)
    {
        Debug.Log("Observers: " + data + " " + info.sender);
        
        if (someNetRef)
            Debug.Log(someNetRef.name);
        else
            Debug.Log("No ref");
    }

    [TargetRPC(bufferLast: true)]
    private void SendToTarget<T>([UsedImplicitly] PlayerID target, T message)
    {
        Debug.Log("Targeted: " + message + " " + typeof(T).Name);
    }
}
