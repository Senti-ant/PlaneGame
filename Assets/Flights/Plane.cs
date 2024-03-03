using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

/// <summary>
/// Represents a plane within the game world.
/// In its vanilla form, it is a normal plane that perfectly follows its flight plan.
/// Aberrant planes should derive from this class and override this default behaviour.
/// </summary>
/// <remarks>
/// A prefab for a plane only needs to have the per prefab data filled in.
/// Per-plane/per-flight data will be filled in upon departure.
/// </remarks>
public class Plane : MonoBehaviour
{
    //Per prefab data.
    [SerializeField] float Speed;

    //Per plane data.
    public FlightPlan Plan { get; protected set; }
    public bool HasDeparted { get; protected set; } = false;

    //The actual time the plane departed, in case this is different from the planned time.
    public double PreciseDepartureTime { get; protected set; }

    public void Depart(FlightPlan plan)
    {
        Plan = plan;
        HasDeparted = true;
        PreciseDepartureTime = Time.timeSinceLevelLoadAsDouble;
    }

    void Update()
    {
        if (!HasDeparted)
            return;

        Move();
    }

    /// <summary>
    /// Implement the logic for how the plane moves throughout the scene here.
    /// This includes physically moving the object, but also thinngs like landing and crashing the plane.
    /// </summary>
    protected virtual void Move()
    {
        float timeTaken = (float)(Time.timeSinceLevelLoadAsDouble - PreciseDepartureTime);
        float dist = timeTaken * Speed;

        transform.position = Plan.PointAtDistance(dist);
        transform.rotation = Plan.RotationAtDistance(dist);

        if (Plan.ShouldLand(dist))
            Land();
    }

    /// <summary>
    /// Safely lands the plane at its current planned destination.
    /// </summary>
    protected void Land()
    {
        Score.Add(10);
        Destroy(gameObject);
    }
}
