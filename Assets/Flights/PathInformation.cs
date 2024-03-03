using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

/// <summary>
/// Stores the meta-information for a custom path.
/// It does not do anything by itself. It's purely a container for some data.
/// This is a monobehaviour just because I like putting them into my scenes as actual components.
/// </summary>
/// <remarks>
/// You do not have to make PathInformations for the simple straight-line-shaped flights from airport to airport.
/// Instead, use them for more advanced flight plans, like scenic tours.
/// </remarks>
[RequireComponent(typeof(PathCreator))]
public class PathInformation : MonoBehaviour
{
    public Airport Origin;
    public Airport Destination;
    [HideInInspector] public VertexPath Path;

    void Awake() => Path = GetComponent<PathCreator>().path;
}
