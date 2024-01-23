using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private GameObject car;
    private Transform car_transform;
    private float distance = 8f;
    private float height = 4f;
    private float rotationDamping = 3f;

    private Vector3 rotationVector;

    public void Start()
    {
        FindCar();
    }

    public void FindCar()
    {
        car = GameObject.FindGameObjectWithTag("Car");
        if (car != null)
        {
            // Si une voiture est trouvée, assigner son transform à la variable "car"
            car_transform = car.transform;
        }
        else
        {
            Debug.LogError("Aucune voiture avec le tag 'Car' trouvée.");
        }
    }

    void LateUpdate()
    {
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
        else
        {
            FindCar();
            transform.position = new Vector3(-38, 5, -60);
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.Rotate(new Vector3(20, -8, 0));
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