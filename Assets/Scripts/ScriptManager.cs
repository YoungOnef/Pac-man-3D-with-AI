using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    public int totalCoins;
    public int collectedCoins;
    public TMP_Text coinText;

    void Start()
    {
        collectedCoins = 0;
        totalCoins = GameObject.FindObjectsOfType<Coin>().Length;
        print("Total coins: " + totalCoins);
        UpdateCoinText();
    }

    public void IncrementCoinCount()
    {
        collectedCoins++;
        UpdateCoinText();

        if (collectedCoins == totalCoins)
        {
            // Player has collected all coins, handle win condition
            // e.g. display win message, play win sound effect, etc.
            print("You win!");
        }
    }

    void UpdateCoinText()
    {
        coinText.text = "Coins: " + collectedCoins + "/" + totalCoins;
    }
}
