using UnityEngine;
using System.Linq;
using Unity.MLAgents;
using System.Collections;
using System.Collections.Generic;

public class GcMazeController : MonoBehaviour
{

    public GameObject agentPrefab;
    public int numAgents;
    public List<FoodCollectorAgent> FoodCollectorAgents { get; private set; }


    public GameObject keyPrefab;
    public int numKeyFirst;
    public int numKeySecond;
    public bool respawnKey;

    public GameObject food;
    public int numFood;
    public bool respawnFood;

    // For both key and food
    public float range;

    // Stage information
    public int stepEachStage = 300;

    [HideInInspector]
    public int curStage;
    public int curStep;
    public int nextStageStep;
    public int aliveAgentNums;



    // Create food / key
    void Create(GameObject type, int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject f = Instantiate(type, new Vector3(Random.Range(-range, range), 1f,
                Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            f.GetComponent<FoodLogic>().respawn = respawnFood;
            f.GetComponent<FoodLogic>().myArea = this;
        }
    }

    // Clear food
    // Example : ClearObjects(GameObject.FindGameObjectsWithTag("food"));
    void ClearObjects(GameObject[] objects)
    {
        foreach (var food in objects)
        {
            Destroy(food);
        }
    }

    // Create agents
    void CreateAgent()
    {
        for (int i = 0; i < numAgents; i++)
        {
            Instantiate<GameObject>(agentPrefab, transform);
        }

        FoodCollectorAgents = transform.GetComponentsInChildren<FoodCollectorAgent>().ToList();
    }

    void EnvironmentReset()
    {

        ClearObjects(GameObject.FindGameObjectsWithTag("food"));
        ClearObjects(GameObject.FindGameObjectsWithTag("key"));
        //ClearObjects(GameObject.FindGameObjectsWithTag("agent"));

        if (FoodCollectorAgents == null) CreateAgent();

        foreach (var agent in FoodCollectorAgents)
        {
            agent.gameObject.SetActive(true);
        }

        Create(keyPrefab, numKeyFirst);
        //Create(food, numKeyFirst);

        curStage = 0;
        curStep = 0;
        nextStageStep = curStep + stepEachStage;
        aliveAgentNums = FoodCollectorAgents.Count();
    }

    // Start is called before the first frame update
    void Start()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        //EnvironmentReset();
    }

    // End episode for all agents
    void EndAllEpisodes()
    {
        foreach (var agent in FoodCollectorAgents)
        {
            agent.EndEpisode();
        }
        EnvironmentReset();
    }

    // Update is called once per frame
    void Update()
    {
        curStep++;
        if (curStep >= nextStageStep)
        {
            ClearObjects(GameObject.FindGameObjectsWithTag("food"));
            ClearObjects(GameObject.FindGameObjectsWithTag("key"));

            curStage += 1;
            nextStageStep = curStep + stepEachStage;

            if (curStage >= 3)
            {
                EndAllEpisodes();
                return;
            }

            foreach (var agent in FoodCollectorAgents)
            {
                if (agent.m_HaveAKey == false)
                {
                    agent.gameObject.SetActive(false);
                    aliveAgentNums--;
                }
                else
                {
                    agent.DropKey();
                }
            }

            //if (aliveAgentNums == 0)
            //{
            //    EndAllEpisodes();
            //    return;
            //}

            if(curStage == 1)
            {
                Create(keyPrefab, numKeySecond);
            }
            else
            {
                Create(food, numFood);
            }
        }
    }
}
