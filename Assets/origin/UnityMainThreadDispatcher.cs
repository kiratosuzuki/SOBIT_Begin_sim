using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<IEnumerator> _executionQueue = new Queue<IEnumerator>();
    private static UnityMainThreadDispatcher _instance;

    public static UnityMainThreadDispatcher Instance()
    {
        if (!_instance)
        {
            _instance = new GameObject("MainThreadDispatcher").AddComponent<UnityMainThreadDispatcher>();
        }
        return _instance;
    }

    public void Enqueue(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                StartCoroutine(_executionQueue.Dequeue());
            }
        }
    }
}
