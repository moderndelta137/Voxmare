using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBlockPalette : MonoBehaviour
{
    public Material Block_material;
    public Material HUD_eye_material;
    public Texture[] Block_palettes;
    public int Palette_index;

    public Dropdown UI_dropdown;
    // Start is called before the first frame update
    void Start() 
    {
        if(!PlayerPrefs.HasKey("ColorPalette"))
        {
            PlayerPrefs.SetInt("ColorPalette",0);
        }
        Palette_index = PlayerPrefs.GetInt("ColorPalette");
        UI_dropdown.value = Palette_index;
        Block_material.SetTexture("_MainTexture", Block_palettes[Palette_index]);
        HUD_eye_material.SetTexture("_MainTexture", Block_palettes[Palette_index]);
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void UpdatePalette(int index)
    {
        Palette_index = index;
        Block_material.SetTexture("_MainTexture", Block_palettes[Palette_index]);
        HUD_eye_material.SetTexture("_MainTexture", Block_palettes[Palette_index]);
        PlayerPrefs.SetInt("ColorPalette",index);
    }
}
