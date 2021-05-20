using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class TouchHandler : MonoBehaviour, IMixedRealityTouchHandler
{
    private RadialMenuItem _item;
    private InteractionOrb _orb;

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
       
    }

    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {
        if (_item)
        {
            _item.Select();
            
            /*
            var hierachy = _item.transform.GetComponentsInParent<Transform>().ToList();
            hierachy.AddRange(_item.transform.GetComponentsInChildren<Transform>());
            foreach (var child in _orb.MenuRoot.GetComponentsInChildren<Transform>())
            {
                if (!hierachy.Contains(child))
                {
                    child.GetComponent<RadialMenuItem>()?.Hide(true);
                }
            }*/
            
        }
        else
        {
            _orb.MenuRoot.StartHover();
            _orb.OnGrabStart?.Invoke();
        }
    }

    public void OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        
    }
}
