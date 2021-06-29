using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicroKnights.IO.Streams;
using Mirror;
using Newtonsoft.Json;
using SharpAdbClient;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class MasterManager : MonoBehaviour
{
    public string ADBPath = "C:\\Program Files\\Unity\\Hub\\Editor\\2019.4.28f1\\Editor\\Data\\PlaybackEngines\\AndroidPlayer\\SDK\\platform-tools";
    
    public TMP_Text MainText;
    public Toggle IsHLConnectedToggle;
    public Toggle IsTabletConnectedToggle;
    public Toggle IsCodenameSetToggle;
    public Toggle RecordToggle;
    public Toggle RecordOnDevice;
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
    private int _setup = -1;

    private Stream _inputStream;
    private ReadableSplitStream _inputSplitStream;
    private Stream _fileInputStream;
    private FileStream _fileStream;
    private Stream _ffplayInputStream;
    private Process _ffplayProcess;
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
        
        NetworkServer.RegisterHandler<NetworkMessages.TrialInformation>(info =>
        {
            MainText.text = "Trials finished: " + info.TrialCount;
        });
        
        NetworkServer.RegisterHandler<NetworkMessages.Questionnaire>(questionnaire =>
        {
            if (questionnaire.Type == -1)
            {
                MainText.text = "Questionnaire ausgefüllt.";
                _networkManager.GetHololensConnection().Send(questionnaire);
            }
            else
            {
                StopStreams();

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

#if UNITY_STANDALONE || UNITY_EDITOR
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
#if UNITY_STANDALONE || UNITY_EDITOR
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
#if UNITY_STANDALONE || UNITY_EDITOR
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
            StopStreams();
            if (RecordToggle.isOn && RecordOnDevice.isOn && !IsIntroduction)
            {
                
                try
                {
                    _http.PostAsync(
                        "http://" + HololensIPInput.text.Trim() +
                        "/API/Holographic/MRC/Video/Control/Start?holo=true&pv=true&mic=false&loopback=false&RenderFromCamera=true&vstab=true", null);
                }
                catch (Exception e)
                {
                    Debug.Log("Could not start recording");
                    Debug.LogError(e);
                }
            }
            else
            {
                _inputStream = await _http.GetStreamAsync("http://" + HololensIPInput.text.Trim() + "/API/Holographic/Stream/live.mp4?MIC=false&Loopback=false&vstab=true");
                _inputSplitStream = new ReadableSplitStream(_inputStream);

                _cancellationTokenSource = new CancellationTokenSource();
            
                if (RecordToggle.isOn)
                {
#if UNITY_EDITOR
                    _fileStream = File.Create(Path.Combine(Application.dataPath,"..",_codename + "_" + setup + ".mp4"));
#else
                    _fileStream = File.Create(Path.Combine(Application.persistentDataPath,_codename + "_" + setup + ".mp4"));
#endif
                
                    _fileStream.Position = 0;
                }

                if (RecordToggle.isOn && !IsIntroduction)
                {
                    _fileInputStream = _inputSplitStream.GetForwardReadOnlyStream();
                    _fileInputStream.CopyToAsync(_fileStream,81920,_cancellationTokenSource.Token);
                }

                ProcessStartInfo info = new ProcessStartInfo()
                {
#if UNITY_EDITOR
                    FileName = Path.Combine(Application.dataPath, "..", "ffmpeg", "ffplay.exe"),
#else
                    FileName = Path.Combine(Path.GetFullPath("."), "ffmpeg", "ffplay.exe"),
#endif
                
                    Arguments =
                        "-i -",
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
            
                _ffplayProcess = Process.Start(info);
                _ffplayInputStream = _inputSplitStream.GetForwardReadOnlyStream();
                _ffplayInputStream.CopyToAsync(_ffplayProcess.StandardInput.BaseStream, 81920, _cancellationTokenSource.Token);

                _inputSplitStream.StartReadAhead();
            }
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
        _setup = 1;
        StartTrial(true,1);
    }

    public void StartOccGrab()
    {
        _setup = 1;
        StartTrial(false,1);
    }

    public void IntroductionNoOccGrab()
    {
        _setup = 2;
        StartTrial(true,2);
    }

    public void StartNoOccGrab()
    {
        _setup = 2;
        StartTrial(false,2);
    }

    public void IntroductionOccTouch()
    {
        _setup = 3;
        StartTrial(true,3);
    }

    public void StartOccTouch()
    {
        _setup = 3;
        StartTrial(false,3);
    }

    public void IntroductionNoOccTouch()
    {
        _setup = 4;
        StartTrial(true,4);
    }

    public void StartNoOccTouch()
    {
        _setup = 4;
        StartTrial(false,4);
    }

    public void CloseScene()
    {
        _networkManager.GetHololensConnection().Send(new NetworkMessages.CloseScene());
        StopStreams();
        MainText.text = "";
    }

    public void StartingQuestionnaire()
    {
        _networkManager.GetTabletConnection().Send(new NetworkMessages.Questionnaire()
        {
            Codename = _codename,
            Type = 0
        });
        MainText.text = "Starting Questionaire";
    }

    public void FinalQuestionnaire()
    {
        _networkManager.GetTabletConnection().Send(new NetworkMessages.Questionnaire()
        {
            Codename = _codename,
            Type = 5
        });
        MainText.text = "Final Questionaire";
    }

    public async void OpenTablet()
    {
        var client = new AdbClient();
        var device = client.GetDevices().FirstOrDefault(data => data.Model == "SM_T720");
        if (device == null)
        {
            Debug.LogError("Tablet not found");
            return;
        }
        
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

    private void StopStreams()
    {
        if (RecordOnDevice.isOn)
        {
            try
            {
                var result = _http.PostAsync("http://" + HololensIPInput.text.Trim() +
                                             "/api/holographic/mrc/Video/Control/Stop", null).Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;
                    var response = JsonConvert.DeserializeObject<dynamic>(jsonString);

#if UNITY_EDITOR
                    _fileStream =
                        File.Create(Path.Combine(Application.dataPath, "..", _codename + "_" + _setup + ".mp4"));
#else
                _fileStream =
 File.Create(Path.Combine(Application.persistentDataPath,_codename + "_" + _setup + ".mp4"));
#endif
                    var filename = Convert.ToBase64String(Encoding.UTF8.GetBytes((string) response.VideoFileName));
                    var message = _http.GetAsync("http://" + HololensIPInput.text.Trim() +
                                                 "/api/holographic/mrc/file?filename=" + filename + "&op=stream",
                        HttpCompletionOption.ResponseContentRead).Result;
                    var stream = message.Content.ReadAsStreamAsync().Result;
                    stream.CopyTo(_fileStream);
                    stream.Flush();
                    if (_fileStream != null)
                        _fileStream.Flush();
                    stream.Close();
                    if (_fileStream != null)
                        _fileStream.Close();
                    _fileStream = null;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Could not stop recording:");
                Debug.LogError(e);
            }
            
            
        }
        else
        {
            if (_inputSplitStream != null && _networkManager.GetHololensConnection().address != "::ffff:192.168.178.71")
            {
                if (_ffplayProcess != null && !_ffplayProcess.HasExited)
                {
                    _ffplayProcess.Kill();
                }
                _ffplayProcess = null;
            
                try
                {
                    if (_inputStream != null)
                        _inputStream.Flush();
                    if (_ffplayInputStream != null)
                        _ffplayInputStream.Flush();
                    if (_fileInputStream != null)
                        _fileInputStream.Flush();
                    if (_fileStream != null)
                        _fileStream.Flush();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    if (_inputStream != null)
                        _inputStream.Close();
                    if (_ffplayInputStream != null)
                        _ffplayInputStream.Close();
                    if (_fileInputStream != null)
                        _fileInputStream.Close();
                    if (_fileStream != null)
                        _fileStream.Close();
                    _inputSplitStream.Dispose();
                }
                catch
                {
                    // ignored
                }

                try
                {
                    _cancellationTokenSource.Cancel();
                }
                catch
                {
                    // ignored
                }

                _inputStream = null;
                _ffplayInputStream = null;
                _fileInputStream = null;
                _fileStream = null;
                _inputSplitStream = null;
            }
        }
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

    private void OnDestroy()
    {
        StopStreams();
    }
}