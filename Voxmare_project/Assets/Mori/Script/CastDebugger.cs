using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastDebugger : MonoBehaviour
{
    Block block;
    // Start is called before the first frame update
    void Start()
    {
        block = GetComponent<Block>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(block.CheckOverlap(), this);
    }
}
