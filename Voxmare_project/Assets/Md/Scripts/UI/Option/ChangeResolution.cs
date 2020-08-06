using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeResolution : MonoBehaviour
{
    public Vector2Int[] Resolutions;
    public int Resolution_index;
    public bool Fullscreened;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetResolution(int index)
    {
        Resolution_index = index;
        Screen.SetResolution(Resolutions[index].x,Resolutions[index].y,Fullscreened);
    }

    public void SetFullscreen(bool value)
    {
        Fullscreened = value;
        Screen.fullScreen = Fullscreened;
    }
}
