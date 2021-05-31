using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class TouchHandler : MonoBehaviour, IMixedRealityTouchHandler
{
    private RadialMenuItem _item;
    private InteractionOrb _orb;

    private GameObject _dummy;
    private Vector3 _fingerTipPosition;

    private void Awake()
    {
        _item = GetComponent<RadialMenuItem>();
        _orb = GetComponent<InteractionOrb>();
        if (_orb == null)
        {
            _orb = FindObjectOfType<InteractionOrb>();
        }
    }

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        if (_dummy != null)
        {
            DestroyImmediate(_dummy);
            _fingerTipPosition = Vector3.zero;
        }
        
        
        if (eventData != null && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, eventData.Handedness, out MixedRealityPose pose))
        {
            _dummy = new GameObject(name + "_dummy");
            _dummy.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
            _dummy.AddComponent<MeshRenderer>().material = GetComponent<Renderer>().material;
            _dummy.transform.position = transform.position;
            _dummy.transform.rotation = transform.rotation;
            _dummy.transform.localScale = transform.lossyScale;

            GetComponent<Renderer>().enabled = false;

            _fingerTipPosition = pose.Position;
        }
    }

    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        if (_dummy != null)
        {
            DestroyImmediate(_dummy);
            _fingerTipPosition = Vector3.zero;
        }

        GetComponent<Renderer>().enabled = true;

        if (_item != null)
        {
            _item.Select();
        }
        else
        {
            _orb.MenuRoot.StartHover();
            _orb.OnInteraction?.Invoke();
        }
    }

    public void OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        if (_dummy != null)
        {
            if (eventData != null && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, eventData.Handedness,
                out MixedRealityPose pose))
            {
                var vec = Vector3.Project(_fingerTipPosition - pose.Position, -transform.forward);
                if (vec.magnitude <= 0.02f)
                {
                    _dummy.transform.position = transform.position - vec;
                }
                
            }
        }
    }
}