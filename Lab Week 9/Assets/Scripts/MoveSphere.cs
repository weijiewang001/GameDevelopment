using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSphere : MonoBehaviour {
    public Tweener tweener;

    Vector3 target = new Vector3(-3.0f, 1.0f, 0.0f);
    float duration = 1.5f;

	void Update () {
		if (tweener != null) {
            if (!tweener.TweenExists(transform)) {
                target.x = -target.x;
                tweener.AddTween(transform, transform.position, target, duration / SpeedManager.SpeedModifier);
            }
        }
	}
}
