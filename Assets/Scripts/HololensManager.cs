using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Mirror;
using TMPro;
using UnityEngine;

public class HololensManager : MonoBehaviour, IMixedRealityGestureHandler<Vector3>
{
    public TMP_Text MainText;
    public GameObject InteractionOrbPrefab;
    public GameObject InteractionOrbAnchor;
    public GameObject PreviewPrefab;
    public GameObject PreviewAnchor;
    public GameObject TargetPrefab;
    public GameObject TargetAnchor;

    public string Type;
    
    public void Start()
    {

        FindObjectOfType<CustomNetworkManager>().OnConneting += () =>
        {
            NetworkClient.RegisterHandler<NetworkMessages.StartTrial>(trial =>
            {
                StartTrial(trial.IsIntroduction, trial.IsOcclusionEnabled, trial.IsPhysical, trial.Count,
                    trial.TrialTime, trial.RestingTime);
                FindObjectOfType<ResultManager>().Codename = trial.Codename;
                Type = trial.Type;
            });
            
            NetworkClient.RegisterHandler<NetworkMessages.ConfirmCodename>(codename =>
            {
                FindObjectOfType<ResultManager>().Codename = codename.Codename;
            });

            NetworkClient.RegisterHandler<NetworkMessages.CloseScene>(scene =>
            {
                var coroutine = FindObjectOfType<ResultManager>()._restingCoroutine;
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                MainText.text = "";

                for (int i = 0; i < InteractionOrbAnchor.transform.childCount; i++)
                {
                    DestroyImmediate(InteractionOrbAnchor.transform.GetChild(i).gameObject);
                }

                for (int i = 0; i < PreviewAnchor.transform.childCount; i++)
                {
                    DestroyImmediate(PreviewAnchor.transform.GetChild(i).gameObject);
                }

                for (int i = 0; i < TargetAnchor.transform.childCount; i++)
                {
                    DestroyImmediate(TargetAnchor.transform.GetChild(i).gameObject);
                }
            });
        };
    }

    public void StartTrial(bool isIntroduction, bool isOcclusion, bool isPhysical, int count, float trialTime, float restingTime)
    {
        for (int i = 0; i < InteractionOrbAnchor.transform.childCount; i++)
        {
            DestroyImmediate(InteractionOrbAnchor.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < PreviewAnchor.transform.childCount; i++)
        {
            DestroyImmediate(PreviewAnchor.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < TargetAnchor.transform.childCount; i++)
        {
            DestroyImmediate(TargetAnchor.transform.GetChild(i).gameObject);
        }

        var orb = Instantiate(InteractionOrbPrefab).GetComponent<InteractionOrb>();
        orb.transform.parent = InteractionOrbAnchor.transform;
        orb.transform.localPosition = Vector3.zero;
        orb.transform.localRotation = Quaternion.identity;
        orb.transform.localScale = InteractionOrbPrefab.transform.localScale;
        
        var preview = Instantiate(PreviewPrefab).GetComponent<PreviewItem>();
        preview.transform.parent = PreviewAnchor.transform;
        preview.transform.localPosition = Vector3.zero;
        preview.transform.localRotation = Quaternion.identity;
        preview.transform.localScale = PreviewPrefab.transform.localScale;
        
        var target = Instantiate(TargetPrefab).GetComponent<Target>();
        target.transform.parent = TargetAnchor.transform;
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        target.transform.localScale = TargetPrefab.transform.localScale;

        StartCoroutine(Wait());
        IEnumerator Wait()
        {
            yield return new WaitForEndOfFrame();
            orb.Initialize(isOcclusion,isPhysical);
            yield return new WaitForEndOfFrame();
            FindObjectOfType<ResultManager>().Initialize(isIntroduction,count,trialTime,restingTime);
        }
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        MainText.text = "Started "+ Time.time + " " + eventData.MixedRealityInputAction.Description;
    }

    public void OnGestureUpdated(InputEventData eventData)
    {
        MainText.text = "Updated "+ Time.time + " " + eventData.MixedRealityInputAction.Description;
    }

    public void OnGestureCompleted(InputEventData eventData)
    {
        MainText.text = "Completed "+ Time.time + " " + eventData.MixedRealityInputAction.Description;
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        MainText.text = "Canceled "+ Time.time + " " + eventData.MixedRealityInputAction.Description;
    }

    private void OnEnable()
    {
        CoreServices.InputSystem.RegisterHandler<IMixedRealityGestureHandler<Vector3>>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem.UnregisterHandler<IMixedRealityGestureHandler<Vector3>>(this);
    }

    public void OnGestureUpdated(InputEventData<Vector3> eventData)
    {
        MainText.text = "Updated "+ Time.time + " " + eventData.MixedRealityInputAction.Description;
    }

    public void OnGestureCompleted(InputEventData<Vector3> eventData)
    {
        MainText.text = "Completed "+ Time.time + " " + eventData.MixedRealityInputAction.Description;
    }
}
