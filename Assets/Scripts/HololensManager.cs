using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Video;

public class HololensManager : MonoBehaviour
{

    public VideoClip Video1;
    public VideoClip Video2;
    public VideoClip Video3;
    public VideoClip Video4;
    
    public AudioClip HoverSound;
    public AudioClip SelectSound;
    
    public TMP_Text MainText;
    public GameObject InteractionOrbPrefab;
    public GameObject InteractionOrbAnchor;
    public GameObject TargetPrefab;
    public GameObject TargetAnchor;
    public GameObject CloudPrefab;
    public GameObject CloudAnchor;

    public int Type;

    private Camera _camera;
    private InteractionOrb _orb;

    public void Start()
    {
        _camera = Camera.main;

        PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
        PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.AlwaysOff);

        FindObjectOfType<CustomNetworkManager>().OnConneting += () =>
        {
            NetworkClient.RegisterHandler<NetworkMessages.StartTrial>(trial =>
            {
                
                StartTrial(trial.IsIntroduction, trial.IsOcclusionEnabled, trial.IsPhysical, trial.Iterations,
                    trial.TrialCount, trial.RestingTime);
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

                for (int i = 0; i < TargetAnchor.transform.childCount; i++)
                {
                    DestroyImmediate(TargetAnchor.transform.GetChild(i).gameObject);
                }

                for (int i = 0; i < CloudAnchor.transform.childCount; i++)
                {
                    DestroyImmediate(CloudAnchor.transform.GetChild(i).gameObject);
                }
                FindObjectOfType<VideoPlayer>().gameObject.SetActive(false);
            });
        };
    }

    public void StartTrial(bool isIntroduction, bool isOcclusion, bool isPhysical, int iterations, int trialCount, float restingTime)
    {

        var videoPlayer = FindObjectOfType<VideoPlayer>();
        
        if (isIntroduction)
        {
            videoPlayer.GetComponent<Renderer>().enabled = true;
            
            if (isPhysical)
            {
                videoPlayer.clip = isOcclusion ? Video1 : Video2;
            }
            else
            {
                videoPlayer.clip = isOcclusion ? Video3 : Video4;
            }
            
            videoPlayer.Stop();
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.GetComponent<Renderer>().enabled = false;
            videoPlayer.Stop();
        }
        
        for (int i = 0; i < InteractionOrbAnchor.transform.childCount; i++)
        {
            DestroyImmediate(InteractionOrbAnchor.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < TargetAnchor.transform.childCount; i++)
        {
            DestroyImmediate(TargetAnchor.transform.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < CloudAnchor.transform.childCount; i++)
        {
            DestroyImmediate(CloudAnchor.transform.GetChild(i).gameObject);
        }

        _orb = Instantiate(InteractionOrbPrefab).GetComponent<InteractionOrb>();
        _orb.transform.parent = InteractionOrbAnchor.transform;
        _orb.transform.localPosition = Vector3.zero;
        _orb.transform.localRotation = Quaternion.identity;
        _orb.transform.localScale = InteractionOrbPrefab.transform.localScale;
        
        var target = Instantiate(TargetPrefab).GetComponent<TargetItem>();
        target.transform.parent = TargetAnchor.transform;
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        target.transform.localScale = TargetPrefab.transform.localScale;
        
        var cloud = Instantiate(CloudPrefab).GetComponent<Cloud>();
        cloud.transform.parent = CloudAnchor.transform;
        cloud.transform.localPosition = Vector3.zero;
        cloud.transform.localRotation = Quaternion.identity;
        cloud.transform.localScale = CloudPrefab.transform.localScale;

        StartCoroutine(Wait());
        IEnumerator Wait()
        {
            yield return new WaitForEndOfFrame();
            _orb.Initialize(isOcclusion,isPhysical);
            yield return new WaitForEndOfFrame();
            FindObjectOfType<ResultManager>().Initialize(isIntroduction,iterations,trialCount,restingTime);
        }
    }

    private Coroutine _moveOrbAnchor;
    public void Update()
    {

        var targetToOrbVec3 = InteractionOrbAnchor.transform.position - CloudAnchor.transform.position;
        var targetToCameraVec3 = _camera.transform.position - CloudAnchor.transform.position;
        var targetToOrb = new Vector2(targetToOrbVec3.x, targetToOrbVec3.z);
        var targetToCamera = new Vector2(targetToCameraVec3.x, targetToCameraVec3.z);

        if ( _moveOrbAnchor == null && Vector2.Angle(targetToOrb, targetToCamera) > 10f && _orb != null && !_orb.IsCurrentlyManipulated)
        {
            _moveOrbAnchor = StartCoroutine(Move());
        }
        IEnumerator Move()
        {
            var angle = Vector2.SignedAngle(targetToOrb, targetToCamera);
            
            while (Math.Abs(angle) > 0.5f && _orb != null && !_orb.IsCurrentlyManipulated)
            {
                InteractionOrbAnchor.transform.RotateAround(CloudAnchor.transform.position,Vector3.up, -angle * 0.03f);
                TargetAnchor.transform.RotateAround(CloudAnchor.transform.position,Vector3.up, -angle * 0.03f);
                
                InteractionOrbAnchor.transform.LookAt(_camera.transform);
                targetToOrbVec3 = InteractionOrbAnchor.transform.position - CloudAnchor.transform.position;
                targetToCameraVec3 = _camera.transform.position - CloudAnchor.transform.position;
                targetToOrb = new Vector2(targetToOrbVec3.x, targetToOrbVec3.z);
                targetToCamera = new Vector2(targetToCameraVec3.x, targetToCameraVec3.z);
                angle = Vector2.SignedAngle(targetToOrb, targetToCamera);
                yield return null;
            }

            _moveOrbAnchor = null;
        }
    }
}
