using System.Collections;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class InteractionOrb : MonoBehaviour
{
    public UnityAction OnGrab;
    public UnityAction OnRelease;
    public UnityAction OnInteraction;

    public bool IsPhysicalMenu;
    public bool HasOcclusion;

    [HideInInspector] public RadialMenuItem CurrentSelected;
    [HideInInspector] public RadialMenuItem MenuRoot;
    public bool IsCurrentlyManipulated { private set; get; } = false;

    private ObjectManipulator _manipulator;
    private TouchHandler _touchHandler;
    private NearInteractionTouchable _touchable;
    private NearInteractionGrabbable _nearInteraction;
    private Collider _collider;
    private Rigidbody _rigidbody;

    private Color _standardColor;


    public void Initialize(bool IsOcclusionEnabled, bool IsPhysical)
    {
        MixedRealityHandTrackingProfile handTrackingProfile =
            CoreServices.InputSystem?.InputSystemProfile.HandTrackingProfile;
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

        FindObjectOfType<HololensManager>().TriggerMenu += TriggerMenu;

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

            var rotConstaint = gameObject.AddComponent<RotationAxisConstraint>();
            rotConstaint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.YAxis | AxisFlags.ZAxis;

            _manipulator = GetComponent<ObjectManipulator>();
            if (_manipulator == null)
            {
                _manipulator = gameObject.AddComponent<ObjectManipulator>();
                _manipulator.AllowFarManipulation = false;
            }


            _collider = gameObject.AddComponent<SphereCollider>();

            _nearInteraction = GetComponent<NearInteractionGrabbable>();
            if (_nearInteraction == null)
            {
                _nearInteraction = gameObject.AddComponent<NearInteractionGrabbable>();
            }

            _manipulator.OnManipulationStarted.AddListener(OnManipulationStart);
            _manipulator.OnManipulationEnded.AddListener(OnManipulationEnd);

            StartCoroutine(Wait());

            IEnumerator Wait()
            {
                yield return null;
                DestroyImmediate(MenuRoot.GetComponent<Collider>());
                DestroyImmediate(MenuRoot.GetComponent<Rigidbody>());
                DestroyImmediate(MenuRoot.GetComponent<ObjectManipulator>());
                DestroyImmediate(MenuRoot.GetComponent<NearInteractionGrabbable>());
            }
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

            StartCoroutine(Wait());

            IEnumerator Wait()
            {
                yield return null;
                DestroyImmediate(MenuRoot.GetComponent<Collider>());
                DestroyImmediate(MenuRoot.GetComponent<TouchHandler>());
                DestroyImmediate(MenuRoot.GetComponent<NearInteractionTouchable>());
            }
        }

        _standardColor = GetComponent<Renderer>().material.color;

        foreach (var itemMetadata in RadialMenuItemMetadata.ExampleMenu)
        {
            MenuRoot.AddChildren(CreateMenuItem(itemMetadata));
        }

        _collider.isTrigger = true;
    }

    private void TriggerMenu()
    {
        _colliderCount = 0;

        if (MenuRoot != null && MenuRoot.GetComponent<Collider>() != null)
        {
            if (MenuRoot.IsExpanded)
            {
                MenuRoot.Hide(true);
            }
            else
            {
                MenuRoot.GetComponent<Collider>().enabled = true;
            }
        }

        var v = Camera.main.transform.position;
        //v.y = transform.position.y;
        transform.localRotation = Quaternion.identity;
        transform.LookAt(v, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(180f, Vector3.up);

        GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
        GetComponent<Collider>().enabled = !GetComponent<Collider>().enabled;
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
        IsCurrentlyManipulated = true;
        MenuRoot.transform.parent = null;
        MenuRoot.StartHover();

        GetComponent<Renderer>().material.color = Color.blue;

        OnGrab?.Invoke();
        OnInteraction?.Invoke();
    }

    public void OnManipulationEnd(ManipulationEventData eventData)
    {
        _colliderCount = 0;

        MenuRoot.Hide(false);

        GetComponent<Renderer>().material.color = _standardColor;

        OnRelease?.Invoke();

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

            IsCurrentlyManipulated = false;
            MenuRoot.transform.parent = transform;
            MenuRoot.transform.localPosition = Vector3.zero;
            MenuRoot.transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
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

    private int _colliderCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        RadialMenuItem item = other.GetComponent<RadialMenuItem>();
        if (item == null)
        {
            return;
        }

        if (IsCurrentlyManipulated)
        {
            _colliderCount++;
            GetComponent<Renderer>().material.color = Color.green;
            GetComponent<AudioSource>().PlayOneShot(FindObjectOfType<HololensManager>().HoverSound);
        }
        else
        {
            _colliderCount = 0;
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

        _colliderCount--;
        if (_colliderCount <= 0)
        {
            if (IsCurrentlyManipulated)
            {
                GetComponent<Renderer>().material.color = Color.blue;
            }
            else
            {
                GetComponent<Renderer>().material.color = _standardColor;
            }
        }
    }

    private void OnDestroy()
    {
        if (MenuRoot != null)
        {
            DestroyImmediate(MenuRoot.gameObject);
        }


        FindObjectOfType<HololensManager>().TriggerMenu -= TriggerMenu;
        OnRelease = null;
        OnGrab = null;
    }
}