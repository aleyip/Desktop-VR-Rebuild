using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    Shader sphereShader;

    public Ray rayPointer;

    public enum controlOption {mouse};
    public controlOption control;

    public RaycastHit hit;

    [NonSerialized]
    public int activeObjectID;

    [NonSerialized]
    public bool mouseRightDown, mouseLeftDown, mouseMiddleDown;

    [NonSerialized]
    public bool mouseRightUp, mouseLeftUp, mouseMiddleUp;

    [NonSerialized]
    public bool mouseRightHold, mouseLeftHold, mouseMiddleHold;

    [NonSerialized]
    public float mouseWheelValue;

    [NonSerialized]
    public string inputString;

    [NonSerialized]
    public Color sphereColor = Color.white;

    public float radius;

    Renderer rend;

    //dados iniciais do ponteiro
    float scaleInit;
    float distInit;

    // Start is called before the first frame update
    void Start()
    {
        //sphereShader = Shader.Find("SphereShader");

        //gameObject.transform.GetComponent<Renderer>().material = new Material(sphereShader);

        mouseLeftDown = Input.GetMouseButtonDown(0);
        mouseRightDown = Input.GetMouseButtonDown(1);
        mouseMiddleDown = Input.GetMouseButtonDown(2);
        mouseLeftUp = Input.GetMouseButtonUp(0);
        mouseRightUp = Input.GetMouseButtonUp(1);
        mouseMiddleUp = Input.GetMouseButtonUp(0);

        rend = gameObject.GetComponent<Renderer>();

        activeObjectID = 0;

        scaleInit = this.transform.localScale.x;
        distInit = this.transform.position.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (control == controlOption.mouse)
        {
            Debug.Log(Input.mousePosition);
            rayPointer = Camera.main.ScreenPointToRay(Input.mousePosition);

            mouseLeftDown = Input.GetMouseButtonDown(0);

            mouseLeftDown = Input.GetMouseButtonDown(0);
            mouseRightDown = Input.GetMouseButtonDown(1);
            mouseMiddleDown = Input.GetMouseButtonDown(2);
            mouseLeftUp = Input.GetMouseButtonUp(0);
            mouseRightUp = Input.GetMouseButtonUp(1);
            mouseMiddleUp = Input.GetMouseButtonUp(2);
            mouseLeftHold = Input.GetMouseButton(0);
            mouseRightHold = Input.GetMouseButton(1);
            mouseMiddleHold = Input.GetMouseButton(2);

            mouseWheelValue = Input.GetAxis("Mouse ScrollWheel");

            if (Input.anyKeyDown)
            {
                inputString = Input.inputString;
                foreach (char c in inputString) Debug.Log((int)c);
            }
            else
                inputString = null;
            
        }
        if (Physics.Raycast(new Vector3(0, 0, 0), rayPointer.direction, out hit, 1000.0f))
        {
            this.transform.position = hit.point;
            float valueScale = this.transform.position.magnitude / distInit * scaleInit;
            this.transform.localScale = new Vector3(valueScale, valueScale, valueScale);
            if (mouseLeftDown || mouseRightDown || mouseMiddleDown)
            {
                activeObjectID = hit.collider.gameObject.GetInstanceID();
                Debug.Log($"ActiveID: {activeObjectID} {hit.collider.gameObject.name}");
            }
            rend.material.color = sphereColor;
        }
        else
        {
            this.transform.position = rayPointer.direction * radius;
            float valueScale = this.transform.position.magnitude / distInit * scaleInit;
            this.transform.localScale = new Vector3(valueScale, valueScale, valueScale);
            if (mouseLeftDown || mouseRightDown || mouseMiddleDown)
            {
                activeObjectID = 0;
            }

            rend.material.color = Color.grey;
        }
    }
}
