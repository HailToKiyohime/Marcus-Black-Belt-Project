using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SimplePickup : MonoBehaviour
{
    public GameObject currentItem;
    public int count;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (count == 0)
        {
            // add the ui to show it

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Collectable")
        {
            currentItem = other.gameObject;
            Destroy(other.gameObject); 
            count++;
        }
    }

}
