using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ResultManager : MonoBehaviour
{
    
    public class Result
    {
        public string Codename;
        public float SearchTaskTime;
        public float SelectTaskTime;
        public float SelectTaskTimeShort;
        public int Grabs = -77;
        public int Releases = -77;
        public int Hovers;
        public int AttributeSelections;
        public int WrongAttributeErrors;
        public bool CorrectConfirm;

        public override string ToString()
        {
            return Codename + ",\"" + SearchTaskTime + "\",\"" + SelectTaskTime + "\",\"" + SelectTaskTimeShort + "\"," + Grabs + "," + Releases + ","+ Hovers + ","+ AttributeSelections + "," + WrongAttributeErrors + "," +
                   CorrectConfirm;
        }
    }
    
    public UnityAction OnStart;
    public Coroutine _restingCoroutine;
    public string Codename = "";
    public bool IsIntroduction = false;
    
    private Cloud _cloud;
    private TargetItem _targetItem;
    private InteractionOrb _orb;

    private HololensManager _hololensManager;
    
    
    private Result _currentResult = null;
    private int _maxIterations = 0;
    private int _iterations = 0;
    private int _trialCount = 0;
    private float _maxTrialCount = 0f;
    private float _restingTime;

    public void Initialize(bool isIntroduction, int iterations, int trialCount, float restingTime)
    {
        _hololensManager.MainText.text = "";
        
        _cloud = FindObjectOfType<Cloud>();
        _targetItem = FindObjectOfType<TargetItem>();
        _orb = FindObjectOfType<InteractionOrb>();

        _orb.OnInteraction += OnInteraction;
        _orb.OnGrab += OnGrab;
        _orb.OnRelease += OnRelease;
        IsIntroduction = isIntroduction;
        _maxIterations = iterations;
        _iterations = 0;
        _maxTrialCount = trialCount;
        _trialCount = 0;
        _restingTime = restingTime;
        
        OnStart?.Invoke();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _hololensManager = FindObjectOfType<HololensManager>();
        
        RadialMenuItem.OnConfirm += OnConfirm;
        RadialMenuItem.OnSelect += OnSelect;
        RadialMenuItem.OnHover += OnHover;

        _hololensManager.TriggerMenu += () =>
        {
            if (_currentResult == null)
                return;
            
            if (_currentResult.SelectTaskTime == 0)
            {
                _currentResult.SearchTaskTime = Time.time - _currentResult.SearchTaskTime;
                _currentResult.SelectTaskTime = Time.time;
                
                PrintLog("Triggered Menu");
            }
            PrintLog("Triggered Menu invalid");
        };
        
        OnStart += () =>
        {
            _currentResult = new Result()
            {
                Codename = Codename,
                Hovers = 0,
                AttributeSelections = 0,
                WrongAttributeErrors = 0,
                SearchTaskTime = Time.time,
                SelectTaskTime = 0,
                SelectTaskTimeShort = 0,
            };

            if (_orb.IsPhysicalMenu)
            {
                _currentResult.Grabs = 0;
                _currentResult.Releases = 0;
            }

            var camPos = Camera.main.transform.position;
            camPos.y = _cloud.transform.position.y;
            _cloud.transform.LookAt(camPos);

            PrintLog("Start Trial: "+ (_orb.IsPhysicalMenu ? "grab" : "touch") + " "+ (_orb.HasOcclusion ? "occluded" : "not occluded"));
            
        };
    }

    private void OnHover(RadialMenuItem item)
    {
        _currentResult.Hovers++;
        PrintLog("Hover: "+item.gameObject.name);
    }

    private void OnInteraction()
    {
        if (_currentResult.SelectTaskTimeShort == 0)
        {
            _currentResult.SelectTaskTimeShort = Time.time;
            PrintLog("First Interaction");
        }
        else
        {
            PrintLog("Interaction");
        }
        
    }

    public void OnGrab()
    {
        _currentResult.Grabs++;
        PrintLog("Grab");
    }
    
    private void OnRelease()
    {
        _currentResult.Releases++;
        PrintLog("Release");
    }
    
    private void OnConfirm()
    {
        PrintLog("Confirm");
        
        _currentResult.SelectTaskTime = Time.time - _currentResult.SelectTaskTime;
        _currentResult.SelectTaskTimeShort = Time.time - _currentResult.SelectTaskTimeShort;
        _currentResult.CorrectConfirm = _cloud.ColourType.Color == _targetItem.ColourType.Color &&
                                   _cloud.ShapeType.MeshGameObjectName == _targetItem.ShapeType.MeshGameObjectName &&
                                   _cloud.TextureType.MaterialGameObjectName == _targetItem.TextureType.MaterialGameObjectName;

        if (!IsIntroduction)
        {
            PrintResult(_currentResult);
        }

        NetworkClient.Send(new NetworkMessages.TrialInformation()
        {
            TrialCount = _trialCount
        });
        
        _trialCount++;
        if (_trialCount < _maxTrialCount)
        {
            OnStart?.Invoke();
        }
        else
        {
            _trialCount = 0;
            _iterations++;
            if (_iterations < _maxIterations || IsIntroduction)
            {
                _restingCoroutine = StartCoroutine(RestingTime());
                IEnumerator RestingTime()
                {
                    _orb.GetComponent<Collider>().enabled = false;

                    _hololensManager.MainText.text = "Sie können sich für " + _restingTime + " Sekunden entspannen.";
                    yield return new WaitForSeconds(_restingTime-5f);
                    _hololensManager.MainText.text = "Es geht weiter in: 5 Sekunden";
                    yield return new WaitForSeconds(1f);
                    _hololensManager.MainText.text = "Es geht weiter in: 4 Sekunden";
                    yield return new WaitForSeconds(1f);
                    _hololensManager.MainText.text = "Es geht weiter in: 3 Sekunden";
                    yield return new WaitForSeconds(1f);
                    _hololensManager.MainText.text = "Es geht weiter in: 2 Sekunden";
                    yield return new WaitForSeconds(1f);
                    _hololensManager.MainText.text = "Es geht weiter in: 1 Sekunden";
                    yield return new WaitForSeconds(1f);
                    _hololensManager.MainText.text = "";
                    _restingCoroutine = null;
                    if (_orb != null)
                    {
                        _orb.GetComponent<Collider>().enabled = true;
                        OnStart?.Invoke();
                    }
                }
            }
            else
            {
                _hololensManager.MainText.text = "Durchlauf beendet.\nBitte nehmen Sie die Brille ab und begeben sich wieder zum Tablet.";
                NetworkClient.Send(new NetworkMessages.Questionnaire()
                {
                    Type = _hololensManager.Type
                });
                Destroy(_orb.gameObject);
                Destroy(_cloud.gameObject);
                Destroy(_targetItem.gameObject);
            }
        }
    }

    private void OnSelect(RadialMenuItemMetadata.IItemType type)
    {
        _currentResult.AttributeSelections++;
        
        switch (type)
        {
            case RadialMenuItemMetadata.ColourType t:
            {
                PrintLog("Selected Type: Colour " +((RadialMenuItemMetadata.ColourType) type).Color);
                if (_cloud.ColourType.Color != ((RadialMenuItemMetadata.ColourType) type).Color)
                {
                    _currentResult.WrongAttributeErrors++;
                }
                break;
            }
            case RadialMenuItemMetadata.ShapeType t:
            {
                PrintLog("Selected Type: Shape " +((RadialMenuItemMetadata.ShapeType) type).MeshGameObjectName);
                if (_cloud.ShapeType.MeshGameObjectName != ((RadialMenuItemMetadata.ShapeType) type).MeshGameObjectName)
                {
                    _currentResult.WrongAttributeErrors++;
                }
                break;
            }
            case RadialMenuItemMetadata.TextureType t:
            {
                PrintLog("Selected Type: Texture " +((RadialMenuItemMetadata.TextureType) type).MaterialGameObjectName);
                if (_cloud.TextureType.MaterialGameObjectName != ((RadialMenuItemMetadata.TextureType) type).MaterialGameObjectName)
                {
                    _currentResult.WrongAttributeErrors++;
                }
                break;
            }
        }
    }

    private void PrintResult(Result result)
    {
        if (IsIntroduction)
        {
            return;
        }
        
        var scenario = "";
        if (_orb.IsPhysicalMenu)
        {
            scenario = _orb.HasOcclusion ? "A" : "B";
        }
        else
        {
            scenario = _orb.HasOcclusion ? "C" : "D";
        }
        
        var filename = result.Codename + "_" + scenario + "_" + DateTime.Now.Day +
                       "_" + DateTime.Now.Month + "_" + DateTime.Now.Year+".csv";
#if UNITY_EDITOR
        var platformDependendPath = Application.dataPath;
#else
        var platformDependendPath = Application.persistentDataPath;
#endif
        if (!Directory.Exists(Path.Combine(platformDependendPath, "Results")))
        {
            Directory.CreateDirectory(Path.Combine(platformDependendPath, "Results"));
        }
        
        var path = Path.Combine(platformDependendPath, "Results",filename);

        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(result.ToString());
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(result.ToString());
            }
        }
    }

    private void PrintLog(string entry)
    {
        
        if (IsIntroduction)
            return;
        
        var scenario = "";
        if (_orb.IsPhysicalMenu)
        {
            scenario = _orb.HasOcclusion ? "A" : "B";
        }
        else
        {
            scenario = _orb.HasOcclusion ? "C" : "D";
        }
        
        var filename = _currentResult.Codename + "_" + scenario + "_" + DateTime.Now.Day +
                       "_" + DateTime.Now.Month + "_" + DateTime.Now.Year+".log";
        
#if UNITY_EDITOR
        var platformDependendPath = Application.dataPath;
#else
        var platformDependendPath = Application.persistentDataPath;
#endif
        
        if (!Directory.Exists(Path.Combine(platformDependendPath, "Logs")))
        {
            Directory.CreateDirectory(Path.Combine(platformDependendPath, "Logs"));
        }
        
        var path = Path.Combine(platformDependendPath, "Logs",filename);

        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssffff")+": "+entry);
            }
        }
        else
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssffff")+": "+entry);
            }
        }
    }
}
