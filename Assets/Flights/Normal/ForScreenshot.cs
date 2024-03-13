using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForScreenshot : MonoBehaviour
{
    [SerializeField] float Circliness;
    [SerializeField] float Speediness;
    [SerializeField] Transform Target;

    // Update is called once per frame
    void Update()
    {
        transform.SetPositionAndRotation(NextPos(), NextRot());
    }

    Vector3 NextPos()
    {
        return transform.position + transform.up * (Time.deltaTime * Speediness);
    }

    Quaternion NextRot()
    {
        Vector3 dir = (Target.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, dir);
        return Quaternion.RotateTowards(transform.rotation, rot, Time.deltaTime * Circliness);
    }
}
