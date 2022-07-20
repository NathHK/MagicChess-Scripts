using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    private Material defaultMaterial;
    private Material currentMaterial;
    public Material selectTileMaterial;
    public Material validTileMaterial;
    public Material pieceSelectedMaterial;

    // Start is called before the first frame update
    void Start()
    {
        defaultMaterial = GetComponent<MeshRenderer>().material;
        currentMaterial = GetComponent<MeshRenderer>().material;
        
        //selected = false;
        //valid = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Material GetMaterial()
    {
        return currentMaterial;
    }

    public Material GetDefaultMaterial()
    {
        return GetComponent<MeshRenderer>().material;
    }

    public void RevertMaterial()
    {
        GetComponent<MeshRenderer>().material = defaultMaterial;
        currentMaterial = GetComponent<MeshRenderer>().material;

        //selected = false;
        //valid = false;
    }

    public void HighlightSelected()
    {
        GetComponent<MeshRenderer>().material = selectTileMaterial;
        currentMaterial = GetComponent<MeshRenderer>().material;

        //selected = true;
    }

    public void HighlightValid()
    {
        GetComponent<MeshRenderer>().material = validTileMaterial;
        currentMaterial = GetComponent<MeshRenderer>().material;

        //valid = true;
    }

    public void HighlightGreen()
    {
        GetComponent<MeshRenderer>().material = pieceSelectedMaterial;
        currentMaterial = GetComponent<MeshRenderer>().material;
    }
    
}
