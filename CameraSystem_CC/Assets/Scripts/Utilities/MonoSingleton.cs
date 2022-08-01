using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public bool global = true;
    static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType<T>();
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (global)
        {
            if (instance != null && instance != this.gameObject.GetComponent<T>())
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject.GetComponent<T>();
        }
    }
}
