using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool isTnt = false;
    public CubeType cubeType;

    public bool isExploded = false;
    public int damage = 0;
    public int maxDamage;

    public Cube()
    {
        // Set default maxDamage
        maxDamage = 1;

        // If the cube is a Vase, set maxDamage to 2
        if (cubeType == CubeType.Vase)
        {
            maxDamage = 2;
        }
    }

    public bool IsObstacle()
    {
        return cubeType == CubeType.Box || cubeType == CubeType.Stone || cubeType == CubeType.Vase;
    }
}