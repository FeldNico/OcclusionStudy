using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Utilities;
using Mirror;
using Newtonsoft.Json;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MasterManager : MonoBehaviour
{
    public string ADBPath = "C:\\Program Files\\Unity\\Hub\\Editor\\2019.4.22f1\\Editor\\Data\\PlaybackEngines\\AndroidPlayer\\SDK\\platform-tools";
    
    public TMP_Text MainText;
    public Toggle IsHLConnectedToggle;
    public Toggle IsTabletConnectedToggle;
    public Toggle IsCodenameSetToggle;
    public TMP_InputField HololensIPInput;
    public TMP_InputField IPDInput;
    public TMP_InputField IterationsCount;
    public TMP_InputField TrialCountInput;
    public TMP_InputField RestingTimeInput;
    public GameObject SceneSelection;

    public UnityAction ReadyToTest;
    public UnityAction NotReadyToTest;

    private CustomNetworkManager _networkManager;
    private HttpClient _http;
    private NetworkCredential _credential = new NetworkCredential(string.Format("auto-{0}", "hololens"), "hololens");
    private dynamic _package = null;
    private string _codename = "";

    private Stream _inputStream;
    private FileStream _fileStream;
    private CancellationTokenSource _cancellationTokenSource;
    private AdbServer _adbServer;
    
    private void Start()
    {
        _networkManager = FindObjectOfType<CustomNetworkManager>();
        _http = new HttpClient(new HttpClientHandler {Credentials = _credential});
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes("hololens:hololens")));

        _adbServer = new AdbServer();
        _adbServer.StartServer(Path.Combine(ADBPath, "adb.exe"),true);
        
        NetworkServer.RegisterHandler<NetworkMessages.ConfirmCodename>(codename =>
        {
            IsCodenameSetToggle.isOn = true;
            _codename = codename.Codename;
        });
        
        NetworkServer.RegisterHandler<NetworkMessages.Reset>(reset =>
        {
            MainText.text = "Versuch beendet.";
        });
        
        NetworkServer.RegisterHandler<NetworkMessages.Questionnaire>(questionnaire =>
        {
            if (questionnaire.Type == -1)
            {
                MainText.text = "Questionnaire ausgefüllt.";
            }
            else
            {
                if (_networkManager.GetHololensConnection().address != "::ffff:192.168.178.71")
                {
                    _inputStream.Flush();
                    _fileStream.Flush();
                    _inputStream.Close();
                    _fileStream.Close();
                    _cancellationTokenSource.Cancel();
                }

                MainText.text = "Questionnaire wird bearbeitet.";
                _networkManager.GetTabletConnection().Send(questionnaire);
            }
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

    public async void StartTrial(bool IsIntroduction, int setup)
    {
        if (_networkManager.GetHololensConnection().address != "::ffff:192.168.178.71")
        {
            _inputStream = await _http.GetStreamAsync("http://" + HololensIPInput.text.Trim() + "/API/Holographic/Stream/live.mp4?MIC=false&Loopback=false");

#if UNITY_EDITOR
            _fileStream = File.Create(Path.Combine(Application.dataPath,"..",_codename + "_" + setup + ".mp4"));
#else
        _fileStream = File.Create(Path.Combine(Application.persistentDataPath,_codename + "_" + setup + ".mp4"));
#endif

            _cancellationTokenSource = new CancellationTokenSource();
        
            _fileStream.Position = 0;
            _inputStream.CopyToAsync(_fileStream,81920,_cancellationTokenSource.Token);
        }
        
        var msg = new NetworkMessages.StartTrial()
        {
            Iterations = Convert.ToInt32(IterationsCount.text),
            IsIntroduction = IsIntroduction,
            IsPhysical = setup == 1 || setup == 2,
            IsOcclusionEnabled = setup == 1|| setup == 3,
            RestingTime = Convert.ToSingle(RestingTimeInput.text),
            TrialCount = Convert.ToInt32(TrialCountInput.text),
            Codename = _codename,
            Type = setup
        };
        _networkManager.GetHololensConnection().Send(msg);
        MainText.text = "Trial startet: "+setup;
    }
    
    public void IntroductionOccGrab()
    {
        StartTrial(true,1);
    }

    public void StartOccGrab()
    {
        StartTrial(false,1);
    }

    public void IntroductionNoOccGrab()
    {
        StartTrial(true,2);
    }

    public void StartNoOccGrab()
    {
        StartTrial(false,2);
    }

    public void IntroductionOccTouch()
    {
        StartTrial(true,3);
    }

    public void StartOccTouch()
    {
        StartTrial(false,3);
    }

    public void IntroductionNoOccTouch()
    {
        StartTrial(true,4);
    }

    public void StartNoOccTouch()
    {
        StartTrial(false,4);
    }

    public void CloseScene()
    {
        _networkManager.GetHololensConnection().Send(new NetworkMessages.CloseScene());
        MainText.text = "";
    }

    public void StartingQuestionnaire()
    {
        _networkManager.GetTabletConnection().Send(new NetworkMessages.Questionnaire()
        {
            Type = 0
        });
        MainText.text = "Starting Questionaire";
    }

    public void FinalQuestionnaire()
    {
        _networkManager.GetTabletConnection().Send(new NetworkMessages.Questionnaire()
        {
            Type = 5
        });
        MainText.text = "Final Questionaire";
    }

    public async void RestartTablet()
    {
        var client = new AdbClient();
        var device = client.GetDevices().FirstOrDefault(data => data.Model == "SM_T720");
        if (device == null)
        {
            Debug.LogError("Tablet not found");
            return;
        }

        await client.ExecuteRemoteCommandAsync("am force-stop com.DefaultCompany.OcclusionStudy", device,null,CancellationToken.None);
        await client.ExecuteRemoteCommandAsync("monkey -p com.DefaultCompany.OcclusionStudy -c android.intent.category.LAUNCHER 1", device,null,CancellationToken.None);
    }

    public async void KillTablet()
    {
        var client = new AdbClient();
        var device = client.GetDevices().FirstOrDefault(data => data.Model == "SM_T720");
        if (device == null)
        {
            Debug.LogError("Tablet not found");
            return;
        }
        await client.ExecuteRemoteCommandAsync("am force-stop com.DefaultCompany.OcclusionStudy", device,null,CancellationToken.None);
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