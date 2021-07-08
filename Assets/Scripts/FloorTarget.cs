using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Vuforia;

public class FloorTarget : MonoBehaviour
{
    public GameObject Floor;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<HololensManager>().MainText.text =
            "Bitte schauen Sie auf den bunten Marker auf dem Boden des Raumes";

        GetComponent<DefaultTrackableEventHandler>().OnTargetFound.AddListener(() =>
        {
            Floor.transform.parent = null;
            FindObjectOfType<HololensManager>().MainText.text =
                "Bitte warten Sie bis der Versuchsleiter den Versuch startet.";
            FindObjectOfType<CustomNetworkManager>().StartClient();
            FindObjectOfType<VideoPlayer>().GetComponent<Renderer>().enabled = false;
            if (TrackerManager.Instance != null)
            {
                //Positional DeviceTracker
                if (TrackerManager.Instance.GetTracker<PositionalDeviceTracker>() != null)
                {
                    TrackerManager.Instance.GetTracker<PositionalDeviceTracker>().Stop();
                    TrackerManager.Instance.DeinitTracker<PositionalDeviceTracker>();
                }
                   

                if (TrackerManager.Instance.GetTracker<AreaTracker>() != null)
                {
                    TrackerManager.Instance.GetTracker<AreaTracker>().Stop();
                    TrackerManager.Instance.DeinitTracker<AreaTracker>();
                }
                    

                //Object Tracker
                if (TrackerManager.Instance.GetTracker<ObjectTracker>() != null)
                {
                    TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
                    TrackerManager.Instance.DeinitTracker<ObjectTracker>();
                }
                    
            }
            if (CameraDevice.Instance.IsActive()) {
                CameraDevice.Instance.Stop ();
                CameraDevice.Instance.Deinit ();
            }

            GetComponent<DefaultTrackableEventHandler>().enabled = false;
            GetComponent<ImageTargetBehaviour>().enabled = false;
            
            VuforiaBehaviour.Instance.enabled = false;
            
        });
    }
}