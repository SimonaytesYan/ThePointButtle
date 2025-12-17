using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private List<GameObject> checkpoints = new List<GameObject>();
    private const float gameTime = 12f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(GameOver), gameTime);
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
        TMP_Text game_over_text = gameOverUI.GetComponentInChildren<TMP_Text>();
        Cursor.lockState = CursorLockMode.None;

        int first_player_score = 0;
        int second_player_score = 0;
        foreach(GameObject checkpoint in checkpoints)
        {
            var checkpoint_results = checkpoint.GetComponent<Checkpoint>().GetPlayerScore();

            first_player_score += checkpoint_results.firstPlayerScore;
            second_player_score += checkpoint_results.secondPlayerScore;
        }
        
        if (first_player_score > second_player_score)
        {
            game_over_text.text = "First player win!!!";
            Debug.Log("First player win!");
        }
        else if (first_player_score < second_player_score)
        {
            game_over_text.text = "Second player win!!!";
            Debug.Log("Second player win!");
        }
        else
        {
            game_over_text.text = "It's a draw!";
            Debug.Log("It's a draw!");
        }
    }
}
