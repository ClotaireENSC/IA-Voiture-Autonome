using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public float currentSpeed;
    public float maxSpeed = 50;
    private int Accelerate = 0;
    public int TurnRight = 0;
    public int TurnLeft = 0;
    public bool Collision = false;

    public NeuralNetwork NeuralNetwork;

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
        NeuralNetwork = new NeuralNetwork();
    }

    void Update()
    {
        //GetUserKeys();
        GetAiKeys();

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
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 2f);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 1f);
        }

        if (currentSpeed < 0.01f)
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
            angle += 90;
        }
        if (TurnLeft > 0.1)
        {
            angle -= 90;
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
        TurnLeft = 0;

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
            TurnLeft = 1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            TurnRight = 1;
        }
    }

    private void GetAiKeys()
    {
        double[] outputs = NeuralNetwork.Brain(new double[] { 0, 0, 0, 0, 0 });
        Accelerate = Convert.ToInt32(outputs[0]);
        TurnRight = Convert.ToInt32(outputs[1]);
        TurnLeft = Convert.ToInt32(outputs[2]);
    }
}