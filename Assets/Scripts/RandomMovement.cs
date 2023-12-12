using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; //important

//if you use this code you are contractually obligated to like the YT video
public class RandomMovement : MonoBehaviour //don't forget to change the script name if you haven't
{
    public NavMeshAgent agent;
    public float range; //radius of sphere
    public Transform centrePoint; //centre of the area the agent wants to move around in
    public Transform targetAvatar;

    public GameObject seagull;
    public float minDistance = 5f;

    public Animator anim; 
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim.SetFloat("vertical", 0);
        //anim.SetFloat("orizontal", 1);
    }

    
    void Update()
    {
        // Check the distance between this object and the target avatar
        float distance = Vector3.Distance(agent.transform.position, targetAvatar.position);

        // If the distance is less than the minimum distance, perform the desired actions
        if (distance < minDistance)
        {
            anim.SetFloat("vertical", 1); // Enter in Seagull_interaction

            if (distance > 2f)
            {
                anim.SetFloat("orizontal", 0);
                agent.SetDestination(targetAvatar.position);
            }
            else
            {
                Vector3 directionToTarget = targetAvatar.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
                transform.rotation = rotation;
                anim.SetFloat("orizontal", 1);
                //agent.isStopped = true;
                seagull.GetComponent<PlayerData>().TakeDamage(1f);
            }
        }
        else
        {
            anim.SetFloat("orizontal", 0);
            if (agent.velocity.magnitude > 0)
            {
                anim.SetFloat("vertical", 1);
            }
            else
            {
                anim.SetFloat("vertical", 0);
            }

            if (agent.isStopped || agent.remainingDistance <= agent.stoppingDistance)
            {
                anim.SetFloat("vertical", 0);
                Vector3 point;
                if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
                {
                    Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                    agent.SetDestination(point);
                    anim.SetFloat("vertical", 1);
                }
            }
        }

        

        
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }
        
        result = Vector3.zero;
        return false;
    }


    /*void OnTriggerEnter(Collider other)
    {
        // Check if the object that triggered the collider is the desired avatar
        if (other.CompareTag("SeagullV5-Tex"))
        {
            // Perform desired actions when the avatar is nearby
            //Debug.Log("The target avatar is nearby!");
            anim.SetFloat("orizontal", 2);

            // You can perform other actions here, such as stopping or slowing down the NavMeshAgent
        }
        anim.SetFloat("orizontal", 2);
    }
    */
    




}
