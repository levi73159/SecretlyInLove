using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For exiting the game (useful for editor and standalone builds)

public class RandomNumberCheck : MonoBehaviour
{
    public GameObject targetObject; // Assign the GameObject to enable in the Inspector

    void Start()
    {
        // Generate a random number between 0 and 100
        int randomNumber = Random.Range(0, 101);
        Debug.Log("Generated Random Number: " + randomNumber);

        // Check if the number is within the range 20 to 30 (inclusive)
        if (randomNumber >= 20 && randomNumber <= 23)
        {
            Debug.Log("Number is within range. Enabling object and closing game.");

            if (targetObject != null)
            {
                targetObject.SetActive(true); // Enable the GameObject
            }

            // Call the CloseGame function after 5 seconds
            Invoke("CloseGame", 5f);
        }
        else
        {
            Debug.Log("Number is out of range. No action taken.");
        }
    }

    void CloseGame()
    {
#if UNITY_EDITOR
        // Exit play mode in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Close the application in standalone builds
        Application.Quit();
#endif
    }
}
