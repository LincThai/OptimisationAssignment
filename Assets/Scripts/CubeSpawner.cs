using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class CubeSpawner : MonoBehaviour
{
    public static CubeSpawner Instance {  get; private set; }

    [SerializeField] private SpawnedCube cubePrefab;
    [SerializeField] private Transform origin;

    private ObjectPool<SpawnedCube> cubePool;
    public ObjectPool<SpawnedCube> CubePool => cubePool;

    private int spawnRate = 16;
    private int maxSpawns = 10000;
    [HideInInspector] public int spawnedCubes;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
	{
        CreatePools();

        Task spawnCubesTask = SpawnCubes();
        await spawnCubesTask;

        if (spawnCubesTask.IsFaulted || spawnCubesTask.IsCanceled)
        {
            Debug.LogException(spawnCubesTask.Exception);
        }
	}
    
    private void CreatePools()
    {
        cubePool = new ObjectPool<SpawnedCube>(
            CreateCube,
            ActivateCube,
            DeactivateCube,
            DestroyCube,
            false,
            0, maxSpawns);
    }

    private SpawnedCube CreateCube() 
    {
        SpawnedCube cube = Instantiate(cubePrefab, origin);
        return cube;
    }

    private void ActivateCube(SpawnedCube cube)
    {
        cube.gameObject.SetActive(true);
    }

    private void DeactivateCube(SpawnedCube cube)
    {
        cube.gameObject.SetActive(false); 
    }

    private void DestroyCube(SpawnedCube cube)
    {
        Destroy(cube.gameObject);
    }

    private async Task SpawnCubes()
    {
        while (cubePool.CountActive < maxSpawns)
        {
            SpawnCube();
            await Task.Delay(spawnRate);
        }
    }

    private void SpawnCube() 
    {
        cubePool.Get(out SpawnedCube newSpawnedCube);

        if (!newSpawnedCube) return;
        newSpawnedCube.Initialized();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("I was called");
        CubePool.Clear();
    }
}
