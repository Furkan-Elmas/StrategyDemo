using System.Collections;
using UnityEngine;

/// <summary>
/// Generic Singleton base class that takes any type.
/// </summary>
/// <typeparam name="T">Any type to be required as Singleton.</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// The singleton instance of stated Type.
    /// </summary>
    public static T Instance
    {
        get
        {
            lock (Lock)
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<T>();

                    if (instance == null)
                    {
                        var go = new GameObject($"[{typeof(T)}]");
                        instance = go.AddComponent<T>();
                    }

                }

                return instance;
            }
        }

    }

    private static readonly object Lock = new object();
    private static T instance;


    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
    }
}
