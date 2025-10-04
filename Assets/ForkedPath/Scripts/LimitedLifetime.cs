using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class LimitedLifetime : MonoBehaviour
{
    public float lifetime = 5f;
    SimpleTimer timer;

    private void OnEnable()
    {
        if (timer == null)
        {
            timer = new SimpleTimer(lifetime);
        }
    }

    private void Update()
    {
        if (timer != null && timer.tick(Time.deltaTime))
        {
            Destroy(gameObject);
            timer = null;
        }
    }
}