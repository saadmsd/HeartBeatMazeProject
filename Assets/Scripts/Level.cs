using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    private GameObject monsterPrefab;

    [SerializeField]
    private GameObject goalPrefab;
    private int nb_monsters = 2;

    private List<GameObject> monsters = new List<GameObject>();

    private List<GameObject> goals = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateMonsters();
        CreateGoals();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        // Instantiate the goal prefab at a random position
        GameObject goal = Instantiate(goalPrefab, GetRandomPosition(), Quaternion.identity);
        goals.Add(goal);
    }
    private Vector3 GetRandomPosition()
    {
        // Generate a random position within the level bounds
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