using UnityEngine;

public class Move : MonoBehaviour
{
    Vector3 speed;
    Vector3Int movingCoeff;
    float weight;

    Vector3 FrictionCoeff = new Vector3(0.1f, 0f, 0.1f);
    const float DeltaTime = 0.1f;
    const float Mass = 1f;
    const float G = 9.8f;
    Vector3 MovingAcceleration = new Vector3(5f, 0, 5f);

    void Start()
    {
        weight = Mass * G;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckKeybordPress();
        ProcessMoving();
    }

    void CheckKeybordPress()
    {
        if (Input.GetKey(KeyCode.W))
        {
            movingCoeff.x = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movingCoeff.x = -1;
        }
        else
        {
            movingCoeff.x = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movingCoeff.z = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movingCoeff.z = -1;
        }
        else
        {
            movingCoeff.z = 0;
        }
    }

    void ProcessMoving()
    {
        Vector3 power = new Vector3();
        power.x = movingCoeff.x * MovingAcceleration.x;
        power.y = movingCoeff.y * MovingAcceleration.y;
        power.z = movingCoeff.z * MovingAcceleration.z;

        ProcessFrictionPower(ref power);

        CalculateNewPosition(power);
    }

    void ProcessFrictionPower(ref Vector3 power)
    {
        Debug.Log("power.x = " + power.x);
        if (speed.x != 0)
        {
            if (speed.x < 0)
                power.x += FrictionCoeff.x * weight;
            else
                power.x -= FrictionCoeff.x * weight;
        }

        if (speed.z != 0)
        {
            if (speed.z < 0)
                power.z += FrictionCoeff.z * weight;
            else
                power.z -= FrictionCoeff.z * weight;
        }

        Debug.Log("power.x = " + power.x);
    }

    void CalculateNewPosition(Vector3 power)
    {
        Vector3 acceleration = power / Mass;

        speed.x += DeltaTime * acceleration.x;
        speed.y += DeltaTime * acceleration.y;
        speed.z += DeltaTime * acceleration.z;

        Vector3 pos = transform.position;
        pos.x += DeltaTime * speed.x;
        pos.y += DeltaTime * speed.y;
        pos.z += DeltaTime * speed.z;

        transform.position = pos;
    }
}
