using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBlockPalette : MonoBehaviour
{
    public Material Block_material;
    public MeshRenderer block_render;
    public Texture[] Block_palettes;
    public int Palette_index;

    public Dropdown UI_dropdown;
    // Start is called before the first frame update
    void Start()
    {
        //Palette_index = UI_dropdown.value;
        Block_material = block_render.sharedMaterial;
        Block_material.SetTexture("_MainTexture", Block_palettes[Palette_index]);
    }

    // Update is called once per frame
    void Update()
    {

        //Block_palettes[0]=Block_material.mainTexture;
        //Block_material.mainTexture=Block_palettes[Palette_index];
    }
    public void ChangePalette(int index)
    {
        Palette_index = index;
        Block_material.SetTexture("_MainTexture", Block_palettes[Palette_index]);
    }
}
