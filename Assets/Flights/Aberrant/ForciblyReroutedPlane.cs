using UnityEngine;

/// <summary>
/// This represents a plane with less evil hijackers than the hijacked plane.
/// They just want to force the pilot to go to a different (unplanned) destination.
/// </summary>
public class ForciblyReroutedPlane : Plane
{
    [Header("Timings")]
    [SerializeField] [Range(0, 1)] float MinMalfunctionStart; //As a fraction of the plan length.
    [SerializeField] [Range(0, 1)] float MaxMalfunctionStart; //As a fraction of the plan length.

    [Header("Malfunction Properties")]
    [SerializeField] float MinDistToHit;


    float malfunctionStart;
    Transform target;
    Airport[] potentialTargets;

    public override bool IsAberrant => true;
    public override bool IsFriendly => false;
    public override void Depart(FlightPlan plan)
    {
        base.Depart(plan);
        malfunctionStart = Random.Range(MinMalfunctionStart, MaxMalfunctionStart) * plan.Length();
        potentialTargets = FindObjectsOfType<Airport>();
    }
    protected override void Move()
    {
        float timeTaken = (float)(Time.timeSinceLevelLoadAsDouble - PreciseDepartureTime);
        float dist = timeTaken * Speed;

        if (dist < malfunctionStart)
            base.Move();
        //Only executed once since there is always at least one valid target available.
        //(At least, assuming you correctly set up the level to have more than two airports which aren't all bunched up).
        else if (target == null)
        {
            int i = Random.Range(0, potentialTargets.Length);
            while (potentialTargets[i] == Plan.destination || potentialTargets[i] == Plan.origin || TooClose(i))
                i = (i + 1) % potentialTargets.Length;
            
            target = potentialTargets[i].transform;
            OnAberrate.Invoke();
        }
        else
        {
            Vector3 dir = target.position - transform.position;
            transform.position += Speed * Time.deltaTime * dir.normalized;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

            if (dir.magnitude < MinDistToHit)
                LandIllegally();
        }
    }

    void LandIllegally()
    {
        //TODO: Animations, VFX, etc.
        Score.Subtract(15, "Wrong airport!!", transform.position);
        Destroy(gameObject);
    }

    bool TooClose(int i)
    {
        return (potentialTargets[i].transform.position - transform.position).sqrMagnitude < 50;
    }
}

