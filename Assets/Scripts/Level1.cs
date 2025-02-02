using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour
{
    [SerializeField]
    public GameObject game;

    [SerializeField]
    private GameObject monsterPrefab;

    [SerializeField]
    private GameObject goalPrefab;
    [SerializeField]
    UDPListener udpListener;

    
    private int nb_monsters = 2;

    private int nb_goals = 1;

    private float minSpeed = 1.0f;
    private float maxSpeed = 4f;
    private float speedIncrease = 0.15f; 
    private float switchInterval = 2f; 


    private float currentSpeed;
    private float currentFactoredSpeed;
    private float speed_factor = 1.0f;

    private bool increase;
    private float timer;

    private List<GameObject> monsters = new List<GameObject>();

    private List<GameObject> goals = new List<GameObject>();

    




    void Start()
    {
        currentSpeed = minSpeed;
        CreateMonsters();
        CreateGoals();
    }

    // Update is called once per frame
    void Update()
    {
        if ( !(game.GetComponent<Game>().isPlaying && game.GetComponent<Game>().niveau == 1)){
            return;
        }
        // Increase the speed of monsters over time
        timer += Time.deltaTime;

        if (timer >= switchInterval)
        {
            if (udpListener.speedFactor != speed_factor){ // If the speed factor has changed
                // incrementer ou decrementer si bpm augmente ou baisse
                Debug.Log("Speed factor changed from " + speed_factor + " to " + udpListener.speedFactor);
                if (udpListener.speedFactor > speed_factor){
                    increase = true;
                }
                else{
                    increase = false;
                }
                speed_factor = udpListener.speedFactor;
            }
            // Increment the speed (time)
            currentSpeed += speedIncrease;
            // Increment or decrement the speed (factor)
            if (increase){
                currentFactoredSpeed += speedIncrease * speed_factor;
            }
            else{
                currentFactoredSpeed -= speedIncrease * speed_factor;
            }
            
            // Cap the speed at the maximum value
            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
            if (currentFactoredSpeed > maxSpeed)
            {
                currentFactoredSpeed = maxSpeed;
            }
            // max entre speed du temps et factor
            float agent_speed = Mathf.Max(currentSpeed, currentFactoredSpeed);

            // Update the speed for all monsters
            foreach (var monster in monsters)
            {
                Agent agent = monster.GetComponent<Agent>(); // Assuming the Agent component controls the speed
                if (agent != null)
                {
                    agent.speed = agent_speed;
                }
            }
          //  Debug.Log("Speed: " + agent_speed);
            // Reset the timer
            timer = 0f;
        }
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