using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
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
            return Name + "," + TrialTime + "," + TrialTimeShort + "," + Grabs + "," + Releases + "," + Errors + "," +
                   IsCorrect;
        }
    }
    
    public UnityAction OnStart;
    public Canvas ProbandNameCanvas;
    
    private Target _target;
    private PreviewItem _previewItem;
    private InteractionOrb _orb;

    private string _probandName = "";
    private Result _currentResult = null;

    // Start is called before the first frame update
    void Start()
    {
        _target = FindObjectOfType<Target>();
        _previewItem = FindObjectOfType<PreviewItem>();
        _orb = FindObjectOfType<InteractionOrb>();
        
        foreach (var child in _target.GetComponentsInChildren<Renderer>())
        {
            child.enabled = false;
        }

        _previewItem.GetComponent<Renderer>().enabled = false;
        foreach (var child in _previewItem.GetComponentsInChildren<Renderer>())
        {
            child.enabled = false;
        }

        _orb.GetComponent<Renderer>().enabled = false;
        _orb.GetComponent<Collider>().enabled = false;

        _orb.OnGrabStart += OnGrabStart;
        _orb.OnGrabEnd += OnGrabEnd;
        RadialMenuItem.OnConfirm += OnConfirm;
        RadialMenuItem.OnSelect += OnSelect;
        
        ProbandNameCanvas.GetComponentInChildren<MRTKTMPInputField>().onSubmit.AddListener(probandName =>
        {
            _probandName = probandName;
            ProbandNameCanvas.gameObject.SetActive(false);
            foreach (var child in _target.GetComponentsInChildren<Renderer>())
            {
                child.enabled = true;
            }
            _previewItem.GetComponent<Renderer>().enabled = true;
            _orb.GetComponent<Renderer>().enabled = true;
            _orb.GetComponent<Collider>().enabled = true;
            
            OnStart?.Invoke();
        });

        OnStart += () =>
        {
            _currentResult = new Result()
            {
                Name = _probandName,
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
                    _orb.transform.position = GameObject.Find("Shape").transform.position;
                }
                else
                {
                    GameObject.Find("Shape").GetComponent<TouchHandler>().OnTouchStarted(null);
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
        
        PrintResult(_currentResult);
        
        
        OnStart?.Invoke();
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
#if UNITY_EDITOR
        if (!Directory.Exists(Path.Combine(Application.dataPath, "Results")))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Results"));
        }

        var filename = result.Name + "_" + DateTime.Now.Day +
                       "_" + DateTime.Now.Month + "_" + DateTime.Now.Year+".csv";
        var path = Path.Combine(Application.dataPath, "Results",filename);
#else
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Results")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Results"));
        }

        var filename = result.Name + "_" + DateTime.Now.Day +
                       "_" + DateTime.Now.Month + "_" + DateTime.Now.Year+".csv";
        var path = Path.Combine(Application.persistentDataPath, "Results",filename);
#endif
       
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
