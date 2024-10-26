using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    // 目的地となるGameObjectをセットします。
    public Transform target;
    private NavMeshAgent myAgent;
    public bool canFollow=true;

    void Start()
    {
        // Nav Mesh Agent を取得します。
        myAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // targetに向かって移動します。
        if(canFollow){
            myAgent.SetDestination(target.position);
        }
    }

    public void CanFollowTrue(){
        canFollow=true;
    }
    public void CanFollowFalse(){
        canFollow=false;
    }
    public void NavMeshAgentFalse(){
        myAgent.enabled=false;
    }public void NavMeshAgentTrue(){
        myAgent.enabled=true;
    }
}