using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _actions = new Queue<Action>();

    public static UnityMainThreadDispatcher Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Enqueue(Action action)
    {
        lock (_actions)
        {
            _actions.Enqueue(action);
        }
    }

    void Update()
    {
        lock (_actions)
        {
            while (_actions.Count > 0)
            {
                _actions.Dequeue().Invoke();
            }
        }
    }
}