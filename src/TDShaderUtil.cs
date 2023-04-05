using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class TDShaderUtil{
    
    public  virtual Shader GetShader(string name)
    {
        Shader shader;
        if (name.Contains("fskin_ZBias") | name.Contains("fskin") | name.Contains("RFWToon_Clothes_PPhong_trans"))
        {

            shader = Shader.Find("Unlit/Transparent Cutout");
        }
        else if (name.Contains("trans"))
        {
            shader = Shader.Find("Unlit/Transparent");
            
        }
        else
        {
            shader = Shader.Find("Unlit/Texture");
        }
        return shader;
    }
    public virtual Shader GetShader(Shader shader) {
        return GetShader(shader.name);
    }
}
public class UTSShaderUtil: TDShaderUtil
{
    public  override  Shader GetShader(string name)
    {
        Shader shader;
        if (name.Contains("fskin_ZBias"))
        {

            shader = Shader.Find("UnityChanToonShader/NoOutline/ToonColor_DoubleShadeWithFeather_TransClipping");
        }
        else if (name.Contains("trans") | name.Contains("fskin") | name.Contains("RFWToon_Clothes_PPhong_trans"))
        {
            shader = Shader.Find("UnityChanToonShader/Toon_DoubleShadeWithFeather_TransClipping");

        }
        else
        {
            shader = Shader.Find("UnityChanToonShader/Toon_DoubleShadeWithFeather");
        }
        return shader;
    }
    public override Shader GetShader(Shader shader)
    {
        return GetShader(shader.name);
    }
}

public class UnlitShaderUtil: TDShaderUtil
{
    public override Shader GetShader(string name)
    {
        return base.GetShader(name);
    }
}

public class OriginalShaderUtil: TDShaderUtil
{
    public override Shader GetShader(string name)
    {
        return Shader.Find(name);
    }
    public override Shader GetShader(Shader shader)
    {
        return shader;
    }
}
