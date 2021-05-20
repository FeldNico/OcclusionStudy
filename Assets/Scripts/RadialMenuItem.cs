using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class RadialMenuItem : MonoBehaviour
{
    public static UnityAction<RadialMenuItemMetadata.IItemType> OnSelect;
    public static UnityAction OnConfirm;

    public float Radius = 0.035f;
    public float AnimationTime = 0.2f;
    public List<RadialMenuItem> Children { private set; get; } = new List<RadialMenuItem>();

    public bool IsExpanded { private set; get; } = false;
    public RadialMenuItemMetadata.IItemType Type;

    private Collider _collider;
    private Renderer _renderer;
    private Rigidbody _rigidbody;
    private TouchHandler _touchHandler;
    private NearInteractionTouchable _touchable;
    private Camera _camera;
    private InteractionOrb _orb;
    private Target _target;

    void Start()
    {
        _orb = FindObjectOfType<InteractionOrb>();
        _target = FindObjectOfType<Target>();
        _camera = Camera.main;
        
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _renderer.enabled = false;

        if (_orb.IsPhysicalMenu)
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                _rigidbody = gameObject.AddComponent<Rigidbody>();
            }

            _rigidbody.useGravity = false;
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

        _collider = GetComponent<Collider>();
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<SphereCollider>();
        }

        _collider.isTrigger = true;
        _collider.enabled = false;
    }

    public void AddChildren(RadialMenuItem child)
    {
        Children.Add(child);
        child.transform.parent = transform;
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
    }

    public void StartHover()
    {
        if (Children.Count == 0 || IsExpanded)
        {
            return;
        }

        /*
        var hierachy = transform.GetComponentsInParent<Transform>().ToList();
        hierachy.AddRange(this.transform.GetComponentsInChildren<Transform>());
        foreach (var child in _orb.MenuRoot.GetComponentsInChildren<Transform>())
        {
            if (!hierachy.Contains(child))
            {
                child.GetComponent<RadialMenuItem>()?.Hide(true);
            }
        }
        */
        
        StartCoroutine(Animate());

        IEnumerator Animate()
        {
            foreach (var child in Children)
            {
                child._renderer.enabled = true;
                child._collider.enabled = false;
            }
            
            var start = 0f;
            while (start < AnimationTime)
            {
                yield return new WaitForEndOfFrame();
                foreach (var child in Children)
                {
                    var x = (float) (Radius * Math.Cos(2 * Children.IndexOf(child) * Math.PI / Children.Count));
                    var y = (float) (Radius * Math.Sin(2 * Children.IndexOf(child) * Math.PI / Children.Count));
                    var pos = transform.position + _camera.transform.up * x + _camera.transform.right * y;
                    pos = Vector3.Lerp(transform.position, pos, start / AnimationTime);
                    child.transform.position = pos;
                }

                start += Time.deltaTime;
            }

            foreach (var child in Children)
            {
                child._collider.enabled = true;
            }

            IsExpanded = true;
        }
    }

    public void Select()
    {
        if (Children.Count != 0)
        {
            StartHover();
        }
        else
        {
            if (Type != null)
            {
                OnSelect?.Invoke(Type);
            }
            else
            {
                OnConfirm?.Invoke();
            }
            _orb.MenuRoot.Hide(false);
        }
    }

    public void Hide(bool hideSelf)
    {
        if (hideSelf)
        {
            _collider.enabled = false;
        }

        IsExpanded = false;

        foreach (var child in Children)
        {
            child.Hide(true);
        }

        StartCoroutine(Animate());

        IEnumerator Animate()
        {
            var start = 0f;
            while (start < AnimationTime)
            {
                yield return new WaitForEndOfFrame();
                foreach (var child in Children)
                {
                    var pos = Vector3.Lerp(child.transform.localPosition, Vector3.zero, start / (AnimationTime));
                    child.transform.localPosition = pos;
                }

                start += Time.deltaTime;
            }

            if (hideSelf)
            {
                _renderer.enabled = false;
                transform.localPosition = Vector3.zero;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractionOrb orb = other.GetComponent<InteractionOrb>();
        if (orb == null)
        {
            return;
        }

        StartHover();
    }

    public void Update()
    {
        if (transform.parent != null && transform.parent != _orb.transform)
        {
            if (_orb.IsPhysicalMenu && IsExpanded && Vector3.Distance(_orb.transform.position, transform.position) > Radius * 1.2f)
            {
                _orb.CurrentSelected = null;
                Hide(false);
            }
        }
    }
}