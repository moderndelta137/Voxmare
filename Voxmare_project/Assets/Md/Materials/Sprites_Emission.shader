Shader "Sprites/Emission"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

		[Toggle(_EMISSION)] _Emission ("Emission", Float ) = 0
        [HDR] _EmissionColor ("Emission Color", Color) = (0, 0, 0)
        [HDR] _EmissionMap ("Emission Map", 2D) = "black" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma shader_feature _EMISSION
            //#pragma multi_compile _ EMISSION        
            #include "UnitySprites.cginc"

            sampler2D _EmissionMap;
            half3 _EmissionColor;

            half4 frag(v2f IN) : SV_Target
            {
            #ifdef _EMISSION
                return SpriteFrag(IN) + half4(tex2D(_EmissionMap, IN.texcoord).rgb * _EmissionColor.rgb, 0.0);
            #else
                return SpriteFrag(IN);
            #endif
            }

        ENDCG
        }
    }
}