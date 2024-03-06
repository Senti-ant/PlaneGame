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
    [SerializeField] GameObject NormalPlanePrefab;
    [SerializeField] GameObject[] AberrantPlanePrefabs;
    [SerializeField] Airport[] AvailableForSimplePaths;
    [SerializeField] PathInformation[] CustomPaths;

    [Header("Spawn Parameters")]
    [SerializeField] int NumNormalFlights;

    //Note: Which aberrant flights are spawned is random, since otherwise the player could 'cheat' using meta-info about
    //the level. However, if you want to influence the odds a hacky way to do that is to just put the same aberrant prefab
    //in the aberrant prefabs list multiple times.
    [SerializeField] int NumAberrantFlights;
    [SerializeField] float EarliestTime;
    [SerializeField] float LatestTime;
    [SerializeField] float MinTimeBetweenSpawns;
    [SerializeField] [Range(0, 1)] float CustomPathChance;


    Departure[] plannedFlights;

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

        plannedFlights = new Departure[NumNormalFlights + NumAberrantFlights];
        for (int i = 0; i < NumNormalFlights; i++)
            plannedFlights[i] = new Departure(MakePlan(airports), NormalPlanePrefab);
        for (int i = 0; i < NumAberrantFlights; i++)
        {
            GameObject randomPrefab = AberrantPlanePrefabs[UnityEngine.Random.Range(0, AberrantPlanePrefabs.Length)];
            plannedFlights[i + NumNormalFlights] = new Departure(MakePlan(airports), randomPrefab);
        }

        Array.Sort(plannedFlights, (f1, f2) => f1.plan.departureTime.CompareTo(f2.plan.departureTime));
        StartCoroutine(SpawnFlights());
    }

    IEnumerator SpawnFlights()
    {
        float lastTime = 0;
        foreach (Departure flight in plannedFlights)
        {
            yield return new WaitUntil(() => Time.timeSinceLevelLoad >= flight.plan.departureTime
                                          && Time.timeSinceLevelLoad >= lastTime + MinTimeBetweenSpawns);

            lastTime = Time.timeSinceLevelLoad;
            flight.plan.origin.EnqueueFlight(flight);
        }
    }

    FlightPlan MakePlan(Airport[] airports)
    {
        float r = UnityEngine.Random.Range(0f, 1f);
        return (r < CustomPathChance && CustomPaths.Length > 0) ? 
                FlightPlan.Random(CustomPaths, EarliestTime, LatestTime) :
                FlightPlan.Random(airports, EarliestTime, LatestTime);
    }
}
