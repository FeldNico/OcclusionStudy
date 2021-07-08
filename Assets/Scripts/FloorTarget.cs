using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class FloorTarget : MonoBehaviour
{
    public GameObject Floor;
    
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<HololensManager>().MainText.text = "Bitte schauen Sie auf den bunten Marker auf dem Boden des Raumes";
        
        GetComponent<DefaultTrackableEventHandler>().OnTargetFound.AddListener(() =>
        {
            Floor.transform.parent = null;
            FindObjectOfType<HololensManager>().MainText.text =
                "Bitte warten Sie bis der Versuchsleiter den Versuch startet.";
            FindObjectOfType<CustomNetworkManager>().StartClient();
        });
    }
}
