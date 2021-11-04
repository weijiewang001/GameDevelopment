using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauseExplosion : MonoBehaviour {
    public float explosionRadius = 2.0f;
    public float explosionForce = 1000.0f;
    public float explosionLift = 10.0f;

    private List<Rigidbody> explosionTargets;
    private SphereCollider sphereCollider;

    private void Start() {
        sphereCollider = GetComponent<SphereCollider>();
        explosionTargets = new List<Rigidbody>();
    }

    private void Update() {
        if (sphereCollider.radius != explosionRadius)
            sphereCollider.radius = explosionRadius;

        if (Input.GetKeyDown(KeyCode.Space))             
            foreach (Rigidbody r in explosionTargets)
                r.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionLift);
    }

    private void OnTriggerEnter(Collider other) {
        explosionTargets.Add(other.GetComponent<Rigidbody>());
    }

    private void OnTriggerExit(Collider other) {
        explosionTargets.Remove(other.GetComponent<Rigidbody>());
    }

}
