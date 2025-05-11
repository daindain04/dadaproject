using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager instance;

    private int coins;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("รั ฤฺภฮ: " + coins);
    }
}
