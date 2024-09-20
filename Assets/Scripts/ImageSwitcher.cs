using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageSwitcher : MonoBehaviour
{
    public Image[] imageComponents;
    public Sprite[] sprites; // [0] = Broken Pig, [1] = Coin, [2] = Golden Bar, [3] = Treasure
    public double balance;
    public TextMeshProUGUI balanceText;
    public double bet;

    public LineRenderer linePrefab; // Prefab for LineRenderer
    public List<LineRenderer> activeLines = new List<LineRenderer>(); // Keep track of active lines

    // Updated probabilities with the Broken Pig as the most common element
    private int[] probabilities = { 40, 30, 20, 10 }; // Broken Pig (40%), Coin (30%), Golden Bar (20%), Treasure Chest (10%)

    void Start()
    {
        balance = 150;
        bet = 5;

        balanceText.text = "Saldo: R$" + balance;

        // Initialize all image components with the broken pig (neutral sprite)
        for (int i = 0; i < imageComponents.Length; i++)
        {
            imageComponents[i].sprite = sprites[0]; // Set to the broken pig initially
        }

        // Clear any existing lines at the start
        ClearActiveLines();
    }

    public void Spin()
    {
        bool hasBalance = checkBalance();
        if(hasBalance)
        {
            updateBalance(-bet);

        // Set random sprites based on weighted probabilities
        for (int i = 0; i < imageComponents.Length; i++)
        {
            imageComponents[i].sprite = GetRandomWeightedSprite();
        }

        // Clear any active lines before checking for winnings
        ClearActiveLines();

        checkLines(); // Check for matching rows
        } else
        {
            balanceText.text = "Perdeu tudo mané";
        }
        
    }

    private bool checkBalance()
    {
        return balance > bet;
    }

    // Random selection of sprites based on weighted probabilities
    private Sprite GetRandomWeightedSprite()
    {
        int randomValue = Random.Range(0, 100);
        if (randomValue < probabilities[0])  // 0-49: Broken Pig
            return sprites[0];
        else if (randomValue < probabilities[0] + probabilities[1])  // 50-79: Coin
            return sprites[1];
        else if (randomValue < probabilities[0] + probabilities[1] + probabilities[2])  // 80-94: Golden Bar
            return sprites[2];
        else  // 95-99: Treasure Chest
            return sprites[3];
    }

    private void checkLines()
    {
        // Check if all images in a row are the same
        for (int i = 0; i < imageComponents.Length; i += 3)
        {
            if (imageComponents[i].sprite == imageComponents[i + 1].sprite &&
                imageComponents[i + 1].sprite == imageComponents[i + 2].sprite)
            {
                // Only show the line for actual winnings (Coin, Golden Bar, Treasure Chest)
                if (imageComponents[i].sprite != sprites[0]) // Not the broken pig
                {
                    // Create a new LineRenderer for the winning row
                    LineRenderer winningLine = Instantiate(linePrefab, transform);
                    activeLines.Add(winningLine); // Track the active lines
                    
                    // Get the middle of the first and last elements
                    Vector3[] startCorners = new Vector3[4];
                    imageComponents[i].rectTransform.GetWorldCorners(startCorners);
                    Vector3 startPos = (startCorners[0] + startCorners[1]) / 2; // Middle of the first element

                    Vector3[] endCorners = new Vector3[4];
                    imageComponents[i + 2].rectTransform.GetWorldCorners(endCorners);
                    Vector3 endPos = (endCorners[2] + endCorners[3]) / 2; // Middle of the last element

                    // Adjust Z position to ensure the line appears in front of the elements
                    startPos.z = -1;
                    endPos.z = -1;

                    // Set positions for the LineRenderer
                    winningLine.SetPosition(0, startPos); // Start point
                    winningLine.SetPosition(1, endPos);   // End point

                    // Apply rewards based on matching sprite
                    if (imageComponents[i].sprite == sprites[1]) // Coin
                    {
                        updateBalance(bet * 3);
                        Debug.Log("You earned: R$" + bet * 3);
                    }
                    else if (imageComponents[i].sprite == sprites[2]) // Golden Bar
                    {
                        updateBalance(bet * 5);
                        Debug.Log("You earned: R$" + bet * 5);
                    }
                    else if (imageComponents[i].sprite == sprites[3]) // Treasure Chest
                    {
                        updateBalance(bet * 10);
                        Debug.Log("You earned: R$" + bet * 10);
                    }
                }
                else
                {
                    Debug.Log("No winnings: Broken Pig row.");
                }
            }
        }
    }

    // Clear all active lines before a new spin
    private void ClearActiveLines()
    {
        foreach (LineRenderer line in activeLines)
        {
            Destroy(line.gameObject); // Destroy the old line renderers
        }
        activeLines.Clear();
    }

    private void updateBalance(double profitOrLoss)
    {
        balance += profitOrLoss;
        balanceText.text = "Balance: R$" + balance;
        Debug.Log("Current balance: R$" + balance);
    }
}