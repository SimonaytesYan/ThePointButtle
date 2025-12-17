using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    enum CheckpointState
    {
        NoPlayerInside,
        FirstPlayerInside,
        SecondPlayerInside,
        BothPlayersInside
    }

    [SerializeField] private LayerMask playerMask;
    private CheckpointState checkpointState = CheckpointState.NoPlayerInside;

    int firstTeamScore = 0;
    int secondTeamScore = 0;

    int firstTeamPrescore = 0;
    int secondTeamPrescore = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        if (other.gameObject.GetComponent<PlayerNumber>().getPlayerInd() == 1)
        {
            FirstPlayerEnter();
        }
        else if (other.gameObject.GetComponent<PlayerNumber>().getPlayerInd() == 2)
        {
            SecondPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
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
