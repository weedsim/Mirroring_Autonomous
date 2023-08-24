using Fusion;
using UnityEngine;

public abstract class Singleton<T> : SimulationBehaviour where T : SimulationBehaviour
{
    private static T instance;

    public static T Instance

    {

        get

        {

            if (!instance)

            {

                GameObject obj;

                obj = GameObject.Find(typeof(T).Name);

                if (!obj)

                {

                    obj = new GameObject(typeof(T).Name);

                    instance = obj.AddComponent<T>();

                }

                else

                {

                    instance = obj.GetComponent<T>();

                }

            }

            return instance;

        }

    }



    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    public abstract void Initialize();


}
