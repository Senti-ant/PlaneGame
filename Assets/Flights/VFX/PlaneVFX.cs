using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Plane))]
public class PlaneVFX : MonoBehaviour
{
    [SerializeField] GameObject CrashFXPrefab;
    [SerializeField] GameObject LandingFXPrefab;
    Plane plane;
    void Start()
    {
        plane = GetComponent<Plane>();
        plane.OnCrash.AddListener(OnCrash);
        plane.OnLand.AddListener(OnLand);
    }

    void OnLand()
    {
        GameObject fx = Instantiate(LandingFXPrefab, transform.position, Quaternion.identity);
        Destroy(fx, 5);
    }

    void OnCrash() => Instantiate(CrashFXPrefab, transform.position, Quaternion.identity, transform);
}
