using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Controller : MonoBehaviour
{
    [Header("HP")]
    public Slider HP_bar;
    [Header("Power Rank")]
    public Material Rank_Unlit_Material;
    [Space]
    public Material[] Rank_Lit_Materials;
    [Space]
    public Image[] Rank_images;
    [Space]
    public Material Rank_Dot_Unlit_Material;
    [Space]
    public Material[] Rank_Dot_Lit_Materials;

    [Space]
    public Image[] Rank0_Dot_image;
    public Image[] Rank1_Dot_image;
    public Image[] Rank2_Dot_image;
    public Image[] Rank3_Dot_image;
    public Image[] Rank4_Dot_image;
    public Image[] Rank5_Dot_image;
    public Image[] Rank6_Dot_image;
    [Space]
    public Text[] Rank_Texts;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMaxHP(int HP)
    {
        HP_bar.maxValue = HP;
    }
    public void UpdateHPBar(int HP)
    {
        HP_bar.value = HP;
    }

    public void UpdateRank(int Type, int Rank)
    {   
        //Update Main Rank Image
        if(Rank<=0)
        {
            Rank_images[Type].material = Rank_Unlit_Material;
            Rank_Texts[Type].color = Color.grey;
        }
        else
        {
            Rank_images[Type].material = Rank_Lit_Materials[Type];
            Rank_Texts[Type].color = Color.white;
        }

        //Update Rank Dot Image
        switch(Type)
        {
            case 0:
            for(int i = 0; i <= 2;i++)
            {
                if(i<Rank)
                    Rank0_Dot_image[i].material=Rank_Dot_Lit_Materials[Type];
                else
                    Rank0_Dot_image[i].material=Rank_Dot_Unlit_Material;
            }
            break;
            case 1:
            for(int i = 0; i <= 2;i++)
            {
                if(i<Rank)
                    Rank1_Dot_image[i].material=Rank_Dot_Lit_Materials[Type];
                else
                    Rank1_Dot_image[i].material=Rank_Dot_Unlit_Material;
            }
            break;
            case 2:
            for(int i = 0; i <= 2;i++)
            {
                if(i<Rank)
                    Rank2_Dot_image[i].material=Rank_Dot_Lit_Materials[Type];
                else
                    Rank2_Dot_image[i].material=Rank_Dot_Unlit_Material;
            }
            break;
            case 3:
            for(int i = 0; i <= 2;i++)
            {
                if(i<Rank)
                    Rank3_Dot_image[i].material=Rank_Dot_Lit_Materials[Type];
                else
                    Rank3_Dot_image[i].material=Rank_Dot_Unlit_Material;
            }
            break;
            case 4:
            for(int i = 0; i <= 2;i++)
            {
                if(i<Rank)
                    Rank4_Dot_image[i].material=Rank_Dot_Lit_Materials[Type];
                else
                    Rank4_Dot_image[i].material=Rank_Dot_Unlit_Material;
            }
            break;
            case 5:
            for(int i = 0; i <= 2;i++)
            {
                if(i<Rank)
                    Rank5_Dot_image[i].material=Rank_Dot_Lit_Materials[Type];
                else
                    Rank5_Dot_image[i].material=Rank_Dot_Unlit_Material;
            }
            break;
            case 6:
            for(int i = 0; i <= 2;i++)
            {
                if(i<Rank)
                    Rank6_Dot_image[i].material=Rank_Dot_Lit_Materials[Type];
                else
                    Rank6_Dot_image[i].material=Rank_Dot_Unlit_Material;
            }
            break;
        }
    }
}
