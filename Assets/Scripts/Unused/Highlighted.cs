using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighted : MonoBehaviour
{

    private Material _mat;

    void Awake()
    {
        _mat = gameObject.GetComponent<Renderer>().material;
    }

    public void IsHighlighted()
    {
        _mat.color = new Color(2,2,2,1);
    }
    public void NotHighlightied()
    {
        _mat.color = new Color(1,1,1,1);
    }
}
