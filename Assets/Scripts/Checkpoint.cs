using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private const int framesPerPoint = 50;
    [SerializeField] private int pointsForCatch = 10;
    private int necessaryProgress;

    enum CheckpointState
    {
        NoPlayerInside,
        FirstPlayerInside,
        SecondPlayerInside,
        BothPlayersInside
    }

    enum CheckpointOwner
    {
        Nobody,
        FirstPlayer,
        SecondPlayer
    }

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private GameObject gameManager;
    private CheckpointState checkpointState = CheckpointState.NoPlayerInside;
    private CheckpointOwner checkpointOwner = CheckpointOwner.Nobody;

    private int firstTeamProgress = 0;
    private int secondTeamProgress = 0;

    int firstTeamPrescore = 0;
    int secondTeamPrescore = 0;

    int firstTeamScore = 0;
    int secondTeamScore = 0;

    void Start()
    {
        necessaryProgress = framesPerPoint * pointsForCatch;
    }

    // Update is called once per frame
    void Update()
    {
        updateProgress();
        updateScore();
    }

    private void updateProgress()
    {
        if (checkpointState == CheckpointState.FirstPlayerInside)
        {
            if (secondTeamProgress > 0)
            {
                secondTeamProgress--;
                if (checkpointOwner == CheckpointOwner.SecondPlayer)
                    updateOwner();
            }
            else if (firstTeamProgress < necessaryProgress)
            {
                firstTeamProgress++;
                if (firstTeamProgress == necessaryProgress)
                    updateOwner();
            }
        }
        else if (checkpointState == CheckpointState.SecondPlayerInside)
        {
            if (firstTeamProgress > 0)
            {
                firstTeamProgress--;
                if (checkpointOwner == CheckpointOwner.FirstPlayer)
                    updateOwner();
            }
            else if (secondTeamProgress < necessaryProgress)
            {
                secondTeamProgress++;
                if (secondTeamProgress == necessaryProgress)
                    updateOwner();
            }
        }
    }

    private void updateScore() 
    {
        if (checkpointOwner == CheckpointOwner.FirstPlayer)
        {
            firstTeamPrescore++;
            if (firstTeamPrescore % framesPerPoint == 0)
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

        if (checkpointOwner == CheckpointOwner.SecondPlayer)
        {
            secondTeamPrescore++;
            if (secondTeamPrescore % framesPerPoint == 0)
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

    private void updateOwner()
    {
        if (firstTeamProgress == necessaryProgress)
            checkpointOwner = CheckpointOwner.FirstPlayer;
        else if (secondTeamProgress == necessaryProgress)
            checkpointOwner = CheckpointOwner.SecondPlayer;
        else 
            checkpointOwner = CheckpointOwner.Nobody;
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
