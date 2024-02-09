using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public float currentSpeed;
    public float maxSpeed = 50;
    private int Accelerate = 0;
    private int Decelerate = 0;
    public int TurnRight = 0;
    public int TurnLeft = 0;
    public bool Collision = false;

    private Ray[] ray;
    private float rayLength = 30f;

    public NeuralNetwork NeuralNetwork;

    public int score = 0;

    public TextMeshPro scoreText;

    private void Start()
    {
        InitializeCar();
    }

    private void InitializeCar()
    {
        currentSpeed = 0;

        ray = new Ray[5];
        for (int i = 0; i < ray.Length; i++)
        {
            ray[i] = new Ray();
        }

        scoreText = GetComponentInChildren<TextMeshPro>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Collision)
        {
            if (collision.gameObject.name == "Guard" ||
                collision.gameObject.name == "Extra Guard" ||
                collision.gameObject.name == "Guard_2")
            {
                this.Collision = true;
                currentSpeed = 0;
            }
            else
            {
                this.Collision = false;
            }


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            if (IsSameDirection(transform.forward, other.transform.forward))
            {
                score++;
            }
            else
            {
                score--;
            }
        }
    }

    private bool IsSameDirection(Vector3 direction1, Vector3 direction2)
    {
        return Vector3.Dot(direction1.normalized, direction2.normalized) > 0.9f;
    }


    void Update()
    {
        CheckRay();

        //GetUserKeys();
        GetAiKeys();

        ShowText();

        if (!Collision)
        {
            Move();
            Rotate();
        }
    }

    public void CheckRay()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Checkpoints");
        layerMask = layerMask | (1 << LayerMask.NameToLayer("Voitures"));

        for (int i = 0; i < ray.Length; i++)
        {
            Color rayColor = Physics.Raycast(ray[i], rayLength, ~layerMask) ? Color.red : Color.green;
            Debug.DrawRay(ray[i].origin, ray[i].direction * rayLength, rayColor);
        }
    }

    private void Move()
    {
        if (Accelerate > 0.1)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * 0.1f);
        }
        else if (Decelerate > 0.1)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 2f);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 1f);
        }

        if (currentSpeed < 0.1f && Accelerate < 0.1)
        {
            currentSpeed = 0f;
        }

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        RayFollow();
    }

    private void RayFollow()
    {
        float rayHeight = 0.15f; // Hauteur du rayon par rapport � la position du GameObject

        // Rayon devant
        ray[0].origin = transform.position + new Vector3(0, rayHeight, 0);
        ray[0].direction = transform.forward;

        // Rayon � droite
        ray[1].origin = transform.position + new Vector3(0, rayHeight, 0);
        ray[1].direction = transform.right;

        // Rayon � gauche
        ray[2].origin = transform.position + new Vector3(0, rayHeight, 0);
        ray[2].direction = -transform.right;

        // Rayon Nord Est
        ray[3].origin = transform.position + new Vector3(0, rayHeight, 0);
        ray[3].direction = transform.forward + transform.right;

        // Rayon Nord Ouest
        ray[4].origin = transform.position + new Vector3(0, rayHeight, 0);
        ray[4].direction = transform.forward - transform.right;
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

        if (currentSpeed != 0)
        {
            transform.Rotate(0, Time.deltaTime * angle * 20f / (Mathf.Abs(currentSpeed) + 10f), 0);
        }
    }

    private void GetUserKeys()
    {
        Accelerate = 0;
        Decelerate = 0;
        TurnRight = 0;
        TurnLeft = 0;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Accelerate = 1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Decelerate = 1;
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
        double[] inputs = new double[6];
        inputs[0] = currentSpeed / maxSpeed;

        int layerMask = 1 << LayerMask.NameToLayer("Checkpoints");
        layerMask = layerMask | (1 << LayerMask.NameToLayer("Voitures"));

        RaycastHit hitInfo;
        
        for (int i = 1; i < 6; i++)
        {
            if (Physics.Raycast(ray[i - 1], out hitInfo, rayLength, ~layerMask))
            {
                // Assign the distance of the collision to the input
                inputs[i] = hitInfo.distance / rayLength;
            }
            else
            {
                // If no collision, assign a default value (e.g., maximum distance)
                inputs[i] = 1; // You may want to adjust this value based on your game's requirements
            }
        }

        double[] outputs = NeuralNetwork.Brain(inputs);
        Accelerate = Convert.ToInt32(outputs[0]);
        Decelerate = Convert.ToInt32(outputs[1]);
        TurnRight = Convert.ToInt32(outputs[2]);
        TurnLeft = Convert.ToInt32(outputs[3]);


        //string inputsString = "";
        //for (int i = 0; i < 6; i++)
        //    inputsString += $"{inputs[i]}/";
        //Debug.Log(inputsString);
    }

    public void ShowText()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            scoreText.text = score.ToString("F0");
        }
        else
        {
            scoreText.text = "";
        }
    }
}