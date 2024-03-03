using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlightSpawner: MonoBehaviour
{
    [Header("I'm too lazy to make actual buttons so have some bools.")]
    [SerializeField] bool FetchAllAirports;
    [SerializeField] bool FetchAllCustomPaths;

    [Header("References")]
    [SerializeField] GameObject PlanePrefab;
    [SerializeField] Airport[] AvailableForSimplePaths;
    [SerializeField] PathInformation[] CustomPaths;

    [Header("Spawn Parameters")]
    [SerializeField] int NumFlights;
    [SerializeField] float EarliestTime;
    [SerializeField] float LatestTime;
    [SerializeField] float MinTimeBetweenSpawns;
    [SerializeField] [Range(0, 1)] float CustomPathChance;


    FlightPlan[] plannedFlights;

    void OnValidate()
    {
        if (FetchAllAirports)
        {
            AvailableForSimplePaths = FindObjectsOfType<Airport>();
            FetchAllAirports = false;
        }
        if (FetchAllCustomPaths)
        {
            CustomPaths = FindObjectsOfType<PathInformation>();
            FetchAllCustomPaths = false;
        }
    }

    void Start()
    {
        Airport[] airports = AvailableForSimplePaths;

        plannedFlights = new FlightPlan[NumFlights];
        for (int i = 0; i < NumFlights; i++)
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            plannedFlights[i] =  (r < CustomPathChance && CustomPaths.Length > 0) ? 
                                 FlightPlan.Random(CustomPaths, EarliestTime, LatestTime) :
                                 FlightPlan.Random(airports, EarliestTime, LatestTime);
        }

        Array.Sort(plannedFlights, (f1, f2) => f1.departureTime.CompareTo(f2.departureTime));
        StartCoroutine(SpawnFlights());
    }

    IEnumerator SpawnFlights()
    {
        float lastTime = 0;
        foreach (FlightPlan flight in plannedFlights)
        {
            yield return new WaitUntil(() => Time.timeSinceLevelLoad >= flight.departureTime
                                          && Time.timeSinceLevelLoad >= lastTime + MinTimeBetweenSpawns);

            lastTime = Time.timeSinceLevelLoad;
            flight.origin.EnqueueFlight(flight, PlanePrefab);
        }
    }
}
