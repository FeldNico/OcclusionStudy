using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TabletManager : MonoBehaviour
{

    public Button Button;
    public TMP_InputField InputField;
    public TMP_Text Text;
    private string _codename;
    #if UNITY_ANDROID
    private WebViewObject _web;
    #endif

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
        _web = new GameObject("WebViewObject").AddComponent<WebViewObject>();
        _web.Init(
            cb: (msg) =>
            {
                Debug.Log(string.Format("CallFromJS[{0}]", msg));
            },
            err: (msg) => { Debug.Log(string.Format("CallOnError[{0}]", msg)); },
            
            started: (msg) =>
            {
                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
            },
            hooked: (msg) =>
            {
                Debug.Log(string.Format("CallOnHooked[{0}]", msg));
            },
            ld: (msg) =>
            {
                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
                
                if (!msg.Contains("act="))
                {
                    StartCoroutine(Wait());
                    IEnumerator Wait()
                    {
                        yield return new WaitForSeconds(1f);
                        _web.SetVisibility(false);
                        NetworkClient.Send(new NetworkMessages.Questionnaire()
                        {
                            Type = -1
                        });
                    }
                }
                
            },
            enableWKWebView: true,
            wkContentMode: 0);
        
        _web.SetMargins(0, 0, 0, 0);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        #endif
        
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
                Text.text = "Warten...";
                #if UNITY_ANDROID
                _web.SetVisibility(true);
                _web.LoadURL("https://www.unipark.de/uc/occlusion/?a="+questionnaire.Type+"&b="+_codename);
                #endif
            });
        };
    }
}
