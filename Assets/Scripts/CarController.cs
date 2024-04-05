using System;
using TMPro;
using UnityEngine;

/*
CarController est une classe attachée à une voiture pour la contrôler

Globalement elle contient toutes les informations relatives au mouvement (Vitesse actuelle, Boutons appuyés, état de collision, ...)
et un réseau de neurones de type "NeuralNetwork"
*/

public class CarController : MonoBehaviour
{
    System.Random rnd = new System.Random();

    // Parametres de la voiture
    public float currentSpeed;
    public float maxSpeed = 50;
    public bool Collision = false;

    // "Boutons" actionnés par la voiture
    private int Accelerate = 0;
    private int Decelerate = 0;
    public int TurnRight = 0;
    public int TurnLeft = 0;

    // Initialisation des rayons entourant la voiture pour l'IA
    private Ray[] ray;
    private float rayLength = 10f;

    public NeuralNetwork NeuralNetwork;

    public int score = 0;

    private void Start()
    {
        InitializeCar();
    }


    // Initialisation de la voiture (Initialisation des rayons)
    private void InitializeCar()
    {
        currentSpeed = 0;

        ray = new Ray[5];
        for (int i = 0; i < ray.Length; i++)
        {
            ray[i] = new Ray();
        }
    }


    // Fonction éxecutée dès que la voiture entre en collision avec un élément
    private void OnCollisionEnter(Collision collision)
    {
        if (!Collision)
        {
            // Si la voiture collisionne un mur, elle s'arrête
            if (collision.gameObject.name == "Guard" ||
                collision.gameObject.name == "Extra Guard" ||
                collision.gameObject.name == "Guard_2" ||
                collision.gameObject.name == "Guard 2")
            {
                this.Collision = true;
                currentSpeed = 0;
            }
        }
    }


    // Fonction éxecutée quand la voiture croise un objet
    private void OnTriggerEnter(Collider other)
    {
        //Si la voiture croise un checkpoint dans le bon sens elle augmente son score, sinon l'inverse
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            score += IsSameDirection(transform.forward, other.transform.forward) ? 1 : -1;
        }
    }


    // Fonction qui regarde si 2 directions sont dans le même sens (pour les checkpoints notamment)
    private bool IsSameDirection(Vector3 direction1, Vector3 direction2)
    {
        return Vector3.Dot(direction1.normalized, direction2.normalized) > 0.9f;
    }


    // Fonction éxecutée à chaque frame de la simulation
    void FixedUpdate()
    {
        //GetUserKeys();

        // Récupération des choix de l'IA
        GetAiKeys();

        CheckRay();

        // Tant que la voiture n'est pas collisionée, elle bouge
        if (!Collision)
        {
            Move();
            Rotate();

            // Chance d'appliquer une rotation aléatoire (pour éviter le surapprentissage)
            if (rnd.NextDouble() < 0.001)
            {
                RndRotate();
            }
        }
    }


    // Modification de la vitesse actuelle selon les "Touches appuyées" par l'IA
    private void Move()
    {
        currentSpeed = (Accelerate > 0.1) ? Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * 0.1f) :
               (Decelerate > 0.1) ? Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 2f) :
               Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 1f);

        currentSpeed = (currentSpeed < 0.1f && Accelerate < 0.1) ? 0f : currentSpeed;

        // Version plus lisible

        // if (Accelerate > 0.1)
        // {
        //     currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * 0.1f);
        // }
        // else if (Decelerate > 0.1)
        // {
        //     currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 2f);
        // }
        // else
        // {
        //     currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 1f);
        // }

        // if (currentSpeed < 0.1f && Accelerate < 0.1)
        // {
        //     currentSpeed = 0f;
        // }

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        RayFollow();
    }


    // Suivi des rayons sur la voiture
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


    // Modification de la rotation de la voiture en fonction des "touches appuyées" par l'IA
    private void Rotate()
    {
        float angle = 0;

        angle += (TurnRight > 0.1) ? 90 : (TurnLeft > 0.1) ? -90 : 0;

        angle = (currentSpeed < 0) ? -angle : angle;

        if (currentSpeed != 0)
        {
            float rotationSpeed = Time.deltaTime * angle * 20f / (Mathf.Abs(currentSpeed) + 10f);
            transform.Rotate(0, rotationSpeed, 0);
        }


        // Version simplifiée

        // if (TurnRight > 0.1)
        // {
        //     angle += 90;
        // }
        // if (TurnLeft > 0.1)
        // {
        //     angle -= 90;
        // }

        // if (currentSpeed < 0)
        // {
        //     angle = -angle;
        // }
    }


    // Applique une rotation aléatoire à la voiture
    private void RndRotate()
    {
        transform.Rotate(0, rnd.Next(0, 1), 0);
    }

    public void CheckRay()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Checkpoints");
        layerMask = layerMask | (1 << LayerMask.NameToLayer("Voitures"));

        RaycastHit hitInfo;

        Physics.Raycast(ray[0], out hitInfo, rayLength + 40, ~layerMask);
        Debug.DrawRay(ray[0].origin, ray[0].direction * hitInfo.distance, Color.blue);

        for (int i = 1; i < ray.Length; i++)
        {
            Physics.Raycast(ray[i], out hitInfo, rayLength, ~layerMask);
            Debug.DrawRay(ray[i].origin, ray[i].direction * hitInfo.distance, Color.blue);
        }
    }

    // Fonction Utilisée pour les tests de circuit (L'utilisateur contrôle)
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


    // Récupération des actions de l'IA
    // RDN : Réseau de Neurones
    private void GetAiKeys()
    {
        int nbInputs = NeuralNetwork.LayersLengths[0];
        double[] inputs = new double[nbInputs];

        // Premier input du RDN : Vitesse actuelle normalisée
        inputs[0] = currentSpeed / maxSpeed;


        // Initialisation des calculs de rayons
        int layerMask = 1 << LayerMask.NameToLayer("Checkpoints");
        layerMask = layerMask | (1 << LayerMask.NameToLayer("Voitures"));

        RaycastHit hitInfo;

        // Deuxième input du RDN : Distance devant à 40m normalisée
        inputs[1] = Physics.Raycast(ray[0], out hitInfo, (rayLength + 40), ~layerMask) ? hitInfo.distance / (rayLength + 40) : (rayLength + 40);

        // Les 4 derniers inputs sont les 4 autres rayons autour 
        for (int i = 2; i < nbInputs; i++)
        {
            inputs[i] = Physics.Raycast(ray[i - 1], out hitInfo, rayLength, ~layerMask) ? hitInfo.distance / rayLength : rayLength;
        }


        // Calcul des sorties en fonction des entrées
        double[] outputs = NeuralNetwork.Brain(inputs);
        Accelerate = Convert.ToInt32(outputs[0]);
        Decelerate = Convert.ToInt32(outputs[1]);
        TurnRight = Convert.ToInt32(outputs[2]);
        TurnLeft = Convert.ToInt32(outputs[3]);
    }
}