using System.Collections;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class InteractionOrb : MonoBehaviour
{
    public UnityAction OnGrabStart;
    public UnityAction OnGrabEnd;

    public bool IsPhysicalMenu;
    public bool HasOcclusion;
    
    [HideInInspector]
    public RadialMenuItem CurrentSelected;
    [HideInInspector]
    public RadialMenuItem MenuRoot;
    
    private ObjectManipulator _manipulator;
    private TouchHandler _touchHandler;
    private NearInteractionTouchable _touchable;
    private NearInteractionGrabbable _nearInteraction;
    private Collider _collider;
    private Rigidbody _rigidbody;

    private Color _standardColor;
    private bool _isCurrentlyManipulated = false;

    public void Initialize(bool IsOcclusionEnabled, bool IsPhysical)
    {
        MixedRealityHandTrackingProfile handTrackingProfile = CoreServices.InputSystem?.InputSystemProfile.HandTrackingProfile;
        if (handTrackingProfile != null)
        {
            handTrackingProfile.EnableHandMeshVisualization = IsOcclusionEnabled;
        }

        HasOcclusion = IsOcclusionEnabled;
        IsPhysicalMenu = IsPhysical;
        
        if (MenuRoot != null)
        {
            DestroyImmediate(MenuRoot.gameObject);
        }
        
        MenuRoot = new GameObject("MenuRoot").AddComponent<RadialMenuItem>();
        MenuRoot.transform.parent = transform;
        MenuRoot.transform.localPosition = Vector3.zero;
        MenuRoot.transform.localRotation = Quaternion.identity;
        MenuRoot.transform.localScale = Vector3.one;
        MenuRoot.Radius = 0.15f;

        _collider = GetComponent<Collider>();
        if (_collider)
        {
            DestroyImmediate(_collider);
        }
        
        if (IsPhysical)
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            _rigidbody.useGravity = false;
            
            _manipulator = GetComponent<ObjectManipulator>();
            if (_manipulator == null)
            {
                _manipulator = gameObject.AddComponent<ObjectManipulator>();
            }

            _manipulator.ManipulationType = ManipulationHandFlags.OneHanded;
        
            _collider = gameObject.AddComponent<SphereCollider>();
            
            _nearInteraction = GetComponent<NearInteractionGrabbable>();
            if (_nearInteraction == null)
            {
                _nearInteraction = gameObject.AddComponent<NearInteractionGrabbable>();
            }
            
            _manipulator.OnManipulationStarted.AddListener(OnManipulationStart);
            _manipulator.OnManipulationEnded.AddListener(OnManipulationEnd);
        }
        else
        {
            _collider = gameObject.AddComponent<BoxCollider>();
            _touchHandler = GetComponent<TouchHandler>();
            if (_touchHandler == null)
            {
                _touchHandler = gameObject.AddComponent<TouchHandler>();
            }
            _touchable = GetComponent<NearInteractionTouchable>();
            if (_touchable == null)
            {
                _touchable = gameObject.AddComponent<NearInteractionTouchable>();
            }
            
        }

        _collider.isTrigger = true;

        _standardColor = GetComponent<Renderer>().material.color;

        foreach (var itemMetadata in RadialMenuItemMetadata.ExampleMenu)
        {
            MenuRoot.AddChildren(CreateMenuItem(itemMetadata));
        }
    }

    private RadialMenuItem CreateMenuItem(RadialMenuItemMetadata metadata)
    {
        var go = Instantiate(Resources.Load<GameObject>(metadata.Prefab));
        go.name = metadata.Name;
        var item = go.GetComponent<RadialMenuItem>();
        if (item == null)
        {
            item = go.AddComponent<RadialMenuItem>();
        }

        if (metadata.Radius > 0f)
        {
            item.Radius = metadata.Radius;
        }
        item.Type = metadata.Type;

        if (metadata.Children != null)
        {
            foreach (var child in metadata.Children)
            {
                item.AddChildren(CreateMenuItem(child));
            }
        }

        return item;
    }

    public void OnManipulationStart(ManipulationEventData eventData)
    {
        _isCurrentlyManipulated = true;
        MenuRoot.transform.parent = null;
        MenuRoot.StartHover();

        GetComponent<Renderer>().material.color = Color.blue;
        
        OnGrabStart?.Invoke();
    }

    public void OnManipulationEnd(ManipulationEventData eventData)
    {
        MenuRoot.Hide(false);

        GetComponent<Renderer>().material.color = _standardColor;

        OnGrabEnd?.Invoke();
        
        StartCoroutine(Animate());
        IEnumerator Animate()
        {
            var start = 0f;
            while (start < MenuRoot.AnimationTime)
            {
                yield return new WaitForEndOfFrame();
                var pos = Vector3.Lerp(transform.position, MenuRoot.transform.position, start / MenuRoot.AnimationTime);
                transform.position = pos;
                start += Time.deltaTime;
            }
            _isCurrentlyManipulated = false;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            MenuRoot.transform.parent = transform;
            foreach (var child in MenuRoot.GetComponentsInChildren<Transform>())
            {
                child.transform.localPosition = Vector3.zero;
                child.transform.localRotation = Quaternion.identity;
            }
            if (_rigidbody)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }
        if (CurrentSelected != null)
        {
            GetComponent<AudioSource>().PlayOneShot(FindObjectOfType<HololensManager>().SelectSound);
            CurrentSelected.Select();
        }
        CurrentSelected = null;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        RadialMenuItem item = other.GetComponent<RadialMenuItem>();
        if (item == null)
        {
            return;
        }

        if (_isCurrentlyManipulated)
        {
            GetComponent<Renderer>().material.color = Color.green;
            GetComponent<AudioSource>().PlayOneShot(FindObjectOfType<HololensManager>().HoverSound);
        }
        else
        {
            GetComponent<Renderer>().material.color = _standardColor;
        }

        CurrentSelected = item;
    }

    private void OnTriggerExit(Collider other)
    {
        RadialMenuItem item = other.GetComponent<RadialMenuItem>();
        if (item == null)
        {
            return;
        }

        if (item == CurrentSelected)
        {
            CurrentSelected = null;
        }

        if (_isCurrentlyManipulated)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {
            GetComponent<Renderer>().material.color = _standardColor;
        }
    }

    private void OnDestroy()
    {
        if (MenuRoot != null)
        {
            DestroyImmediate(MenuRoot.gameObject);
        }

        OnGrabEnd = null;
        OnGrabStart = null;
    }
}