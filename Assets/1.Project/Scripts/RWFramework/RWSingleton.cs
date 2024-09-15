using UnityEngine;


public class RWSingleton<T> : MonoBehaviour where T : RWSingleton<T>
{
    private static T instance = default(T);

    static public T Instance
    {
        get
        {
            if (null == RWSingleton<T>.instance)
            {
                if (null == (RWSingleton<T>.instance = GameObject.FindObjectOfType(typeof(T)) as T))
                {
                    string name = typeof(T).ToString();
                    GameObject singletonGameObject = new GameObject($"{name}");

                    RWSingleton<T>.instance = singletonGameObject.AddComponent<T>();
                }
            }
            return RWSingleton<T>.instance;
        }
    }

    public virtual void Awake()
    {
        if (null != RWSingleton<T>.instance)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
    }
}