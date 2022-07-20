using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawn : MonoBehaviour
{

    public GameObject blackKing;

    // Start is called before the first frame update
    void Start()
    {
        GameObject bk = Instantiate<GameObject>(blackKing);
       // bk.AddComponent<NavMeshAgent>();
        //bk.GetComponent<NavMeshAgent>().agentTypeID = "Humanoid";
        Debug.Log(bk.GetComponent<NavMeshAgent>().agentTypeID);
        NavMeshAgent bkAgent = bk.GetComponent<NavMeshAgent>();
        bkAgent.SetDestination(2*bk.transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
