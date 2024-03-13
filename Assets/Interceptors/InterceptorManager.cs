using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InterceptorManager : MonoBehaviour
{
    [SerializeField] GameObject LeftClickPrefab;
    [SerializeField] GameObject RightClickPrefab;

    [HideInInspector] public UnityEvent<Plane> OnRescue; //Triggers when a friendly plane is intercepted.
    [HideInInspector] public UnityEvent<Plane> OnKill; //Triggers when a hijacked plane is intercepted.


    Camera mainCam;
    Airport[] airports;

    void Start()
    {
        mainCam = Camera.main;
        airports = FindObjectsOfType<Airport>();
    }

    void Update()
    {
        bool leftClicked = Input.GetMouseButtonDown(0);
        bool rightClicked = Input.GetMouseButtonDown(1);

        if (leftClicked || rightClicked)
        { 
            var hit = Physics2D.Raycast(mainCam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider == null || !hit.collider.GetComponent<Plane>())
                return;

            Plane plane = hit.collider.GetComponent<Plane>();
            if (leftClicked)
                Launch(LeftClickPrefab, plane);
            else
                Launch(RightClickPrefab, plane);
        }
    }

    void Launch(GameObject prefab, Plane target)
    {
        //Be careful that this doesn't become a performance problem on maps with many airports.
        Airport closestToTarget = null; float minDist = float.MaxValue;
        foreach (Airport airport in airports)
        {
            float dist = (airport.transform.position - target.transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                closestToTarget = airport; 
                minDist = dist;
            }
        }
        Debug.Assert(closestToTarget != null);

        Interceptor launched = Instantiate(prefab, closestToTarget.transform.position, Quaternion.identity).GetComponent<Interceptor>();
        launched.Target = target;
        launched.manager = this;
    }
}
