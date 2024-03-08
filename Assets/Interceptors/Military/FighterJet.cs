using UnityEngine;

/// <summary>
/// A type of interceptor that takes down unfriendly planes, preventing score loss.
/// </summary>
/// <remarks>
/// When deployed against an unfriendly plane, it destroys the target.
/// When deployed against another plane it you get a hefty score penalty for scaring the shit out of them,
/// and it prevents the target from continuing their flight.
/// We assume that the friendly plane will at least cooperate and not get shot, so that's why the score penalty
/// isn't as big as for losing a plane.
/// </remarks>
public class FighterJet : Interceptor
{
    protected override void Intercept(Plane plane)
    {
        if (plane.IsFriendly)
            Score.Subtract(10, "Unnecessary!", transform.position);
        else
            manager.OnKill.Invoke(plane);

        //TODO: a little animation that shows what happened.
        Destroy(plane.gameObject);
    }
}
