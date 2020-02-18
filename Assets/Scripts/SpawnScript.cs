using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    [SerializeField]
    private GameObject physicsCube;
    private float xPos = 1;
    [SerializeField]
    private float spawnXOffset = 5;
    private float spawnYOffset = 0;
    [SerializeField]
    private int amountToSpawn = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSpawner();
        }
    }
    void StartSpawner()
    {
        //GameObject newCube = physicsCube;

        StartCoroutine(CubeSpawner());
    }
    IEnumerator CubeSpawner()
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            yield return new WaitForSeconds(.05f);
            Instantiate(physicsCube, new Vector3(transform.position.x + xPos, transform.position.y + spawnYOffset, transform.position.z), Quaternion.identity, transform.parent);
            xPos += spawnXOffset;
            if (xPos >= amountToSpawn+1)
            {
                xPos = 1;
                spawnYOffset += 2;
            }
        }
    }
}
