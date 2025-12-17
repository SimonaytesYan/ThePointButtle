using UnityEngine;
using Unity.Netcode;

public class Checkpoint : NetworkBehaviour
{
    enum CheckpointState
    {
        NoPlayerInside,
        FirstPlayerInside,
        SecondPlayerInside,
        BothPlayersInside
    }

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private GameObject gameManager;
    private CheckpointState checkpointState = CheckpointState.NoPlayerInside;

    int firstTeamScore = 0;
    int secondTeamScore = 0;

    int firstTeamPrescore = 0;
    int secondTeamPrescore = 0;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        updateScore();
    }

    private void updateScore()
    {
        if (checkpointState == CheckpointState.FirstPlayerInside)
        {
            firstTeamPrescore++;
            if (firstTeamPrescore % 50 == 0)
            {
                firstTeamPrescore = 0;
                firstTeamScore++;
                gameManager.GetComponent<GameManager>().updatePlayerScore();
            }
        }
        else
        {
            firstTeamPrescore = 0;
        }

        if (checkpointState == CheckpointState.SecondPlayerInside)
        {
            secondTeamPrescore++;
            if (secondTeamPrescore % 50 == 0)
            {
                secondTeamPrescore = 0;
                secondTeamScore++;
                gameManager.GetComponent<GameManager>().updatePlayerScore();
            }
        }
        else
        {
            secondTeamPrescore = 0;
        }
    }

    private void FirstPlayerEnter()
    {
        if (checkpointState == CheckpointState.NoPlayerInside)
            checkpointState = CheckpointState.FirstPlayerInside;
        else if (checkpointState == CheckpointState.SecondPlayerInside)
            checkpointState = CheckpointState.BothPlayersInside;
    }

    private void SecondPlayerEnter()
    {
        if (checkpointState == CheckpointState.NoPlayerInside)
            checkpointState = CheckpointState.SecondPlayerInside;
        else if (checkpointState == CheckpointState.FirstPlayerInside)
            checkpointState = CheckpointState.BothPlayersInside;
    }
    private void FirstPlayerExit()
    {
        if (checkpointState == CheckpointState.FirstPlayerInside)
            checkpointState = CheckpointState.NoPlayerInside;
        else if (checkpointState == CheckpointState.BothPlayersInside)
            checkpointState = CheckpointState.SecondPlayerInside;
    }

    private void SecondPlayerExit()
    {
        if (checkpointState == CheckpointState.SecondPlayerInside)
            checkpointState = CheckpointState.NoPlayerInside;
        else if (checkpointState == CheckpointState.BothPlayersInside)
            checkpointState = CheckpointState.FirstPlayerInside;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigget enter\n");
        PlayerNumber player_number_obj = null;
        if (other.gameObject.TryGetComponent<PlayerNumber>(out player_number_obj))
        {
            Debug.Log("Player enter\n");
            if (player_number_obj.getPlayerInd() == 1)
            {
                FirstPlayerEnter();
            }
            else if (player_number_obj.getPlayerInd() == 2)
            {
                SecondPlayerEnter();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigget exit\n");
        PlayerNumber player_number_obj = null;
        if (other.gameObject.TryGetComponent<PlayerNumber>(out player_number_obj))
        {
            Debug.Log("Player exit\n");
            if (other.gameObject.GetComponent<PlayerNumber>().getPlayerInd() == 1)
            {
                FirstPlayerExit();
            }
            else if (other.gameObject.GetComponent<PlayerNumber>().getPlayerInd() == 2)
            {
                SecondPlayerExit();
            }
        }
    }

    public (int firstPlayerScore, int secondPlayerScore) GetPlayerScore()
    {
        return (firstTeamScore, secondTeamScore);
    }
}
