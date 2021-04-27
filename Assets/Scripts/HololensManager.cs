using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class HololensManager : MonoBehaviour
{
    public TMP_Text MainText;
    public GameObject InteractionOrbPrefab;
    public GameObject InteractionOrbAnchor;
    public GameObject PreviewPrefab;
    public GameObject PreviewAnchor;
    public GameObject TargetPrefab;
    public GameObject TargetAnchor;

    public void Start()
    {
        FindObjectOfType<CustomNetworkManager>().OnConneting += () =>
        {
            NetworkClient.RegisterHandler<NetworkMessages.StartTrial>(trial =>
            {
                StartTrial(trial.IsIntroduction, trial.IsOcclusionEnabled, trial.IsPhysical, trial.Count,
                    trial.TrialTime, trial.RestingTime);
                FindObjectOfType<ResultManager>().Codename = trial.Codename;
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
}
