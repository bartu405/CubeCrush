using UnityEngine;


public class ParticleManager : MonoBehaviour
{
    public GameObject redParticlePrefab;
    public GameObject greenParticlePrefab;
    public GameObject blueParticlePrefab;
    public GameObject yellowParticlePrefab;
    public GameObject vaseParticlePrefab_1;
    public GameObject vaseParticlePrefab_3;
    public GameObject stoneParticlePrefab_1;
    public GameObject stoneParticlePrefab_2;
    public GameObject stoneParticlePrefab_3;
    public GameObject boxParticlePrefab_1;
    public GameObject boxParticlePrefab_2;
    public GameObject boxParticlePrefab_3;


    public void InstantiateParticle(CubeType cubeType, Vector3 position)
    {
        switch (cubeType)
        {
            case CubeType.Red:
                Instantiate(redParticlePrefab, position, Quaternion.identity);
                break;
            case CubeType.Green:
                Instantiate(greenParticlePrefab, position, Quaternion.identity);
                break;
            case CubeType.Blue:
                Instantiate(blueParticlePrefab, position, Quaternion.identity);
                break;
            case CubeType.Yellow:
                Instantiate(yellowParticlePrefab, position, Quaternion.identity);
                break;
            case CubeType.Vase:
                Instantiate(vaseParticlePrefab_1, position, Quaternion.identity);
                Instantiate(vaseParticlePrefab_3, position, Quaternion.identity);
                break;
            case CubeType.Stone:
                Instantiate(stoneParticlePrefab_1, position, Quaternion.identity);
                Instantiate(stoneParticlePrefab_2, position, Quaternion.identity);
                Instantiate(stoneParticlePrefab_3, position, Quaternion.identity);
                break;
            case CubeType.Box:
                Instantiate(boxParticlePrefab_1, position, Quaternion.identity);
                Instantiate(boxParticlePrefab_2, position, Quaternion.identity);
                Instantiate(boxParticlePrefab_3, position, Quaternion.identity);
                break;

        }
    }

}


