using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationLevel : MonoBehaviour
{
    [SerializeField]
    public GameObject game;

    [SerializeField]
    private GameObject monsterPrefab;

    [SerializeField]
    private GameObject goalPrefab;
    
    private int nb_monsters = 0;

    private int nb_goals = 3;

    private float minSpeed = 1.0f;
    private float maxSpeed = 3.5f;
    private float speedIncrease = 0.1f; 
    private float switchInterval = 2f; 


    private float currentSpeed;
    private float timer;

    private List<GameObject> monsters = new List<GameObject>();

    private List<GameObject> goals = new List<GameObject>();

    




    void Start()
    {
        currentSpeed = minSpeed;
        
        CreateGoals();
    }

    // Update is called once per frame
    void Update()
    {
        if ( !(game.GetComponent<Game>().isPlaying && game.GetComponent<Game>().niveau == 0)){
            return;
        }
        // Increase the speed of monsters over time
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            // Increment the speed
            currentSpeed += speedIncrease;

            // Cap the speed at the maximum value
            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }

            // Update the speed for all monsters
            foreach (var monster in monsters)
            {
                Agent agent = monster.GetComponent<Agent>(); // Assuming the Agent component controls the speed
                if (agent != null)
                {
                    agent.speed = currentSpeed;
                }
            }

            // Reset the timer
            timer = 0f;
        }

        Debug.Log("Speed: " + currentSpeed);
    }

    private void CreateMonsters()
    {
        for (int i = 0; i < nb_monsters ; i++)
        {
            GameObject monster = Instantiate(monsterPrefab, GetRandomPosition(), Quaternion.identity);
            monsters.Add(monster);
        }
    }

    private void CreateGoals()
    {
        
        for (int i = 0; i < nb_goals; i++)
        {
            GameObject goal = Instantiate(goalPrefab, GetRandomPosition(), Quaternion.identity);
            goals.Add(goal);
        }
        
    }
    private Vector3 GetRandomPosition()
    {
        
        return new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10));
    }

    public List<GameObject> GetMonsters()
    {
        return monsters;
    }

    internal List<GameObject> GetGoals()
    {
        return goals;
    }
}