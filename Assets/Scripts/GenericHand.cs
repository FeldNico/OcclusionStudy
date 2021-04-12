using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHand : MonoBehaviour
{

    public float Scale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.one * Scale;
    }
}
