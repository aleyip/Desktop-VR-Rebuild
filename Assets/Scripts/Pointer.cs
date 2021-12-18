using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    #region NonSerialized Fields

    Shader sphereShader;

    public Ray rayPointer;

    enum controlOption {mouse2D, mouse3D ,hydraRazor};

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

    //dados iniciais do ponteiro
    float scaleInit;
    float distInit;

    Renderer rend;

    Dictionary<controlOption, Action> Controller;

    #endregion

    #region Serialized Fields
    public float radius;

    [SerializeField]
    controlOption control;

    public float sensibilidade = 2.0f;

    #endregion

    #region Input Controllers

    void mouse2DController()
    {
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

    void mouse3DController()
    {
        //mouseX += Input.GetAxis("Mouse X") * sensibilidade; // Incrementa o valor do eixo X e multiplica pela sensibilidade
        //mouseY -= Input.GetAxis("Mouse Y") * sensibilidade; // Incrementa o valor do eixo Y e multiplica pela sensibilidade. (Obs. usamos o - para inverter os valores)
        //Adaptar aqui usando GetAxis

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

    void hydraRazorController()
    {
        //TO DO... adicionar aqui código para controle usando hydra razor
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();

        activeObjectID = 0;

        scaleInit = this.transform.localScale.x;
        distInit = this.transform.position.magnitude;

        Controller = new Dictionary<controlOption, Action>();
        Controller.Add(controlOption.hydraRazor, hydraRazorController);
        Controller.Add(controlOption.mouse2D, mouse2DController);
        Controller.Add(controlOption.mouse3D, mouse3DController);
    }

    // Update is called once per frame
    void Update()
    {
        Controller[control]();

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
