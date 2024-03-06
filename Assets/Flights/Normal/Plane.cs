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
    [SerializeField] protected float Speed;

    //Per plane data.
    public FlightPlan Plan { get; protected set; }
    public bool HasDeparted { get; protected set; } = false;
    public bool HasCrashed {get; protected set; } = false;

    //The actual time the plane departed, in case this is different from the planned time.
    public double PreciseDepartureTime { get; protected set; }
    public double CrashTime {get; protected set; }

    //All unfriendly planes are aberrant, but not all aberrant planes are unfriendly. The aberration could be an honest mistake!
    //Rule of thumb: You should only mark a plane as !IsFriendly if it would make sense to intercept it with a fighter jet.
    //So e.g. hijacked plane: IsFriendly => false
    //but plane where pilot is an idiot: IsFriendly => true.
    public virtual bool IsAberrant => false;
    public virtual bool IsFriendly => true; 

    /// <summary>
    /// Call this function when the plane is ready to leave. It will set up the necessary values.
    /// Attempting to move a plane without departing first is undefined behaviour.
    /// </summary>
    /// <param name="plan">The plan we must follow when moving.</param>
    public virtual void Depart(FlightPlan plan)
    {
        Plan = plan;
        HasDeparted = true;
        PreciseDepartureTime = Time.timeSinceLevelLoadAsDouble;
    }

    void Update()
    {
        if (HasDeparted && !HasCrashed)
            Move();
    }

    /// <summary>
    /// Takes the plane to its next position. 
    /// Also lands or crashes the plane when appropriate.
    /// Do not call move on a plane that has not departed, or that has crashed.
    /// </summary>
    protected virtual void Move()
    {
        float timeTaken = (float)(Time.timeSinceLevelLoadAsDouble - PreciseDepartureTime);
        float dist = timeTaken * Speed;

        transform.SetPositionAndRotation(Plan.PointAtDistance(dist), Plan.RotationAtDistance(dist));
        if (Plan.ShouldLand(dist))
            Land();
    }

    /// <summary>
    /// Safely lands the plane at its current planned destination.
    /// This takes the plane out of the game, as it is no longer the player's responsibility.
    /// </summary>
    protected void Land()
    {
        Score.Add(10, "Landed", transform.position);
        Destroy(gameObject);
    }

    protected void Crash(int additionalScoreSubtraction = 0)
    {
        //TODO: Big explosion, Kabloom!, people scream, etc. etc.
        Score.Subtract(20 + additionalScoreSubtraction, "Crashed!", transform.position);
        HasCrashed = true;
        CrashTime = Time.timeSinceLevelLoadAsDouble;
    }
}
