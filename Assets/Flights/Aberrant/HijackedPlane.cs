using UnityEngine;

public class HijackedPlane : Plane
{
    [Header("Timings")]
    [SerializeField] [Range(0, 1)] float MinMalfunctionStart; //As a fraction of the plan length.
    [SerializeField] [Range(0, 1)] float MaxMalfunctionStart; //As a fraction of the plan length.

    [Header("Malfunction Properties")]
    [SerializeField] float MinDistToHit;


    float malfunctionStart;
    bool malfunctionStarted = false;
    Transform target;

    public override bool IsAberrant => true;
    public override bool IsFriendly => false;
    public override void Depart(FlightPlan plan)
    {
        base.Depart(plan);
        malfunctionStart = Random.Range(MinMalfunctionStart, MaxMalfunctionStart) * plan.Length();
    }
    protected override void Move()
    {
        float timeTaken = (float)(Time.timeSinceLevelLoadAsDouble - PreciseDepartureTime);
        float dist = timeTaken * Speed;

        if (dist < malfunctionStart)
            base.Move();
        else if (target == null)
        {
            target = GameObject.FindWithTag("HijackerTarget").transform;
            if (TooCloseToTargetSoItFeelsUnfair())
            {
                target = null; //Try again next frame.
                base.Move();
            }
            else if (!malfunctionStarted)
            {
                malfunctionStarted = true;
                OnAberrate.Invoke();
            }
        }
        else
        {
            Vector3 dir = target.position - transform.position;
            transform.position += Speed * Time.deltaTime * dir.normalized;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

            if (dir.magnitude < MinDistToHit)
                Hit();
        }
    }

    void Hit()
    {
        //TODO: Animations, VFX, etc.
        Destroy(target.gameObject);
        Crash(20);
        Destroy(gameObject);
    }

    bool TooCloseToTargetSoItFeelsUnfair()
    {
        return (target.position - transform.position).sqrMagnitude < 50;
    }
}
