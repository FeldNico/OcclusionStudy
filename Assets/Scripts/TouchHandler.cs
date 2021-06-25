using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class TouchHandler : MonoBehaviour, IMixedRealityTouchHandler
{
    private RadialMenuItem _item;
    private InteractionOrb _orb;

    private GameObject _dummy;
    private Vector3 _fingerTipPosition;

    private bool _currentlyTouched = false;
    private bool _blacklisted = false;
    
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
        if (_currentlyTouched || _blacklisted)
        {
            return;
        }

        if (_dummy != null)
        {
            DestroyImmediate(_dummy);
            _fingerTipPosition = Vector3.zero;
        }

        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left,
            out MixedRealityPose leftTipPose) && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip,
            Handedness.Right, out MixedRealityPose rightTipPose))
        {
            if (Vector3.Distance(leftTipPose.Position, rightTipPose.Position) < 0.1f)
            {
                return;
            }
        }

        if (eventData != null && HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, eventData.Handedness, out MixedRealityPose pose))
        {
            if (Vector3.Distance(Camera.main.transform.position, pose.Position) >
                Vector3.Distance(Camera.main.transform.position, transform.position))
            {
                _blacklisted = true;
                return;
            }
            
            _dummy = new GameObject(name + "_dummy");
            _dummy.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
            _dummy.AddComponent<MeshRenderer>().material = GetComponent<Renderer>().material;
            _dummy.transform.position = transform.position;
            _dummy.transform.rotation = transform.rotation;
            _dummy.transform.localScale = transform.lossyScale;

            GetComponent<Renderer>().enabled = false;

            _fingerTipPosition = pose.Position;
            _currentlyTouched = true;
        }
    }

    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        _currentlyTouched = false;

        if (_dummy != null)
        {
            DestroyImmediate(_dummy);
            _fingerTipPosition = Vector3.zero;
        }
        
        if (_blacklisted)
        {
            _blacklisted = false;
            return;
        }

        GetComponent<Renderer>().enabled = true;

        _orb.GetComponent<AudioSource>().PlayOneShot(FindObjectOfType<HololensManager>().SelectSound);
        
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
        if (_dummy != null && !_blacklisted)
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