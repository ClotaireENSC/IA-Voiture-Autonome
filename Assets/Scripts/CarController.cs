using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public float currentSpeed;
    public float maxSpeed = 50;
    private int Accelerate = 0;
    private int TurnRight = 0;
    public bool Collision = false;

    private void Start()
    {
        InitializeCar();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Collision)
        {
            if (collision.gameObject.name == "Guard" || collision.gameObject.name == "Extra Guard")
            {
                this.Collision = true;
            }
            else
            {
                this.Collision = false;
            }
        }
    }

    private void InitializeCar()
    {
        currentSpeed = 0;
    }

    void Update()
    {
        GetUserKeys();

        if (!Collision)
        {
            Move();
            Rotate();
        }
    }

    private void Move()
    {
        if (Accelerate > 0.1)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * 0.1f);
        }
        else if (Accelerate < -0.1)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 1.0f);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 2f);
        }

        if (currentSpeed<0.01f)
        {
            currentSpeed = 0f;
        }

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        float angle = 0;
        if (TurnRight > 0.1)
        {
            angle = 90;
        }
        else if (TurnRight < -0.1)
        {
            angle = -90;
        }
        else
        {
            angle = 0;
        }

        if (currentSpeed < 0)
        {
            angle = -angle;
        }

        transform.Rotate(0, Time.deltaTime * angle * 20f / (Mathf.Abs(currentSpeed) + 10f), 0);
    }

    private void GetUserKeys()
    {
        Accelerate = 0;
        TurnRight = 0;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Accelerate += 1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Accelerate -= 1;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            TurnRight -= 1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            TurnRight += 1;
        }
    }
}