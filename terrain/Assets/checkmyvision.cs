using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkmyvision : MonoBehaviour
{

   
    
    public enum enmsensitity { High, Low };
    public enmsensitity sensitity = enmsensitity.High;

    public bool targetInSight = false;

    public float fieldofvision = 45f;

    public Transform target = null;

    public Transform Myeyes= null;

    public Transform npcTranform = null;

    private SphereCollider sphereCollider = null;

    public Vector3 lastKhownSighting = Vector3.zero;


    // Start is called before the first frame update


    public void awake()
    {
        npcTranform = GetComponent<Transform>();
        sphereCollider = GetComponent<SphereCollider>();
        lastKhownSighting = npcTranform.position;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); 
    }

    bool InMyFieldOfVision()
    {
        Vector3 dirTotarget = target.position - Myeyes.position;

        float angle = Vector3.Angle(Myeyes.forward, dirTotarget);

        if (angle <= fieldofvision)
            return true;
        else
            return false;
    }


    bool ClearLineOfSight()
    {
        RaycastHit hit;

        if(Physics.Raycast(Myeyes.position,(target.position - Myeyes.position).normalized,out hit , sphereCollider.radius))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
                
        }
        return false;
    }

    void updateSight()
    {
        switch (sensitity)
        {
            case enmsensitity.High:
                targetInSight = InMyFieldOfVision() && ClearLineOfSight();
                break;
            case enmsensitity.Low:
                targetInSight = InMyFieldOfVision() || ClearLineOfSight();
                break;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        updateSight();
        if (targetInSight)
            lastKhownSighting = target.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        targetInSight = false;

    }






    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
