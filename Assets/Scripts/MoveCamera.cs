using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private GameObject[] cars;
    private GameObject car;
    private Transform car_transform;
    private float distance = 8f;
    private float height = 4f;
    private float rotationDamping = 3f;

    private CarController carController;

    private Vector3 rotationVector;

    public void Start()
    {
        FindCar();
    }

    public void FindCar()
    {
        cars = GameObject.FindGameObjectsWithTag("Car");
        if (cars != null)
        {
            double maxDist = double.MinValue;
            car = null;

            foreach (GameObject c in cars)
            {
                Vector3 pos = c.transform.position;

                double distance = Mathf.Sqrt(pos.x * pos.x + pos.z * pos.z);

                if (distance > maxDist)
                {
                    maxDist = distance;
                    car = c;
                    car_transform = car.transform;
                    carController = car.GetComponent<CarController>();
                }
            }

            if (car == null)
            {
                Debug.LogError("Aucune voiture avec le tag 'Car' trouvée ou aucune vitesse valide.");
            }
        }
        else
        {
            Debug.LogError("Aucune voiture avec le tag 'Car' trouvée.");
        }
    }

    void LateUpdate()
    {
        FindCar();
        if (car != null)
        {
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
            float acc = car_transform.GetComponent<Rigidbody>().velocity.magnitude;
            // Vous pouvez modifier le comportement de la rotationVector ici en fonction de vos besoins
            Vector3 temp = rotationVector;
            temp.y = car_transform.eulerAngles.y;
            rotationVector = temp;
        }
        else
        {
            Start();
        }
    }
}