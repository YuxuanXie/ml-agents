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
    public GameObject keySecondPrefab;
    public int numKeyFirst;
    public int numKeySecond;
    public bool respawnKey=false;

    public GameObject food;
    public int numFood;
    public bool respawnFood=false;

    // For both key and food
    public float range;

    // Stage information
    public int stepEachStage = 300;

    [HideInInspector]
    public int curStage;
    public int curStep;
    public int nextStageStep;
    public int aliveAgentNums;

    private SimpleMultiAgentGroup m_AgentGroup;



    // Create food / key
    void Create(GameObject type, int num, bool respawn)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject f = Instantiate(type, new Vector3(Random.Range(-range, range), 1f,
                Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            f.GetComponent<FoodLogic>().respawn = respawn;
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
        //print("I create again");

        for (int i = 0; i < numAgents; i++)
        {
            Instantiate<GameObject>(agentPrefab, transform);
        }

        FoodCollectorAgents = transform.GetComponentsInChildren<FoodCollectorAgent>().ToList();

        for (int i = 0; i < FoodCollectorAgents.Count(); ++i)
        {
            if (i <= 37)
            {
                FoodCollectorAgents[i].maxHealth = 1;
                FoodCollectorAgents[i].hitPoint = 1;
            }
            else if( i >= 38 & i < 74)
            {
                FoodCollectorAgents[i].maxHealth = 2;
                FoodCollectorAgents[i].hitPoint = 2;
            }
            else if (i >= 74 & i < 81)
            {
                FoodCollectorAgents[i].maxHealth = 3;
                FoodCollectorAgents[i].hitPoint = 3;
            }
            else if (i >= 81 & i < 88)
            {
                FoodCollectorAgents[i].maxHealth = 4;
                FoodCollectorAgents[i].hitPoint = 4;
            }
            else if (i >= 88 & i < 95)
            {
                FoodCollectorAgents[i].maxHealth = 5;
                FoodCollectorAgents[i].hitPoint = 5;
            }
            else
            {
                FoodCollectorAgents[i].maxHealth = 6;
                FoodCollectorAgents[i].hitPoint = 6;
            }
        }
    }

    public void CreateOneKey()
    {
        Create(keyPrefab, 1, respawnKey);
    }

    void EnvironmentReset()
    {
        numKeyFirst = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("numKeyFirst", numKeyFirst);
        numKeySecond = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("numKeySecond", numKeySecond);
        numFood = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("numFood", numFood);

        ClearObjects(GameObject.FindGameObjectsWithTag("food"));
        ClearObjects(GameObject.FindGameObjectsWithTag("key"));

        if (FoodCollectorAgents == null)
            CreateAgent();

        foreach (var agent in FoodCollectorAgents)
        {
            //agent.gameObject.SetActive(true);
            agent.Respawn();
            m_AgentGroup.RegisterAgent(agent);
        }

        Create(keyPrefab, numKeyFirst, respawnKey);
        //Create(food, numKeyFirst);

        curStage = 0;
        curStep = 0;
        nextStageStep = curStep + stepEachStage;
        aliveAgentNums = FoodCollectorAgents.Count();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_AgentGroup = new SimpleMultiAgentGroup();
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        EnvironmentReset();

    }

    // Update is called once per frame
    void FixedUpdate()
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
                m_AgentGroup.EndGroupEpisode();
                EnvironmentReset();
                return;
            }


            foreach (var agent in FoodCollectorAgents)
            {
                if (agent.m_HaveAKey == false)
                {
                    agent.Die();
                    aliveAgentNums--;
                }
                else
                {
                    agent.DropKey();
                }
            }

            //if (aliveAgentNums == 0)
            //{
            //    m_AgentGroup.EndGroupEpisode();
            //    EnvironmentReset();
            //    return;
            //}
            ClearObjects(GameObject.FindGameObjectsWithTag("key"));
            if (curStage == 1)
            {
                Create(keySecondPrefab, numKeySecond, respawnKey);
            }
            else
            {
                Create(food, numFood, respawnFood);
            }
        }
    }
}
