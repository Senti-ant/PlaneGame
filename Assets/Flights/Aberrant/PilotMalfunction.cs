using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This represents a plane where something is wrong with the pilot.
/// Perhaps they are sick, incapacitated, or let their young child take the wheel (that actually happened once).
/// </summary>
public class PilotMalfunction : Plane
{
    //Per-prefab data.
    [Header("Timings")]
    [SerializeField] [Range(0, 1)] float MinMalfunctionStart; //As a fraction of the plan length.
    [SerializeField] [Range(0, 1)] float MaxMalfunctionStart; //As a fraction of the plan length.
    [SerializeField] float MinMalfunctionLength; //As a world distance.
    [SerializeField] float MaxMalfunctionLength; //As a world distance.

    [Header("Malfunction Properties")]
    [SerializeField] float NoiseSize;
    [SerializeField] float NoiseZoom;
    [SerializeField] [Min(1)] int NoiseOctaves;
    [SerializeField] [Range(0, 1)] float NoisePersistance;
    [SerializeField] [Min(1)] float NoiseLacunarity;


    //Per-plane data.
    float noiseSeed;
    float malfunctionStart;
    float malfunctionEnd; //Not a happy ending.


    public override bool IsAberrant => true;
    public override bool IsFriendly => true;
    public override void Depart(FlightPlan plan)
    {
        base.Depart(plan);
        malfunctionStart = Random.Range(MinMalfunctionStart, MaxMalfunctionStart) * plan.Length();
        malfunctionEnd = malfunctionStart + Random.Range(MinMalfunctionLength, MaxMalfunctionLength);
        //Debug.Log($"Will start at {malfunctionStart} and end at {malfunctionEnd}");
        noiseSeed = Random.Range(-500f, 500f);
    }
    protected override void Move()
    {
        float timeTaken = (float)(Time.timeSinceLevelLoadAsDouble - PreciseDepartureTime);
        float dist = timeTaken * Speed;

        transform.position = PointAtDistance(dist);
        transform.rotation = RotationAtDistance(dist);

        if (dist > malfunctionEnd)
            Crash();
    }

    Vector2 PointAtDistance(float dist)
    {
        Vector2 plannedPos = Plan.PointAtDistance(dist);
        if (dist < malfunctionStart)
        {
            //Debug.Log($"At distance {dist} there is no noise yet.");
            return plannedPos;
        }

        float frequency = NoiseZoom;
        float amplitude = NoiseSize * Mathf.InverseLerp(malfunctionStart, malfunctionEnd, dist);
        //Debug.Log($"At distance {dist} amplitude is {amplitude}");
        float noiseVal = 0;
        for (int i = 0; i < NoiseOctaves; i++)
        {
            noiseVal += (Mathf.PerlinNoise(noiseSeed + dist * frequency, 0.5f) * 2f - 1f) * amplitude;
            amplitude *= NoisePersistance;
            frequency *= NoiseLacunarity;
        }
        
        Vector2 tangent = Plan.NormalAtDistance(dist);
        return plannedPos + tangent * noiseVal;
    }

    Quaternion RotationAtDistance(float dist)
    {
        Vector2 pos = PointAtDistance(dist);
        Vector2 nextPos = PointAtDistance(dist + 0.2f);
        return Quaternion.LookRotation(Vector3.forward, nextPos - pos);
    }
}
