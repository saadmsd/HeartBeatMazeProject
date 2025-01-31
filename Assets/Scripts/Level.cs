using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    private GameObject monsterPrefab;

    private List<GameObject> monsters = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateMonsters();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateMonsters()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject monster = Instantiate(monsterPrefab, GetRandomPosition(), Quaternion.identity);
            monsters.Add(monster);
        }
    }

    private Vector3 GetRandomPosition()
    {
        // Generate a random position within the level bounds
        return new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
    }

    public List<GameObject> GetMonsters()
    {
        return monsters;
    }
}