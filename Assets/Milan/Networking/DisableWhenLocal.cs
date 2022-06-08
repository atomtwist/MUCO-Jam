using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class DisableWhenLocal : MonoBehaviour
{

    public GameObject[] disable;
    public GameObject[] enable;
    
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<RealtimeView>().isOwnedLocally)
        {
            foreach (var go in disable)
            {
                go.SetActive(false);
            }

            foreach (var go in enable)
            {
                go.SetActive(true);
            }
        }
        else
        {
            foreach (var go in disable)
            {
                go.SetActive(true);
            }

            foreach (var go in enable)
            {
                go.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
