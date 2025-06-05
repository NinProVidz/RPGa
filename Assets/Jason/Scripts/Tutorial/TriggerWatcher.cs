using UnityEngine;
using System;

public class TriggerWatcher : MonoBehaviour
{
    private Action onPlayerEnter;

    public void Initialize(Action callback)
    {
        onPlayerEnter = callback;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerEnter?.Invoke();
        }
    }
}
