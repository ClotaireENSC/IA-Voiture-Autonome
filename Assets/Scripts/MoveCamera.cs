using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
MoveCamera est une classe attachee à la camera pour la controler 
*/
public class MoveCamera : MonoBehaviour
{
    // Recuperation des voitures (cars) et de la voiture que l'on va suivre (car)
    public GameObject[] cars;
    private GameObject car;

    // Parametres de la camera
    private float distance = 8f;
    private float height = 4f;
    private float rotationDamping = 3f;

    // Deuxieme vue
    private bool isAboveCar = false;

    private Vector3 rotationVector;


    // Recuperation des voitures et de la premiere
    public void Init()
    {
        cars = GameObject.FindGameObjectsWithTag("Car");
        car = cars[0];
    }


    // Fonction majeure de la classe qui va chercher la meilleure voiture pour la suivre
    // Le but ici est de recuperer la voiture ayant le plus haut score, qui n'est pas collisionee
    public GameObject FindBestCar()
    {
        // tri des voitures non collisionees par score croissant (filtrage + tri)
        GameObject[] temp = cars.Where(c => c.GetComponent<CarController>().currentSpeed > 0.01).OrderBy(c => c.GetComponent<CarController>().score).ToArray();

        if (temp.Length == 0) return null; // Plus de vivantes

        GameObject last = temp.Last(); // La meilleure en vie

        if (car == null) return last; // Pas de meilleure, on renvoie la derniere voiture

        // Renvoi de la meilleure voiture entre la meilleure actuelle et la meilleure de la liste
        if (temp.Contains(car))
            return car.GetComponent<CarController>().score >= last.GetComponent<CarController>().score ? car : last;

        // Cas ou la voiture actuelle est collisionee
        return last;
    }


    // A chaque update on recupere la meilleure voiture puis on applique un mouvement a la camera
    void LateUpdate()
    {
        car = FindBestCar();
        if (car != null)
        {
            if (!isAboveCar)
            {
                Transform car_transform = car.transform;

                float wantedAngle = rotationVector.y;
                float wantedHeight = car_transform.position.y + height;
                float myAngle = transform.eulerAngles.y;
                float myHeight = transform.position.y;

                myAngle = Mathf.LerpAngle(myAngle, wantedAngle, rotationDamping * Time.deltaTime);
                myHeight = Mathf.Lerp(myHeight, wantedHeight, rotationDamping * Time.deltaTime);

                Quaternion currentRotation = Quaternion.Euler(0, myAngle, 0);

                transform.position = car_transform.position;
                transform.position -= currentRotation * Vector3.forward * distance;
                Vector3 temp = transform.position;
                temp.y = myHeight;
                transform.position = temp;
                transform.LookAt(car_transform);
            }
            else
            {
                Transform car_transform = car.transform;
                transform.position = new Vector3(car_transform.position.x, 30, car_transform.position.z);
                transform.rotation = Quaternion.Euler(90f, car_transform.eulerAngles.y, 0f);
            }
        }
    }

    void FixedUpdate()
    {
        if (car != null)
        {
            Transform car_transform = car.transform;

            float acc = car_transform.GetComponent<Rigidbody>().velocity.magnitude;

            Vector3 temp = rotationVector;
            temp.y = car_transform.eulerAngles.y;
            rotationVector = temp;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Si la touche "Espace" est enfoncée, inverse la valeur de la variable booléenne
        {
            isAboveCar = !isAboveCar;
        }
    }
}
