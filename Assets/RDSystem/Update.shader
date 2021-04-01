Shader "RDSystem/Update"
{
    Properties
    {
        _Du("Diffusion (u)", Range(0, 1)) = 1
        _Dv("Diffusion (v)", Range(0, 1)) = 0.4
        _Feed("Feed", Range(0, 0.1)) = 0.05
        _Kill("Kill", Range(0, 0.1)) = 0.05
        _KillCircle("Kill in Circle", Range(0, 0.1)) = 0.06
        _circlePos("Circle Center Position", Vector) = (0.5,0.5,0)
        _thickness("Circle Thickness", Range(0.001, 1)) = 0.05
        _radius("Circle Radius", Range(0.001, 1)) = 0.05
        _circlePos2("Input Circle Center Position", Vector) = (0.5,0.5,0)
        _radius2("Input Circle Radius", Range(0.001, 1)) = 0.05
    }

    CGINCLUDE

    #include "UnityCustomRenderTexture.cginc"

    half _Du, _Dv;
    half _Feed, _Kill, _KillCircle;
    half2 _circlePos, _circlePos2;
    half _radius, _radius2, _thickness;

    half4 frag(v2f_customrendertexture i) : SV_Target
    {
        float tw = 1 / _CustomRenderTextureWidth;
        float th = 1 / _CustomRenderTextureHeight;

        float2 uv = i.globalTexcoord;
        float4 duv = float4(tw, th, -tw, 0);

        half2 q = tex2D(_SelfTexture2D, uv).xy;

        half2 dq = -q;
        dq += tex2D(_SelfTexture2D, uv - duv.xy).xy * 0.05;
        dq += tex2D(_SelfTexture2D, uv - duv.wy).xy * 0.20;
        dq += tex2D(_SelfTexture2D, uv - duv.zy).xy * 0.05;
        dq += tex2D(_SelfTexture2D, uv + duv.zw).xy * 0.20;
        dq += tex2D(_SelfTexture2D, uv + duv.xw).xy * 0.20;
        dq += tex2D(_SelfTexture2D, uv + duv.zy).xy * 0.05;
        dq += tex2D(_SelfTexture2D, uv + duv.wy).xy * 0.20;
        dq += tex2D(_SelfTexture2D, uv + duv.xy).xy * 0.05;

        half ABB = q.x * q.y * q.y;

        //q += float2(dq.x * _Du - ABB + _Feed * (1 - q.x), dq.y * _Dv + ABB - (_Kill + _Feed) * q.y);

        //float2 c = uv;
        /*
        if (distance(uv, _mousePos) < _radius) {
            //c = (0, 0);
        }*/

        if (distance(uv, _circlePos) < _radius && distance(uv, _circlePos) > _radius - 0.05) {
            q += float2(dq.x * _Du - ABB + _Feed * (1 - q.x), dq.y * _Dv + ABB - (_KillCircle + _Feed) * q.y);
        }
        else if(distance(uv, _circlePos2) < _radius2 && distance(uv, _circlePos2) > _radius2 - _thickness){
            q += float2(dq.x * _Du - ABB + _Feed * (1 - q.x), dq.y * _Dv + ABB - (_KillCircle + _Feed) * q.y);
        }
        else {
            q += float2(dq.x * _Du - ABB + _Feed * (1 - q.x), dq.y * _Dv + ABB - (_Kill + _Feed) * q.y);
        }

        // normal Reaction diffusion
        return half4(saturate(q), 0, 0);

        // show uv coordinates
        //return half4(c.x,c.y,0,0);
    }

    ENDCG


    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "Update"
            CGPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            ENDCG
        }
    }
}
