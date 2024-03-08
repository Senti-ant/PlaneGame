using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A type of interceptor that recovers crashed planes and helps the survivors.
/// </summary>
/// <remarks>
/// When it hits a crashed plane, it recuperates some of the lost score.
/// When it hits an unfriendly plane, it dies.
/// When it hits a normal plane, it interrupts the flight, which costs score and prevents the plane from flying on.
/// </remarks>
public class SearchAndRescueInterceptor : Interceptor
{
    //Longest time after a crash where the search and rescue plane can still help.
    //(beyond this we can assume that everyone is either dead or saved themselves)
    [SerializeField] float MaxTimeAfterCrash; 

    protected override void Intercept(Plane plane)
    {
         //Unnecessary intercept costs score (else you could just click everything!)
        if (!plane.IsAberrant || !plane.IsFriendly)
            Score.Subtract(5, "Unnecessary!", transform.position);
        else
        {
            float timeSinceCrash = plane.HasCrashed ? (float)(Time.timeSinceLevelLoadAsDouble - plane.CrashTime)
                                                    : 0f;
            decimal score = Mathf.Clamp01(timeSinceCrash / MaxTimeAfterCrash) switch 
            {
                >= 1f       =>  0m,
                >= 0.8f     =>  2.5m,
                >= 0.6f     =>  5m,
                >= 0.4f     =>  7.5m,
                >= 0.2f     =>  10m,
                > 0f        =>  15m,
                <= 0f       =>  0m, //It never crashed, so there is no score to recuperate.
                                    //This can still save a plane from crashing all-together!
            };
            Score.Add(score, "Saved people :)", transform.position);
            manager.OnRescue.Invoke(plane);
        }

        if (plane.IsFriendly)
            //In-universe it does not get destroyed, but is recovered or something.
            Destroy(plane.gameObject);
    }
}
