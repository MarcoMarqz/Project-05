using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject player;
    public GameObject enemyPrefab;
    public GameObject enemyPrefab2;
    public int enemyCount1 = 15; // Count for enemyPrefab
    public int enemyCount2 = 3;  // Count for enemyPrefab2
    public GameObject coinPrefab;

    const int N = 1;
    const int E = 2;
    const int S = 4;
    const int W = 8;

    Dictionary<Vector2, int> cell_walls = new Dictionary<Vector2, int>();

    float tile_size = 10;
    public int width = 10;
    public int height = 10;

    List<List<int>> map = new List<List<int>>();
    private List<Vector3> enemyPositions = new List<Vector3>();

    void Start()
    {
        cell_walls[new Vector2(0, -1)] = N;
        cell_walls[new Vector2(1, 0)] = E;
        cell_walls[new Vector2(0, 1)] = S;
        cell_walls[new Vector2(-1, 0)] = W;

        MakeMaze();
        SpawnPlayer();
        SpawnEnemies();
        SpawnCoins(24);
    }

    private List<Vector2> CheckNeighbors(Vector2 cell, List<Vector2> unvisited)
    {
        List<Vector2> list = new List<Vector2>();
        foreach (var n in cell_walls.Keys)
        {
            if (unvisited.IndexOf((cell + n)) != -1)
            {
                list.Add(cell + n);
            }
        }
        return list;
    }

    private void MakeMaze()
    {
        List<Vector2> unvisited = new List<Vector2>();
        List<Vector2> stack = new List<Vector2>();

        for (int i = 0; i < width; i++)
        {
            map.Add(new List<int>());
            for (int j = 0; j < height; j++)
            {
                map[i].Add(N | E | S | W);
                unvisited.Add(new Vector2(i, j));
            }
        }

        Vector2 current = new Vector2(0, 0);
        unvisited.Remove(current);

        while (unvisited.Count > 0)
        {
            List<Vector2> neighbors = CheckNeighbors(current, unvisited);

            if (neighbors.Count > 0)
            {
                Vector2 next = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
                stack.Add(current);

                Vector2 dir = next - current;

                int current_walls = map[(int)current.x][(int)current.y] - cell_walls[dir];
                int next_walls = map[(int)next.x][(int)next.y] - cell_walls[-dir];

                map[(int)current.x][(int)current.y] = current_walls;
                map[(int)next.x][(int)next.y] = next_walls;

                current = next;
                unvisited.Remove(current);
            }
            else if (stack.Count > 0)
            {
                current = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = GameObject.Instantiate(tiles[map[i][j]]);
                tile.transform.parent = gameObject.transform;
                tile.transform.Translate(new Vector3(j * tile_size, 0, i * tile_size));
                tile.name += " " + i.ToString() + ' ' + j.ToString();
                tile.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
            }
        }
    }

    private void SpawnPlayer()
    {
        // Default position (can customize per scene)
        int x = 1;
        int y = 1;

        // If you're in Maze2, force spawn at (1,1)
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Maze2")
        {
            x = 1;
            y = 1;
        }

        Vector3 spawnPosition = new Vector3(y * tile_size, 1f, x * tile_size);

        // Destroy any player carried over from previous scene
        GameObject existingPlayer = GameObject.FindWithTag("PLAYER");
        if (existingPlayer != null)
        {
            Destroy(existingPlayer);
        }

        GameObject p = GameObject.Instantiate(player);
        p.transform.position = spawnPosition;
    }



    private void SpawnEnemies()
    {
        List<Vector3> usedPositions = new List<Vector3>();
        float minDistance = tile_size * 0.9f;
        float checkRadius = 1f;

        int totalCount = enemyCount1 + enemyCount2;
        int attempts = 0;
        int maxAttempts = totalCount * 50;
        int spawned1 = 0;
        int spawned2 = 0;

        while ((spawned1 + spawned2) < totalCount && attempts < maxAttempts)
        {
            attempts++;

            int x = UnityEngine.Random.Range(0, width);
            int y = UnityEngine.Random.Range(0, height);

            Vector3 spawnPosition = new Vector3(y * tile_size, 1f, x * tile_size);

            bool tooClose = false;
            foreach (var pos in usedPositions)
            {
                if (Vector3.Distance(spawnPosition, pos) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose)
                continue;

            Collider[] colliders = Physics.OverlapSphere(spawnPosition, checkRadius);
            bool blocked = false;
            foreach (var col in colliders)
            {
                if (col.gameObject != gameObject && col.gameObject.tag != "Walkable")
                {
                    blocked = true;
                    break;
                }
            }
            if (blocked)
                continue;

            GameObject prefabToSpawn = null;

            if (spawned1 < enemyCount1)
            {
                prefabToSpawn = enemyPrefab;
                spawned1++;
            }
            else if (spawned2 < enemyCount2)
            {
                prefabToSpawn = enemyPrefab2;
                spawned2++;
            }

            if (prefabToSpawn != null)
            {
                GameObject enemy = GameObject.Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                enemy.name = "Enemy " + (spawned1 + spawned2);
                enemy.tag = "Enemy";
                usedPositions.Add(spawnPosition);
                enemyPositions.Add(spawnPosition);
            }
        }

        if ((spawned1 + spawned2) < totalCount)
            Debug.LogWarning($"Only spawned {spawned1 + spawned2} out of {totalCount} enemies.");
    }

    private void SpawnCoins(int count)
    {
        float offset = 2f;
        int spawned = 0;

        foreach (Vector3 enemyPos in enemyPositions)
        {
            if (spawned >= count) break;

            Vector3 offsetDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
            Vector3 coinPos = enemyPos + offsetDir * offset;

            GameObject coin = GameObject.Instantiate(coinPrefab, coinPos, Quaternion.identity);
            coin.name = "Coin " + spawned;
            spawned++;
        }

        if (spawned < count)
            Debug.LogWarning($"Only spawned {spawned} out of {count} coins. Not enough enemy positions.");
    }
}


