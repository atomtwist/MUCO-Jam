using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class TakeOwnership : MonoBehaviour
{
    [Header("Use this component to ensure that only one player can do certain things, such as spawning toys.")]
    public RealtimeTransform realtimeTransform;
    public MonoBehaviour[] disableWhenNotConnected;
    public bool isUnownedSelf => realtimeTransform.isUnownedSelf;
    public bool isOwnedRemotelySelf => realtimeTransform.isOwnedRemotelySelf;
    public bool isOwnedLocallySelf => realtimeTransform.isOwnedLocallySelf;

    

    void Awake()
    {
        //get realtime stuff
        if (realtimeTransform == null)
            realtimeTransform = GetComponent<RealtimeTransform>();
    }

    void Update()
    {
        //bail if there's no connection
        if (!NetworkManager.Instance.realtime.connected)
            return;


        // whoever is around has to own the spawn fairy.
        if (realtimeTransform.isUnownedSelf)
        {
            realtimeTransform.RequestOwnership();
            Debug.Log("taken ownership of " + gameObject.name + "and enabled components");
            foreach (var monoBehaviour in disableWhenNotConnected)
            {
                monoBehaviour.enabled = true;
            }
        }

        //make sure that when we're not the owner, components are disabled
        if (realtimeTransform.isOwnedRemotelySelf)
        {
            foreach (var monoBehaviour in disableWhenNotConnected)
            {
                monoBehaviour.enabled = false;
            }
        }
    }

    public void RequestOwnership()
    {
        realtimeTransform.RequestOwnership();
    }

    
    
    
    
}
