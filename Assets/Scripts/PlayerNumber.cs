using UnityEngine;

public class PlayerNumber : MonoBehaviour
{
    [SerializeField] private int player_ind = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getPlayerInd()
    {
        return player_ind;
    }
}
