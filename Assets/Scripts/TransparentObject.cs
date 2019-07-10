using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObject : MonoBehaviour
{

    private float alpha = .5f;
    private Material material;

    private Renderer[] comps;


    void Start()
    {
        comps = this.GetComponentsInChildren<Renderer>();
        makeTransparent();
    }

    public void makeTransparent(float alphaArg = 0.5f)
    {
        foreach (Renderer render in comps)
        {
            StandardShaderUtils.ChangeRenderMode(render.material, StandardShaderUtils.BlendMode.Transparent);
            Color color = render.material.color;
            color.a = alphaArg;
            render.material.color = color;
        }
    }
}