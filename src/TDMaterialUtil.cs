using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public abstract class TDMatUtil
    {
   
    public abstract Material GetMaterial(Material baseMaterial); 

    }

public class UTSMatUtil: TDMatUtil
{
    public override Material GetMaterial(Material baseMaterial)
    {
        var mat = baseMaterial;
        if (baseMaterial.name.Contains("fskin_ZBias") )
        {
            // まつ毛、まぶた
            mat.SetFloat("_CullMode", 0f);
        }
        else
        {
            mat.SetColor("_Outline_Color", new Color(0.2f, 0.2f, 0.2f, 1));
            mat.SetFloat("_Outline_Width", 100.0f);
        }

        mat.SetFloat("_BaseColor_Step", 0.0f);
        mat.SetFloat("_Unlit_Intensity", 4.0f);
        
        mat.SetFloat("_Clipping_Level", 0.0f);
        mat.SetFloat("_IsBaseMapAlphaAsClippingMask", 1.0f);
       
        
        return mat;
    }
}
public class UnlitMatUtil : TDMatUtil
{
    public override Material GetMaterial(Material baseMaterial)
    {
        return baseMaterial;
            }

}
public class OrigialMatUtil : TDMatUtil
{
    public override Material GetMaterial(Material baseMaterial)
    {
       return baseMaterial;
    }
}
