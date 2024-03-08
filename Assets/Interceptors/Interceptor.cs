using UnityEngine;

/// <summary>
/// An interceptor is a special vehicle dispatched by the player.
/// It goes to a plane, performs a certain action on that plane, and then disappears.
/// The airport it came from (Base) and the Target must be set from the outside.
/// </summary>
public abstract class Interceptor : MonoBehaviour
{
    [SerializeField] float Speed;
    [SerializeField] float MinDistToHit;
    [HideInInspector] public Plane Target { get; set; }
    [HideInInspector] public InterceptorManager manager { get; set; }

    void Update()
    {
        if (Target == null) //Somebody else got him already.
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = Target.transform.position - transform.position;
        transform.position += Speed * Time.deltaTime * dir.normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

        if (dir.magnitude < MinDistToHit)
        {
            Intercept(Target);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Performs the special action of this interceptor type upon the plane.
    /// Should also call any events on the manager as appropriate.
    /// </summary>
    /// <param name="plane">The plane to be intercepted.</param>
    protected abstract void Intercept(Plane plane);
}
