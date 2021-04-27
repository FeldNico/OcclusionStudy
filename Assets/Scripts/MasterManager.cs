using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Mirror;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MasterManager : MonoBehaviour
{
    public TMP_Text MainText;
    public Toggle IsHLConnectedToggle;
    public Toggle IsTabletConnectedToggle;
    public Toggle IsCodenameSetToggle;
    public TMP_InputField HololensIPInput;
    public TMP_InputField IPDInput;
    public TMP_InputField CountInput;
    public TMP_InputField TrialTimeInput;
    public TMP_InputField RestingTimeInput;
    public GameObject SceneSelection;

    public UnityAction ReadyToTest;
    public UnityAction NotReadyToTest;

    private CustomNetworkManager _networkManager;
    private HttpClient _http;
    private NetworkCredential _credential = new NetworkCredential(string.Format("auto-{0}", "hololens"), "hololens");
    private dynamic _package = null;
    private string _codename = "";
    
    private void Start()
    {
        _networkManager = FindObjectOfType<CustomNetworkManager>();
        _http = new HttpClient(new HttpClientHandler {Credentials = _credential});
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes("hololens:hololens")));

        NetworkServer.RegisterHandler<NetworkMessages.ConfirmCodename>(codename =>
        {
            IsCodenameSetToggle.isOn = true;
            _codename = codename.Codename;
            //_networkManager.GetHololensConnection().Send(codename);
        });
        
        NetworkServer.RegisterHandler<NetworkMessages.Reset>(reset =>
        {
            MainText.text = "Versuch beendet.";
        });

        ReadyToTest += () =>
        {
            foreach (var child in SceneSelection.GetComponentsInChildren<Button>())
            {
                child.interactable = true;
            }
        };
        NotReadyToTest += () =>
        {
            foreach (var child in SceneSelection.GetComponentsInChildren<Button>())
            {
                child.interactable = false;
            }
        };
    }

#if UNITY_STANDALONE
    private async Task<dynamic> GetPackage()
    {
        
        if (_package == null)
        {
            Debug.Log("Get Packages");
            var packagesResponse =
                await _http.GetAsync("http://" + HololensIPInput.text.Trim() + "/API/APP/packagemanager/Packages");
            Debug.Log("Got response");
            var content = await packagesResponse.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(content);

            foreach (dynamic package in data.InstalledPackages)
            {
                if (package.Name == "OcclusionStudy")
                {
                    _package = package;
                    break;
                }
            }
        }
        

        return _package;
    }
#endif
    
    public async void StartHololensApp()
    {
#if UNITY_STANDALONE
        var package = await GetPackage();

        await KillHololensApp();
        
        Debug.Log("Send Post");
        var appEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes((string) package.PackageRelativeId));
        var response = await _http.PostAsync(
            "http://" + HololensIPInput.text.Trim() + "/api/taskmanager/app?appid=" + appEncoded, null);
        Debug.Log(response.StatusCode);
#endif
    }

    public void KillHololensAppWrapper()
    {
        KillHololensApp();
    }
    
    public async Task KillHololensApp()
    {
#if UNITY_STANDALONE
        var package = await GetPackage();
        
        Debug.Log("Send Delete");
        var packageEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes((string) package.PackageFullName));
        var response = await _http.DeleteAsync("http://" + HololensIPInput.text.Trim() +
                                               "/api/taskmanager/app?package=" + packageEncoded);
        Debug.Log(response.StatusCode);
#endif
    }

    public void SetIPD()
    {
        var ipd = Convert.ToInt32(IPDInput.text);
        _http.PostAsync("http://" + HololensIPInput.text.Trim() + "/API/Holographic/OS/Settings/IPD?ipd=" + ipd, null);
    }

    public void RequestCodename()
    {
        _networkManager.GetTabletConnection().Send(new NetworkMessages.ConfirmCodename());
    }

    public void StartTrial(bool IsIntroduction, string setup)
    {
        var msg = new NetworkMessages.StartTrial()
        {
            Count = Convert.ToInt32(CountInput.text),
            IsIntroduction = IsIntroduction,
            IsPhysical = setup == "A" || setup == "B",
            IsOcclusionEnabled = setup == "A" || setup == "C",
            RestingTime = Convert.ToSingle(RestingTimeInput.text),
            TrialTime = Convert.ToSingle(TrialTimeInput.text),
            Codename = _codename
        };
        _networkManager.GetHololensConnection().Send(msg);
        MainText.text = "";
    }
    
    public void IntroductionOccGrab()
    {
        StartTrial(true,"A");
    }

    public void StartOccGrab()
    {
        StartTrial(false,"A");
    }

    public void IntroductionNoOccGrab()
    {
        StartTrial(true,"B");
    }

    public void StartNoOccGrab()
    {
        StartTrial(false,"B");
    }

    public void IntroductionOccTouch()
    {
        StartTrial(true,"C");
    }

    public void StartOccTouch()
    {
        StartTrial(false,"C");
    }

    public void IntroductionNoOccTouch()
    {
        StartTrial(true,"D");
    }

    public void StartNoOccTouch()
    {
        StartTrial(false,"D");
    }

    public void CloseScene()
    {
        _networkManager.GetHololensConnection().Send(new NetworkMessages.CloseScene());
        MainText.text = "";
    }

    private bool _wasHLConnected = false;
    private bool _wasTabletConnected = false;
    private bool _wasReadyToStartTrial = false;

    public void Update()
    {
        if (_networkManager.IsHololensConnected() != _wasHLConnected)
        {
            if (_wasHLConnected)
            {
                IsHLConnectedToggle.isOn = false;
            }
            else
            {
                IsHLConnectedToggle.isOn = true;
            }

            _wasHLConnected = !_wasHLConnected;
        }

        if (_networkManager.IsTabletConnected() != _wasTabletConnected)
        {
            if (_wasTabletConnected)
            {
                IsTabletConnectedToggle.isOn = false;
            }
            else
            {
                IsTabletConnectedToggle.isOn = true;
            }

            _wasTabletConnected = !_wasTabletConnected;
        }

        if (IsTabletConnectedToggle.isOn && IsHLConnectedToggle.isOn && IsCodenameSetToggle.isOn && !_wasReadyToStartTrial)
        {
            _wasReadyToStartTrial = false;
            ReadyToTest?.Invoke();
        }
        if ((!IsTabletConnectedToggle.isOn || !IsHLConnectedToggle.isOn || IsCodenameSetToggle.isOn) && _wasReadyToStartTrial)
        {
            _wasReadyToStartTrial = true;
            NotReadyToTest?.Invoke();
        }
    }
}