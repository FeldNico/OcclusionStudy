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
        public string Name;
        public float TrialTime;
        public float TrialTimeShort;
        public int Grabs;
        public int Releases;
        public int Errors;
        public bool IsCorrect;

        public override string ToString()
        {
            return Name + ",\"" + TrialTime + "\",\"" + TrialTimeShort + "\"," + Grabs + "," + Releases + "," + Errors + "," +
                   IsCorrect;
        }
    }
    
    public UnityAction OnStart;
    public Coroutine _restingCoroutine;
    public string Codename = "";
    public bool IsIntroduction = false;
    
    private Target _target;
    private PreviewItem _previewItem;
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
        
        _target = FindObjectOfType<Target>();
        _previewItem = FindObjectOfType<PreviewItem>();
        _orb = FindObjectOfType<InteractionOrb>();

        _orb.OnGrabStart += OnGrabStart;
        _orb.OnGrabEnd += OnGrabEnd;
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

        OnStart += () =>
        {
            _currentResult = new Result()
            {
                Name = Codename,
                Errors = 0,
                Grabs = 0,
                Releases = 0,
                TrialTime = Time.time,
                TrialTimeShort = 0,
            };

            #if UNITY_EDITOR
            StartCoroutine(Wait());
            IEnumerator Wait()
            {
                yield return new WaitForSeconds(1f);
                if (_orb.IsPhysicalMenu)
                {
                    _orb.OnManipulationStart(null);
                }
                else
                {
                    _orb.GetComponent<TouchHandler>().OnTouchStarted(null);
                }

                yield return new WaitForSeconds(2f);
                if (_orb.IsPhysicalMenu)
                {
                    _orb.transform.position = GameObject.Find("Confirm").transform.position;
                    yield return new WaitForSeconds(2f);
                    _orb.OnManipulationEnd(null);
                }
                else
                {
                    GameObject.Find("Confirm").GetComponent<TouchHandler>().OnTouchStarted(null);
                }
            }
            #endif
        };
    }

    private void OnGrabEnd()
    {
        _currentResult.Releases++;
    }

    private void OnGrabStart()
    {
        if (_currentResult.TrialTimeShort == 0)
        {
            _currentResult.TrialTimeShort = Time.time;
        }
        _currentResult.Grabs++;
    }

    private void OnConfirm()
    {
        _currentResult.TrialTime = Time.time - _currentResult.TrialTime;
        _currentResult.TrialTimeShort = Time.time - _currentResult.TrialTimeShort;
        _currentResult.IsCorrect = _target.ColourType.Color == _previewItem.ColourType.Color &&
                                   _target.ShapeType.MeshGameObjectName == _previewItem.ShapeType.MeshGameObjectName &&
                                   _target.TextureType.MaterialGameObjectName == _previewItem.TextureType.MaterialGameObjectName;

        if (!IsIntroduction)
        {
            PrintResult(_currentResult);
        }

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
                Destroy(_target.gameObject);
                Destroy(_previewItem.gameObject);
            }
        }
    }

    private void OnSelect(RadialMenuItemMetadata.IItemType type)
    {
        switch (type)
        {
            case RadialMenuItemMetadata.ColourType t:
            {
                if (_target.ColourType.Color != ((RadialMenuItemMetadata.ColourType) type).Color)
                {
                    _currentResult.Errors++;
                }
                break;
            }
            case RadialMenuItemMetadata.ShapeType t:
            {
                if (_target.ShapeType.MeshGameObjectName != ((RadialMenuItemMetadata.ShapeType) type).MeshGameObjectName)
                {
                    _currentResult.Errors++;
                }
                break;
            }
            case RadialMenuItemMetadata.TextureType t:
            {
                if (_target.TextureType.MaterialGameObjectName != ((RadialMenuItemMetadata.TextureType) type).MaterialGameObjectName)
                {
                    _currentResult.Errors++;
                }
                break;
            }
        }
    }

    private void PrintResult(Result result)
    {
        var scenario = "";
        if (_orb.IsPhysicalMenu)
        {
            scenario = _orb.HasOcclusion ? "A" : "B";
        }
        else
        {
            scenario = _orb.HasOcclusion ? "C" : "D";
        }
        
        var filename = result.Name + "_" + scenario + "_" + DateTime.Now.Day +
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
}
