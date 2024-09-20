using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BetSelector : MonoBehaviour
{
    public Button[] betButtons; // Assign buttons for each bet (0.2, 1, 5, 10, 25, 50)
    public double[] betValues = { 0.2, 1, 5, 10, 25, 50 }; // Corresponding bet values
    public Color selectedColor = Color.green; // Color for the selected button
    public Color defaultColor = Color.white; // Default color for unselected buttons
    private double currentBet;

    void Start()
    {
        // Set initial bet to the first button value
        SetBet(betValues[0]);

        // Assign button click events
        for (int i = 0; i < betButtons.Length; i++)
        {
            int index = i; // Store index for use in the listener
            betButtons[i].onClick.AddListener(() => OnBetButtonClick(index));
        }
    }

    // Method triggered when a bet button is clicked
    public void OnBetButtonClick(int buttonIndex)
    {
        SetBet(betValues[buttonIndex]); // Set the new bet value
        UpdateButtonVisuals(buttonIndex); // Update button appearance
    }

    // Set the bet value based on the selected button
    private void SetBet(double betValue)
    {
        currentBet = betValue;
        betButtons[0].GetComponent<Image>().color = selectedColor;
        Debug.Log("Selected Bet: " + currentBet);
    }

    // Update the button appearance to reflect the selected one
    private void UpdateButtonVisuals(int selectedIndex)
    {
        for (int i = 0; i < betButtons.Length; i++)
        {
            // Highlight the selected button, reset others
            if (i == selectedIndex)
            {
                betButtons[i].GetComponent<Image>().color = selectedColor;
            }
            else
            {
                betButtons[i].GetComponent<Image>().color = defaultColor;
            }
        }
    }

    // Method to get the current bet value
    public double GetCurrentBet()
    {
        return currentBet;
    }
}