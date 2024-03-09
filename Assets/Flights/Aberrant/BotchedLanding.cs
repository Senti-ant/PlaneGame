using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a plane that had an accident while coming in for landing.
/// </summary>
public class BotchedLanding : Plane
{
    [SerializeField] float MinDistFromAirport;
    [SerializeField] float MaxDistFromAirport;
    public override bool IsFriendly => true;
    public override bool IsAberrant => true;
    float distFromAirport;

    public override void Depart(FlightPlan plan)
    {
        base.Depart(plan);
        distFromAirport = Random.Range(MinDistFromAirport, MaxDistFromAirport);
    }

    protected override void Move()
    {
        base.Move();
        if ((Plan.destination.transform.position - transform.position).magnitude <= distFromAirport)
        {
            OnAberrate.Invoke();
            Crash();
        }
    }
}
