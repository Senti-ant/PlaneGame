using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Departure 
{
    public FlightPlan plan; public GameObject planePrefab;
    public Departure(FlightPlan plan, GameObject planePrefab)
    {
        this.plan = plan; this.planePrefab = planePrefab;
    }
}

public class Airport : MonoBehaviour
{
    [SerializeField] GameObject ApprovalDialoguePrefab;
    [SerializeField] GameObject FlightPlanLinePrefab;

    //Events: Use them so scripts that the airport doesn't know about can do things when something happens here.
    [HideInInspector] public UnityEvent<Plane> OnFlightDeparts;
    [HideInInspector] public UnityEvent OnFlightCancelled;

    Queue<Departure> departures;
    Departure next;
    RectTransform canvas;
    LineRenderer lineGraphic;

    void Awake() 
    { 
        departures = new Queue<Departure>(); 
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>(); 

        lineGraphic = Instantiate(FlightPlanLinePrefab).GetComponent<LineRenderer>();
        lineGraphic.gameObject.SetActive(false);
    }

    void Update()
    {
        if (next == null && departures.Any())
        {
            next = departures.Dequeue();
            CreateApprovalDialogue(next);
        }
    }

    public void EnqueueFlight(FlightPlan flight, GameObject planePrefab) 
        => departures.Enqueue(new Departure(flight, planePrefab));
    public void EnqueueFlight(Departure departure)
        => departures.Enqueue(departure);

    public void DepartNext()
    {
        Vector3 spawnPos = transform.position;
        Quaternion spawnRot = next.plan.RotationAtDistance(0);
        Plane plane = Instantiate(next.planePrefab, spawnPos, spawnRot).GetComponentInChildren<Plane>();

        plane.Depart(next.plan);

        lineGraphic.gameObject.SetActive(false);
        next = null;

        OnFlightDeparts.Invoke(plane);
    }

    public void CancelNext()
    {
        lineGraphic.gameObject.SetActive(false);
        Score.Subtract(5, "Flight Canceled :(", transform.position);
        next = null;

        OnFlightCancelled.Invoke();
    }

    void CreateApprovalDialogue(Departure departure)
    {
        var dialogue = Instantiate(ApprovalDialoguePrefab, new Vector2(10000, 10000), Quaternion.identity, canvas)
                       .GetComponent<ApprovalDialogue>();
        dialogue.airport = this;

        departure.plan.Show(showOn: lineGraphic);
        lineGraphic.gameObject.SetActive(true);
    }

}
