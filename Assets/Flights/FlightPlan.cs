using System;
using PathCreation;
using UnityEngine;

/// <summary>
/// Contains the time and location information pertaining to a flight.
/// Also has helper functions that tell you where you *should* be.
/// Whether to actually follow this is up to the controller of the plane.
/// </summary>
[Serializable]
public struct FlightPlan
{
    public float departureTime;
    public Airport origin;
    public Airport destination;
    public VertexPath intendedPath;

    public FlightPlan(float departureTime, Airport origin, Airport destination, VertexPath intendedPath = null)
    {
        this.departureTime = departureTime; this.origin = origin; this.destination = destination; 
        this.intendedPath = intendedPath;
    }

    /// <summary>
    /// Calculates where the plane should be at a specified distance (as opposed to time) into the flight.
    /// </summary>
    /// <param name="dist">The distance in unity coordinate units.</param>
    /// <returns>The planned world position of the plane.</returns>
    public Vector2 PointAtDistance(float dist)
    {
        if (intendedPath == null)
        {
            Vector2 start = origin.transform.position;
            Vector2 end = destination.transform.position;
            
            return Vector2.Lerp(start, end, dist / (end - start).magnitude);
        }
        return intendedPath.GetPointAtDistance(dist, EndOfPathInstruction.Stop);
    }

    /// <summary>
    /// Calculates where the plane should be pointed at a specified distance (as opposed to time) into the flight.
    /// </summary>
    /// <param name="dist">The distance in world coordinate units.</param>
    /// <returns>The planned rotation of the plane as a quaternion.</returns>
    public Quaternion RotationAtDistance(float dist)
    {
        if (intendedPath == null)
        {
            Vector2 dir = destination.transform.position - origin.transform.position;
            return Quaternion.LookRotation(Vector3.forward, dir);
        }

        return intendedPath.Get2DRotationAtDistance(dist, EndOfPathInstruction.Stop);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The length of the planned path in world coordinate units.</returns>
    public float Length()
    {
        if (intendedPath == null)
        {
            Vector2 start = origin.transform.position;
            Vector2 end = destination.transform.position;

            return (end - start).magnitude;
        }

        return intendedPath.length;
    }
    /// <summary>
    /// Determines whether the plane should land once it is a specified distance into the flight,
    /// assuming the plan has been followed thus far.
    /// </summary>
    /// <param name="dist">The distance in world coordinate units.</param>
    /// <returns>True => yes you should land; false => absolutely not.</returns>
    public bool ShouldLand(float dist) => dist >= Length();

    public void Show(LineRenderer showOn)
    {
        Vector3[] positions;
        if (intendedPath == null)
            positions = new Vector3[] { origin.transform.position, destination.transform.position };
        else
        {
            positions = new Vector3[intendedPath.NumPoints];
            for (int i = 0; i < intendedPath.NumPoints; i++)
                positions[i] = intendedPath.GetPoint(i);
        }

        showOn.positionCount = positions.Length;
        showOn.SetPositions(positions);
    }

    /// <summary>
    /// Generates a random flight plan by picking two random airports and planning to fly in a straight line between them.
    /// </summary>
    /// <param name="airports">Array of all airports that are available to be selected.</param>
    /// <param name="minTime">The earliest possible departure time in seconds since level load.</param>
    /// <param name="maxTime">The latest possible departure time in seconds since level load.</param>
    /// <returns>A randomly selected flight plan.</returns>
    public static FlightPlan Random(Airport[] airports, float minTime, float maxTime)
    {
        float time = UnityEngine.Random.Range(minTime, maxTime);

        int originI = UnityEngine.Random.Range(0, airports.Length);
        int destinationI = (originI + UnityEngine.Random.Range(1, airports.Length)) % airports.Length;

        return new FlightPlan(time, airports[originI], airports[destinationI]);
    }

    /// <summary>
    /// Generates a random flight plan by picking a random PathInformation and planning to follow that path.
    /// </summary>
    /// <remarks>
    /// This is intended to be used for custom paths that are not lines between two airports.
    /// If you want a plane to simply fly from one airport to another, use the other overload of FlightPlan.Random.
    /// </remarks>
    /// <param name="pathInfos">Array of all PathInformations for all paths available for selection.</param>
    /// <param name="minTime">The earliest possible departure time in seconds since level load.</param>
    /// <param name="maxTime">The latest possible departure time in seconds since level load.</param>
    /// <returns>A randomly selected flight plan.</returns>
    public static FlightPlan Random(PathInformation[] pathInfos, float minTime, float maxTime)
    {
        float time = UnityEngine.Random.Range(minTime, maxTime);
        PathInformation pathInfo = pathInfos[UnityEngine.Random.Range(0, pathInfos.Length)];

        return new FlightPlan(time, pathInfo.Origin, pathInfo.Destination, pathInfo.Path);
    }
}
