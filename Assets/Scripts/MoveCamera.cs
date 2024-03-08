using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject[] cars;
    private GameObject car;
    private float distance = 8f;
    private float height = 4f;
    private float rotationDamping = 3f;

    private Vector3 rotationVector;

    public void Start()
    {
    }

    public void Init()
    {
        cars = GameObject.FindGameObjectsWithTag("Car");
        car = cars[0];
    }

    public GameObject FindBestCar()
    {
        // Les vivantes tri�es par score croissant
        GameObject[] temp = cars.Where(c => c.GetComponent<CarController>().currentSpeed > 0.01).OrderBy(c => c.GetComponent<CarController>().score).ToArray();

        if (temp.Length == 0) return null; // Plus de vivantes

        GameObject last = temp.Last(); // La meilleure en vie

        if (car == null) return last; // Pas de best (premier round), on prend la derni�re

        // Best est vivante
        if (temp.Contains(car))
            return car.GetComponent<CarController>().score >= last.GetComponent<CarController>().score ? car : last; // Retourne meilleure entre best et derni�re

        // Best est morte donc derni�re
        return last;
    }

    void LateUpdate()
    {
        car = FindBestCar();

        if (car != null)
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
}