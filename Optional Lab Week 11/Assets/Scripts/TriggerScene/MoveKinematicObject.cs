using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveKinematicObject : MonoBehaviour {
    [SerializeField] private Tweener tweener;
    [SerializeField] private float moveDuration;

	void Update () {
        if (!tweener.TweenExists(transform)) {
            Vector3 targetPos = transform.position;
            targetPos.x = -targetPos.x;
            tweener.AddTween(transform, transform.position, targetPos, moveDuration);
        }
	}
}
