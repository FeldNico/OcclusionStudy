using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TabletManager : MonoBehaviour
{

    public Button Button;
    public TMP_InputField InputField;
    public TMP_Text Text;
    private string _codename;

    public void SubmitCodename()
    {
        var codename = InputField.text;
        codename = codename.Replace(" ", "");
        if (codename.Length >= 4)
        {
            _codename = codename;
            NetworkClient.Send(new NetworkMessages.ConfirmCodename()
            {
                Codename = codename
            });
            InputField.gameObject.SetActive(false);
            Button.gameObject.SetActive(false);
            Text.text = "Warten...";
            InputField.text = "";
        }
        else
        {
            Text.text = "Codename ungültig.";
        }
    }

    private void Start()
    {
        Button.gameObject.SetActive(false);
        InputField.gameObject.SetActive(false);
        Text.text = "Warten...";

        #if UNITY_ANDROID
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        #endif

        Application.OpenURL("https://www.unipark.de/uc/occlusion/?a=0&b="+_codename);
        
        FindObjectOfType<CustomNetworkManager>().OnConneting += () =>
        {
            NetworkClient.RegisterHandler<NetworkMessages.ConfirmCodename>(codename =>
            {
                Button.gameObject.SetActive(true);
                InputField.gameObject.SetActive(true);
                Text.text = "Codename:";
            });
            NetworkClient.RegisterHandler<NetworkMessages.Questionnaire>(questionnaire =>
            {
                Text.text = "example";
                Process.Start("http://example.com");
            });
        };
    }
}
