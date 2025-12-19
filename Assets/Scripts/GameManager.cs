using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject inGameUI;
    private TMP_Text player1Score;
    private TMP_Text player2Score;
    [SerializeField] private List<GameObject> checkpoints = new List<GameObject>();
    private const float gameTime = 60f;
    private bool gameFinished = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player1Score = inGameUI.transform.Find("Player1Score").gameObject.GetComponent<TMP_Text>();
        player2Score = inGameUI.transform.Find("Player2Score").gameObject.GetComponent<TMP_Text>();
        Invoke(nameof(GameOver), gameTime);
    }

    void Update()
    {
    }

    public void updatePlayerScore()
    {
        if (!gameFinished)
        {
            var score = calcPlayerScore();

            player1Score.text = score.first_player_score.ToString();
            player2Score.text = score.second_player_score.ToString();
            Debug.Log($"First player score: {player1Score.text}");
            Debug.Log($"Second player score: {player2Score.text}");
        }
    }

    (int first_player_score, int second_player_score) calcPlayerScore()
    {
        int first_player_score = 0;
        int second_player_score = 0;
        foreach (GameObject checkpoint in checkpoints)
        {
            var checkpoint_results = checkpoint.GetComponent<Checkpoint>().GetPlayerScore();

            first_player_score += checkpoint_results.firstPlayerScore;
            second_player_score += checkpoint_results.secondPlayerScore;
        }

        return (first_player_score, second_player_score);
    }

    public void GameOver()
    {
        gameFinished = true;
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
        TMP_Text game_over_text = gameOverUI.GetComponentInChildren<TMP_Text>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        var score = calcPlayerScore();

        if (score.first_player_score > score.second_player_score)
        {
            game_over_text.text = "First player win!!!";
            Debug.Log("First player win!");
        }
        else if (score.first_player_score < score.second_player_score)
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
