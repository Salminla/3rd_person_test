using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeGenerator : MonoBehaviour
{
    public GameObject cubeContainer;
    public Mesh mesh;
    public Material material;
    public float count = 50;
    public int scale = 2;
    public int seperation = 1;
    public float maxHeight = 10f;
    public float minHeight = 1f;

    [Range(0, 100)]
    public int cubeSpawnChance = 100;
    
    [Header("Color values")]
    [Range(0, 1)] public float colorHueMin = 0.0f;
    [Range(0, 1)] public float colorHueMax = 0.8f;
    [Range(0, 1)] public float colorSatMin = 0f;
    [Range(0, 1)] public float colorSatMax = 1f;
    [Range(0, 1)] public float colorValMin = 0f;
    [Range(0, 1)] public float colorValMax = 1f;

    private bool generated = false;
    private List<GameObject> objects;
    
    void Start()
    {
        //CreateCubes();
        
    }

    private void OnValidate()
    {
        if (colorHueMax < colorHueMin)
        {
            colorHueMax = colorHueMin;
        }
    }

    [ContextMenu("Generate")]
    public void GenerateCubes()
    {
        if (!generated)
            CreateCubes();
        else
        {
            DestroyAll();
            CreateCubes();
        }
    }
    public void CreateCubes()
    {
        
        objects = new List<GameObject>();
    
        for (int x = 0; x < count; x++)
        {
            for (int z = 0; z < count; z++)
            {
                if (Random.Range(0, 100) <= cubeSpawnChance)
                {
                    CreateCube(x * scale * seperation, z * scale * seperation);
                }
            }
        }
        generated = true;
        
    }
    private void CreateCube(int x, int z)
    {
        GameObject cube = new GameObject("Cube", typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider));
        cube.GetComponent<MeshFilter>().mesh = mesh;
        cube.GetComponent<MeshRenderer>().material = material;
        var tempMaterial = new Material(cube.GetComponent<MeshRenderer>().sharedMaterial);

        var localScaleOrig = cube.transform.localScale;
        cube.transform.parent = cubeContainer.transform;
        var transformPosition = cube.transform.position;
        transformPosition = new Vector3(x, transformPosition.y, z);
        localScaleOrig = new Vector3(localScaleOrig.x + scale-1, localScaleOrig.y, localScaleOrig.z + scale-1);
        
        var localScaleNew = localScaleOrig;
        var position = transformPosition;
        
        float randomHeight = Random.Range(minHeight, maxHeight);
        localScaleOrig = new Vector3(localScaleNew.x, randomHeight, localScaleNew.x);
        cube.transform.localScale = localScaleOrig;
        transformPosition = new Vector3(position.x,randomHeight/2,position.z);
        cube.transform.position = transformPosition;

        Color color = Random.ColorHSV(colorHueMin, colorHueMax, colorSatMin, colorSatMax, colorValMin, colorValMax);
        tempMaterial.color = color;
        cube.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
        
        objects.Add(cube);
    }
    [ContextMenu("Randomize")]
    public void OnRandomizeCPU()
    {
        if (generated)
        {
            foreach (var obj in objects)
            {
                var localScale = obj.transform.localScale;
                var position = obj.transform.position;
                float randomHeight = Random.Range(minHeight, maxHeight);
                obj.transform.localScale = new Vector3(localScale.x, randomHeight, localScale.x);
                obj.transform.position = new Vector3(position.x,randomHeight/2,position.z);

                var tempMaterial = new Material(obj.GetComponent<MeshRenderer>().sharedMaterial)
                {
                    color = Random.ColorHSV(0.0f, 0.8f)
                };
                obj.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;
            }
        }
    }
    [ContextMenu("Remove All")]
    public void DestroyAll()
    {
        foreach (var obj in objects)
            DestroyImmediate(obj);

        generated = false;
    }
}
