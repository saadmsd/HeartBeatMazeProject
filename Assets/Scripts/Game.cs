using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

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

    bool loose = true;

    bool calibration = false;

    [SerializeField, Range(0f, 1f)]
    float pickLastProbability = 0.5f,
          openDeadEndProbability = 0.5f,
          openArbitraryProbability = 0.5f;

    [SerializeField]
    Player player;

	[SerializeField]
	private Agent[] agents;

    [SerializeField]
    TextMeshPro displayText;

    

    
	private float gameDuration = 300f; 
	[SerializeField] private TextMeshProUGUI timerText; 

    [SerializeField] private TextMeshProUGUI warningText; 


	[SerializeField]
    UDPListener udpListener;
	
    [SerializeField]
    Level0 level0;
    
    [SerializeField]
    Level1 level1;

    [SerializeField]
    Level2 level2;

    [SerializeField]
    CalibrationLevel calibrationLevel ;


    [SerializeField]

    UDPListener newBehaviourScript;

    public int niveau = -1;

    Maze maze;

    Scent scent;

    public bool isPlaying { get; private set; } = false;

    MazeCellObject[] cellObjects;

	// pour le chronomètre
	private float timeRemaining;
	private bool isTimeUp = false;


    void StartNewGame ()
    {
        isPlaying = true;
		isTimeUp = false;
        
        

        if (loose == false) {
            
            if (niveau != 2){
                niveau += 1;
            }

            

        } 
      
		
		// chronomètre
		timeRemaining = gameDuration;
		if (timerText != null )
		{
            
			timerText.gameObject.SetActive(true);
			timerText.text = $"Time Left: {timeRemaining:F2} sec";
		}
		
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

        if (calibration == false) {
            List<GameObject> monsters = calibrationLevel.GetMonsters(); 
            List<GameObject> goals = calibrationLevel.GetGoals();  
        
            int nb_agents = monsters.Count + goals.Count;  
            agents = new Agent[nb_agents];  
            
            for (int i = 0; i < monsters.Count; i++)
            {
                agents[i] = monsters[i].GetComponent<Agent>();
                agents[i].StartNewGame(maze, int2(Random.Range(0, mazeSize.x), Random.Range(0, mazeSize.y)));
            
            }
            for (int i = monsters.Count; i < nb_agents; i++)
            {
                agents[i] = goals[i - monsters.Count].GetComponent<Agent>();
                agents[i].StartNewGame(maze, int2(Random.Range(0, mazeSize.x), Random.Range(0, mazeSize.y)));
                
            }

            calibration = true;
            niveau = 0;
            return;
        

        }

		if (niveau == 2) {

            List<GameObject> monsters = level2.GetMonsters(); 
            List<GameObject> goals = level2.GetGoals();  
        
            int nb_agents = monsters.Count + goals.Count;  
            agents = new Agent[nb_agents];  
            
            for (int i = 0; i < monsters.Count; i++)
            {
                agents[i] = monsters[i].GetComponent<Agent>();
                agents[i].StartNewGame(maze, int2(Random.Range(0, mazeSize.x), Random.Range(0, mazeSize.y)));
            
            }
            for (int i = monsters.Count; i < nb_agents; i++)
            {
                agents[i] = goals[i - monsters.Count].GetComponent<Agent>();
                agents[i].StartNewGame(maze, int2(Random.Range(0, mazeSize.x), Random.Range(0, mazeSize.y)));
                
            }

            gameDuration = 60f;
            
        }else if (niveau == 1) {
            List<GameObject> monsters = level1.GetMonsters(); 
            List<GameObject> goals = level1.GetGoals();  
        
            int nb_agents = monsters.Count + goals.Count;  
            agents = new Agent[nb_agents];  
            
            for (int i = 0; i < monsters.Count; i++)
            {
                agents[i] = monsters[i].GetComponent<Agent>();
                agents[i].StartNewGame(maze, int2(Random.Range(0, mazeSize.x), Random.Range(0, mazeSize.y)));
            
            }
            for (int i = monsters.Count; i < nb_agents; i++)
            {
                agents[i] = goals[i - monsters.Count].GetComponent<Agent>();
                agents[i].StartNewGame(maze, int2(Random.Range(0, mazeSize.x), Random.Range(0, mazeSize.y)));
                
            }

            gameDuration = 120f;
        } else if (niveau == 0) {
            List<GameObject> monsters = level0.GetMonsters(); 
            List<GameObject> goals = level0.GetGoals();  
        
            int nb_agents = monsters.Count + goals.Count;  
            agents = new Agent[nb_agents];  
            
            for (int i = 0; i < monsters.Count; i++)
            {
                agents[i] = monsters[i].GetComponent<Agent>();
                agents[i].StartNewGame(maze, int2(Random.Range(0, mazeSize.x), Random.Range(0, mazeSize.y)));
            
            }
            for (int i = monsters.Count; i < nb_agents; i++)
            {
                agents[i] = goals[i - monsters.Count].GetComponent<Agent>();
                agents[i].StartNewGame(maze, int2(Random.Range(0, mazeSize.x), Random.Range(0, mazeSize.y)));
                
            }

            gameDuration = 160f;
        }

        bool warining_or_not = newBehaviourScript.warningText();
        if (warining_or_not) {
            warningText.gameObject.SetActive(true);
        } else {
            warningText.gameObject.SetActive(false);
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
		if (isTimeUp) return; // pou éviter d'appeler endgame plusieurs fois

		timeRemaining -= Time.deltaTime;

		if (timeRemaining <= 0)
		{
			timeRemaining = 0;
			isTimeUp = true;
			EndGame("Time's up! You lost !!");
            loose = true;
			return;
		}

		if (timerText != null)
		{
			timerText.text = $"Time Left: {timeRemaining:F2} sec";
		}
		
		Vector3 playerPosition = player.Move();
		NativeArray<float> currentScent = scent.Disperse(maze, playerPosition);
		for (int i = 0; i < agents.Length; i++)
		{
			Vector3 agentPosition = agents[i].Move(currentScent);
			if (
				new Vector2(
					agentPosition.x - playerPosition.x,
					agentPosition.z - playerPosition.z
				).sqrMagnitude < 1f
			)
			{
				EndGame(agents[i].TriggerMessage); // LOG le joueur a perdu
			
				return;
			}
		}
	}

	void EndGame (string message)
	{
		isPlaying = false;
		displayText.text = message;

        
        if (message == "YOU CAUGHT YELLOW")
        {
            loose = false;
        }else{
            loose = true;
        }
		displayText.gameObject.SetActive(true);
		for (int i = 0; i < agents.Length; i++)
		{
			agents[i].EndGame();
		}

		for (int i = 0; i < cellObjects.Length; i++)
		{
			cellObjects[i].Recycle();
		}

		OnDestroy();
	}

	void OnDestroy ()
	{
		maze.Dispose();
		scent.Dispose();
	}
}