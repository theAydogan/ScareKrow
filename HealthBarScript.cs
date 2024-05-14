using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script handles the health bar in the game.
// it adjusts the size of the health bar based on a provided "normalSize" value.
// the health bar consists of a transform with a child named "Bar" whose scale is modified to represent the health level.
// if the "Bar" reference is not found, a warning is logged.


public class HealthBarScript : MonoBehaviour
//I, Ahmet Aydogan, 000792453 certify that this material is my original work.No other person's work has been used without due acknowledgement.
{
    // reference to the transform of the bar
    private Transform bar;

    void Start()
    {
        // find the child transform named "Bar"
        bar = transform.Find("Bar");

        // check if the bar reference is not null
        if (bar != null)
        {
            // set the initial scale of the bar to full (1, 1)
            bar.localScale = new Vector3(1f, 1f);
        }
    }

    // method to set the size of the health bar
    public void SetSize(float normalSize)
    {
        // check if the bar reference is not null
        if (bar != null)
        {
            // set the scale of the bar based on the provided "normalSize" value
            bar.localScale = new Vector3(normalSize, 1f);
        }
        else
        {
            // log a warning if the bar reference is null, these debug logs are more for me to see what is going on in the unity UI
            Debug.LogWarning("Health bar reference is null.");
        }
    }
}
