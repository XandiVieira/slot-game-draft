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

    public int numberOfRows = 3; // Adjust the number of rows
    public int numberOfColumns = 3; // Adjust the number of columns

    // Updated probabilities with the Broken Pig as the most common element
    private int[] probabilities = { 10, 40, 30, 20 }; // Broken Pig (40%), Coin (30%), Golden Bar (20%), Treasure Chest (10%)

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
        if (hasBalance)
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
        }
        else
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
        if (randomValue < probabilities[0])  // 0-39: Broken Pig
            return sprites[0];
        else if (randomValue < probabilities[0] + probabilities[1])  // 40-69: Coin
            return sprites[1];
        else if (randomValue < probabilities[0] + probabilities[1] + probabilities[2])  // 70-89: Golden Bar
            return sprites[2];
        else  // 90-99: Treasure Chest
            return sprites[3];
    }

    private void checkLines()
    {
        // Check all rows, columns, and diagonals
        CheckRows();
        CheckDiagonals();
    }

    // Check rows
    private void CheckRows()
    {
        for (int row = 0; row < numberOfRows; row++)
        {
            int startIndex = row * numberOfColumns;
            int[] rowIndices = new int[numberOfColumns];

            // Collect the indices for this row
            for (int col = 0; col < numberOfColumns; col++)
            {
                rowIndices[col] = startIndex + col;
            }

            CheckLine(rowIndices); // Check if this row has a winning combination, horizontal = true
        }
    }

    // Check diagonals
private void CheckDiagonals()
{
    // Diagonal from top-left to bottom-right
    int[] diagonal1 = { 0, 4, 8 };  // First row, left to right
    int[] diagonal2 = { 2, 4, 6 };  // First row, right to left
    
    // Diagonals that start from second row
    int[] diagonal3 = { 3, 7, 11 }; // Second row, left to right
    int[] diagonal4 = { 5, 7, 9 };  // Second row, right to left

    // Check these four diagonals for wins
    CheckLine(diagonal1);
    CheckLine(diagonal2);
    CheckLine(diagonal3);
    CheckLine(diagonal4);
}


    // Helper method to check a line (row, or diagonal)
    private void CheckLine(int[] indices)
    {
        if (indices.Length < 2) return; // Need at least 2 elements to form a line

        Sprite firstSprite = imageComponents[indices[0]].sprite;

        // Only proceed if the first sprite isn't the broken pig
        if (firstSprite == sprites[0]) return;

        // Check if all sprites in the line are the same
        for (int i = 1; i < indices.Length; i++)
        {
            if (imageComponents[indices[i]].sprite != firstSprite)
            {
                return; // Not a winning line
            }
        }

        // If we reached here, it's a winning line
        CreateWinningLine(indices); // Draw the line

        // Apply rewards based on the matching sprite
        ApplyReward(firstSprite);
    }

    private void CreateWinningLine(int[] indices)
{
    // Create a new LineRenderer for this winning line
    LineRenderer winningLine = Instantiate(linePrefab, transform);
    activeLines.Add(winningLine); // Track the active lines

    // Get the corners of the first element in the line
    Vector3[] startCorners = new Vector3[4];
    imageComponents[indices[0]].rectTransform.GetWorldCorners(startCorners);
    Vector3 startPos;

    Vector3[] endCorners = new Vector3[4];
    imageComponents[indices[indices.Length - 1]].rectTransform.GetWorldCorners(endCorners);
    Vector3 endPos;

    // Adjust Z position to ensure the line appears in front of the elements
    startPos.z = -1;
    endPos.z = -1;

    // Determine if it's a horizontal line
    if (indices[0] / numberOfColumns == indices[indices.Length - 1] / numberOfColumns)
    {
        // Horizontal line: Start from the middle-left of the first element to the middle-right of the last
        startPos = (startCorners[0] + startCorners[1]) / 2; // Middle of the left side of the first element
        endPos = (endCorners[2] + endCorners[3]) / 2; // Middle of the right side of the last element
    }
    else
    {
        // Diagonal line: Top-left to bottom-right or top-right to bottom-left
        if (indices[0] % numberOfColumns == 0) // Top-left to bottom-right diagonal
        {
            startPos = startCorners[1]; // Top-left corner of the first element
            endPos = endCorners[3]; // Bottom-right corner of the last element
        }
        else // Top-right to bottom-left diagonal
        {
            startPos = startCorners[2]; // Top-right corner of the first element
            endPos = endCorners[0]; // Bottom-left corner of the last element
        }
    }

    // Set positions for the LineRenderer
    winningLine.SetPosition(0, startPos); // Start point
    winningLine.SetPosition(1, endPos);   // End point
}

    // Applies the reward based on the matching sprite
    private void ApplyReward(Sprite winningSprite)
    {
        if (winningSprite == sprites[1]) // Coin
        {
            Debug.Log("You earned: R$" + bet * 3);
            updateBalance(bet * 3);
        }
        else if (winningSprite == sprites[2]) // Golden Bar
        {
            Debug.Log("You earned: R$" + bet * 5);
            updateBalance(bet * 5);
        }
        else if (winningSprite == sprites[3]) // Treasure Chest
        {
            Debug.Log("You earned: R$" + bet * 10);
            updateBalance(bet * 10);
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