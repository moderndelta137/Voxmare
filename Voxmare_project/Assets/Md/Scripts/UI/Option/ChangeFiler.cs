using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFiler : MonoBehaviour
{
    public GameObject Off_text;
    public GameObject Neon_text;
    public GameObject Outline_text;
    public SnapshotMode Filter_script;
    public MeshRenderer Table_render;
    public Material Table_normal_material;
    public Material Table_simple_material;
    public int filter_index;
    // Start is called before the first frame update
    void Start()
    {
        filter_index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeIndex(int delta)
    {
        filter_index += delta;
        if(filter_index < 0)
        {
            filter_index = 2;//Max
        }
        else if(filter_index > 2)
        {
            filter_index = 0;//Min
        }
        Filter_script.filterIndex = filter_index;
        switch(filter_index)
        {
            case 0:
            Off_text.SetActive(true);
            Neon_text.SetActive(false);
            Outline_text.SetActive(false);
            Table_render.material = Table_normal_material;
            break;
            case 1:
            Off_text.SetActive(false);
            Neon_text.SetActive(true);
            Outline_text.SetActive(false);
            Table_render.material = Table_simple_material;
            break;
            case 2:
            Off_text.SetActive(false);
            Neon_text.SetActive(false);
            Outline_text.SetActive(true);
            Table_render.material = Table_simple_material;
            break;
        }
    }
}
