using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearChecker : MonoBehaviour
{
    [SerializeField] private int count; // Show in inspector for debug

    public void decrementCount()
    {
        count--;
        if(count == 0)
        {
            Clear();
        }
    }

    public void setCount(int num)
    {
        count = num;
    }

    void Clear()
    {
        Debug.Log("Clear");
        // do something
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
