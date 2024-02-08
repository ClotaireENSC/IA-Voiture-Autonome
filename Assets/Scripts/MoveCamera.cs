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
        Init();
    }

    public void Init()
    {
        cars = GameObject.FindGameObjectsWithTag("Car");
        car = cars[0];
    }

    public void FindBestCar()
    {
        if (cars == null) { return; }

        CarController carController = car.GetComponent<CarController>();

        GameObject[] tempCars = cars
            .Where(c => c.GetComponent<CarController>().currentSpeed>0.01f)
            .OrderBy(c => c.GetComponent<CarController>().score)
            .ToArray();
        
        if (!tempCars.Contains(car))
        {
            car = tempCars.Last();
        }
    }

    void LateUpdate()
    {
        FindBestCar();

        Transform car_transform = car.transform;

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
        Transform car_transform = car.transform;

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
            FindBestCar();
        }
    }
}