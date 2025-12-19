using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    // [SerializeField] private GameObject inGameUI;
    [SerializeField] private TMP_Text player1Score;
    [SerializeField] private TMP_Text player2Score;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private List<GameObject> checkpoints = new List<GameObject>();
    private const float gameTime = 200f;

    private NetworkVariable<float> currentTime =
        new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<int> player1ScoreNet =
        new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);

    private NetworkVariable<int> player2ScoreNet =
        new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);

    private bool gameFinished = false;
    private bool isRunning = false;

    public override void OnNetworkSpawn()
    {
        currentTime.OnValueChanged += (_, __) => UpdateTimerUI();
        player1ScoreNet.OnValueChanged += (_, __) => UpdateScoreUI();
        player2ScoreNet.OnValueChanged += (_, __) => UpdateScoreUI();

        UpdateTimerUI();
        UpdateScoreUI();

        if (IsServer)
        {
            currentTime.Value = gameTime;
        }
    }

    void Update()
    {
        if (!IsServer || gameFinished) return;

        currentTime.Value -= Time.deltaTime;

        if (currentTime.Value < 0)
        {
            currentTime.Value = 0;
            EndGame();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(currentTime.Value / 60);
        int seconds = Mathf.FloorToInt(currentTime.Value % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateScoreServerRpc()
    {
        if (!gameFinished)
        {
            var score = calcPlayerScore();

            player1ScoreNet.Value = score.first_player_score;
            player2ScoreNet.Value = score.second_player_score;
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

    private void EndGame()
    {
        if (gameFinished)
            return;

        gameFinished = true;

        GameOverClientRpc(
            player1ScoreNet.Value,
            player2ScoreNet.Value
        );
    }

    [ClientRpc]
    public void GameOverClientRpc(int p1, int p2)
    {
        gameFinished = true;
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
        TMP_Text game_over_text = gameOverUI.GetComponentInChildren<TMP_Text>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        var score = calcPlayerScore();

        if (p1 > p2)
        {
            game_over_text.text = "First player win!!!";
            Debug.Log("First player win!");
        }
        else if (p1 < p2)
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

    private void UpdateScoreUI()
    {
        player1Score.text = player1ScoreNet.Value.ToString();
        player2Score.text = player2ScoreNet.Value.ToString();
    }
}
