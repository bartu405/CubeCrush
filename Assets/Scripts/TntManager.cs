using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TntManager : MonoBehaviour
{

    public PlayManager playManager;
    public GameObject vaseParticlePrefab_2;
    public Sprite brokenVaseSprite;
    public GameObject tntParticlePrefab_1;
    public GameObject tntParticlePrefab_2;

    private void TriggerTNTExplosion(Cube cube)
    {
        if (cube != null && cube.cubeType == CubeType.Tnt && !cube.isExploded)
        {
            Debug.Log("Triggered tnt");
            cube.isExploded = true; // Mark the TNT as exploded, so it won't explode itself
            ExplodeTNT(cube.transform.position);
            StartCoroutine(playManager.SmoothDestroy(cube.gameObject)); // Destroy the TNT cube
        }
    }
    int CheckForAdjacentTnt(Vector3 position, CubeType clickedCubeType, List<GameObject> connectedCubes)
    {
        int count = 0;
        Collider2D neighborCollider = Physics2D.OverlapPoint(position);

        if (neighborCollider != null)
        {
            Cube neighborCube = neighborCollider.gameObject.GetComponent<Cube>();
            if (neighborCube != null && neighborCube.cubeType == clickedCubeType && !connectedCubes.Contains(neighborCollider.gameObject))
            {
                connectedCubes.Add(neighborCollider.gameObject);
                count = 1; // This cube is connected, so start count at 1


                // Increment the count by the number of connected cubes in each direction
                count += CheckForAdjacentTnt(neighborCube.transform.position + Vector3.up, clickedCubeType, connectedCubes);
                count += CheckForAdjacentTnt(neighborCube.transform.position + Vector3.down, clickedCubeType, connectedCubes);
                count += CheckForAdjacentTnt(neighborCube.transform.position + Vector3.left, clickedCubeType, connectedCubes);
                count += CheckForAdjacentTnt(neighborCube.transform.position + Vector3.right, clickedCubeType, connectedCubes);
            }
        }

        return count; // Return the total count of connected cubes
    }
    public void ExplodeTNT(Vector3 position)
    {
        // Manage particles for tnt here
        var particleSystemInstance = Instantiate(tntParticlePrefab_1, position, Quaternion.identity);
        var particleSystemInstance2 = Instantiate(tntParticlePrefab_2, position, Quaternion.identity);
        var ps = particleSystemInstance.GetComponent<ParticleSystem>();
        Destroy(particleSystemInstance, ps.main.duration / 3);


        int radius = 3; // This will create a 5x5 area (since size of the cubes are 1.62, radius = 2 won't work)
        List<GameObject> damagedVases = new List<GameObject>(); // To avoid damaging a vase twice

        //Burada adjacentlara bak ve işaretle isexploded diye sonra da destroy et
        List<GameObject> connectedTnts = new List<GameObject>(); //list for adjacent tnts
        CheckForAdjacentTnt(position, CubeType.Tnt, connectedTnts);
        //Debug.Log("count of adjacent tnt:" + connectedTnts.Count);
        if (connectedTnts.Count >= 2)
        {
            radius = 5; // This will create a 7x7 area for the Combo
            foreach (var tntGameObject in connectedTnts)
            {
                Cube cube = tntGameObject.GetComponent<Cube>();
                if (cube != null)
                {
                    cube.isExploded = true;

                    //Ama destroy yaparken gameobjecti destroy ediyoruz cube yerine
                    StartCoroutine(playManager.SmoothDestroy(tntGameObject));
                }
            }
        }

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3 explosionPosition = position + new Vector3(x, y, 0);

                Collider2D collider = Physics2D.OverlapPoint(explosionPosition);

                if (collider != null)
                {
                    Cube cube = collider.gameObject.GetComponent<Cube>();

                    if (cube != null)
                    {
                        if (cube.cubeType == CubeType.Box || cube.cubeType == CubeType.Vase || cube.cubeType == CubeType.Stone)
                        {
                            // Damage the obstacle
                            if (cube.cubeType == CubeType.Vase && !damagedVases.Contains(collider.gameObject))
                            {
                                cube.damage++;
                                damagedVases.Add(collider.gameObject); // Add the Vase to the list of damaged Vases
                            }
                            else if (cube.cubeType != CubeType.Vase)
                            {
                                cube.damage++;
                            }

                            if (cube.cubeType == CubeType.Vase && cube.damage == 1)
                            {

                                Instantiate(vaseParticlePrefab_2, collider.transform.position, Quaternion.identity);
                                // Change the sprite to the broken vase sprite
                                SpriteRenderer spriteRenderer = collider.gameObject.GetComponent<SpriteRenderer>();
                                if (spriteRenderer != null && brokenVaseSprite != null)
                                {
                                    spriteRenderer.sprite = brokenVaseSprite;
                                }
                            }
                            else if (cube.damage >= cube.maxDamage)
                            {
                                StartCoroutine(playManager.SmoothDestroy(collider.gameObject));

                            }
                        }
                        else if (cube.cubeType == CubeType.Tnt)
                        {
                            TriggerTNTExplosion(cube);
                        }
                        else
                        {
                            // Destroy normal cubes
                            StartCoroutine(playManager.SmoothDestroy(collider.gameObject));
                        }
                    }
                }
            }
        }
    }


}
