using UnityEngine;
using Unity.Netcode;

public class Checkpoint : NetworkBehaviour
{
    private Color DefaultColor      = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private Color FirstPlayerColor  = new Color(0,    1f,   1f,   0.5f);
    private Color SecondPlayerColor = new Color(1f,   0.1f, 0,    0.5f);

    private const int framesPerPoint = 50;
    [SerializeField] private int pointsForCatch = 3;
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

    private NetworkVariable<CheckpointOwner> checkpointOwner =
    new NetworkVariable<CheckpointOwner>(
        CheckpointOwner.Nobody,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // if teamProgress > 0 - first team capcuring checkpoint
    // if teamProgress < 0 - second team capcuring checkpoint
    private int teamProgress = 0;

    int firstTeamPrescore = 0;
    int secondTeamPrescore = 0;

    int firstTeamScore = 0;
    int secondTeamScore = 0;

    public override void OnNetworkSpawn()
    {
        checkpointOwner.OnValueChanged += (_, _) => updateColor();
        updateColor();
    }

    void Start()
    {
        necessaryProgress = framesPerPoint * pointsForCatch;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsServer)
        {
            return;
        }

        updateProgress();
        updateScore();
    }

    private void updateProgress()
    {
        if (checkpointState == CheckpointState.FirstPlayerInside)
        {
            Debug.Log($"First player inside {teamProgress}");
            teamProgress++;
            updateOwner();
        }
        else if (checkpointState == CheckpointState.SecondPlayerInside)
        {
            Debug.Log($"Second player inside {teamProgress}");
            teamProgress--;
            updateOwner();
        }
    }

    private void updateScore() 
    {
        if (checkpointOwner.Value == CheckpointOwner.FirstPlayer)
        {
            firstTeamPrescore++;
            if (firstTeamPrescore % framesPerPoint == 0)
            {
                firstTeamPrescore = 0;
                firstTeamScore++;
                gameManager.GetComponent<GameManager>().UpdateScoreServerRpc();
            }
        }
        else
        {
            firstTeamPrescore = 0;
        }

        if (checkpointOwner.Value == CheckpointOwner.SecondPlayer)
        {
            secondTeamPrescore++;
            if (secondTeamPrescore % framesPerPoint == 0)
            {
                secondTeamPrescore = 0;
                secondTeamScore++;
                gameManager.GetComponent<GameManager>().UpdateScoreServerRpc();
            }
        }
        else
        {
            secondTeamPrescore = 0;
        }
    }

    private void updateOwner()
    {
        if (teamProgress >= necessaryProgress)
        {
            checkpointOwner.Value = CheckpointOwner.FirstPlayer;
            teamProgress = necessaryProgress;
        }
        else if (teamProgress <= -necessaryProgress)
        {
            checkpointOwner.Value = CheckpointOwner.SecondPlayer;
            teamProgress = -necessaryProgress;
        }
        else
            checkpointOwner.Value = CheckpointOwner.Nobody;
    }

    private void updateColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (checkpointOwner.Value == CheckpointOwner.FirstPlayer)
            renderer.material.color = FirstPlayerColor;
        else if (checkpointOwner.Value == CheckpointOwner.SecondPlayer)
            renderer.material.color = SecondPlayerColor;
        else
            renderer.material.color = DefaultColor;
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
