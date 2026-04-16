using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool monstersKilled = false;

    private static GameManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}