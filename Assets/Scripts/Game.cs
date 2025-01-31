using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    [SerializeField]
    MazeVisualization visualization;

    [SerializeField]
    int2 mazeSize = int2(20, 20);

    [SerializeField, Tooltip("Use zero for random seed.")]
    int seed;

    [SerializeField, Range(0f, 1f)]
    float pickLastProbability = 0.5f,
          openDeadEndProbability = 0.5f,
          openArbitraryProbability = 0.5f;

    [SerializeField]
    Player player;

    [SerializeField]
    TextMeshPro displayText;

    // Remove agents from here and reference Level's monsters
    [SerializeField]
    Level level;

    Maze maze;

    Scent scent;

    bool isPlaying;

    MazeCellObject[] cellObjects;

    void StartNewGame ()
    {
        isPlaying = true;
        displayText.gameObject.SetActive(false);
        maze = new Maze(mazeSize);
        scent = new Scent(maze);

        // Maze generation code
        new FindDiagonalPassagesJob
        {
            maze = maze
        }.ScheduleParallel(
            maze.Length, maze.SizeEW, new GenerateMazeJob
            {
                maze = maze,
                seed = seed != 0 ? seed : Random.Range(1, int.MaxValue),
                pickLastProbability = pickLastProbability,
                openDeadEndProbability = openDeadEndProbability,
                openArbitraryProbability = openArbitraryProbability
            }.Schedule()
        ).Complete();

        if (cellObjects == null || cellObjects.Length != maze.Length)
        {
            cellObjects = new MazeCellObject[maze.Length];
        }
        visualization.Visualize(maze, cellObjects);

        if (seed != 0)
        {
            Random.InitState(seed);
        }

        player.StartNewGame(maze.CoordinatesToWorldPosition(
            int2(Random.Range(0, mazeSize.x / 4), Random.Range(0, mazeSize.y / 4))
        ));

        // Initialize monsters (agents) from the Level script
        List<GameObject> monsters = level.GetMonsters();  // Get monsters created in Level
        Agent[] agents = new Agent[monsters.Count];  // Create an agent array with the same size
        for (int i = 0; i < monsters.Count; i++)
        {
            agents[i].StartNewGame(maze, int2(Random.Range(0, mazeSize.x), Random.Range(0, mazeSize.y)));
            monsters[i].transform.position = agents[i].transform.position;
        }
    }

    void Update ()
    {
        if (isPlaying)
        {
            UpdateGame();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            StartNewGame();
            UpdateGame();
        }
    }

    void UpdateGame ()
    {
        Vector3 playerPosition = player.Move();
        NativeArray<float> currentScent = scent.Disperse(maze, playerPosition);

        // Iterate through agents created by Level
        List<GameObject> monsters = level.GetMonsters();
        for (int i = 0; i < monsters.Count; i++)
        {
            Vector3 agentPosition = monsters[i].transform.position;
            if (new Vector2(agentPosition.x - playerPosition.x, agentPosition.z - playerPosition.z).sqrMagnitude < 1f)
            {
                EndGame("Game Over! Monster caught you.");
                return;
            }
        }
    }

    void EndGame (string message)
    {
        isPlaying = false;
        displayText.text = message;
        displayText.gameObject.SetActive(true);
        // Add any cleanup here if necessary
    }
}