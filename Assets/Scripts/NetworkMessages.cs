using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkMessages : MonoBehaviour
{
    public struct CloseScene : NetworkMessage
    {
        
    };
    
    public struct StartTrial : NetworkMessage
    {
        public bool IsOcclusionEnabled;
        public bool IsPhysical;
        public bool IsIntroduction;
        public int Iterations;
        public int TrialCount;
        public float RestingTime;
        public string Codename;
        public int Type;
    };

    public struct ConfirmCodename : NetworkMessage
    {
        public string Codename;
    }

    public struct Reset : NetworkMessage
    {
        
    }

    public enum NetworkDeviceType
    {
        Master,
        Hololens,
        Tablet
    }
    
    public struct RegisterDeviceType: NetworkMessage
    {
        public NetworkDeviceType type;
    }

    public struct Questionnaire : NetworkMessage
    {
        public int Type;
    }
}
