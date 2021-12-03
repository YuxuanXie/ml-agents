using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgentsExamples;


public class FoodCollectorArea : Area
{
    public GameObject food;
    public GameObject badFood;
    public int numFood;
    public int numBadFood;
    public bool respawnFood;
    public float range;

    public GameObject agentPrefab;
    public int numAgent;

    public List<FoodCollectorAgent> FoodCollectorAgents { get; private set; }

    void CreateFood(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject f = Instantiate(type, new Vector3(Random.Range(-range, range), 1f,
                Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            f.GetComponent<FoodLogic>().respawn = respawnFood;
            //f.GetComponent<FoodLogic>().myArea = this;
        }
    }

    public void ResetFoodArea(GameObject[] agents)
    {
        foreach (GameObject agent in agents)
        {
            if (agent.transform.parent == gameObject.transform)
            {
                agent.transform.position = new Vector3(Random.Range(-range, range), 2f,
                    Random.Range(-range, range))
                    + transform.position;
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            }
        }
        CreateAgent();
        CreateFood(numFood, food);
        CreateFood(numBadFood, badFood);
    }

    void CreateAgent()
    {
        for (int i = 0; i < numAgent; i++)
        {
            Instantiate<GameObject>(agentPrefab, transform);
        }

        FoodCollectorAgents = transform.GetComponentsInChildren<FoodCollectorAgent>().ToList();
    }


    public override void ResetArea()
    {
    }
}
