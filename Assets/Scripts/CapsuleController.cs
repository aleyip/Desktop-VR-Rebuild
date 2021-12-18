using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    [SerializeField]
    typeCapsule type;

    [SerializeField]
    float createDistance;

    Pointer pointer;

    Renderer rend;

    Vector3 startPosition;

    enum typeCapsule { Browser, Notepad, Paint, Sheets };

    enum functionList { Nothing, Move, Create};
    functionList function;

    Dictionary<functionList, Action> DoFunction;
    Dictionary<typeCapsule, Action> CreateWindow;

    #region FUNCTIONS CREATE

    void browserCreate()
    {
        BrowserApp go = gameObject.AddComponent<BrowserApp>();
        go.Rotate(new Vector3(-90, 0, 0));
        go.Move(createDistance * gameObject.transform.position.normalized);
        go.setName("Browser");
    }

    void sheetsCreate()
    {
        AddRemoveWindows wind = GameObject.Find("AppInit").GetComponent<AddRemoveWindows>();
        wind.createPos = createDistance * gameObject.transform.position.normalized;

        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        proc.EnableRaisingEvents = false;
        proc.StartInfo.FileName = "scalc.exe";
        proc.Start();
    }

    void notepadCreate()
    {
        AddRemoveWindows wind = GameObject.Find("AppInit").GetComponent<AddRemoveWindows>();
        wind.createPos = createDistance * gameObject.transform.position.normalized;

        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        proc.EnableRaisingEvents = false;
        proc.StartInfo.FileName = "notepad.exe";
        proc.Start();
    }

    void paintCreate()
    {
        AddRemoveWindows wind = GameObject.Find("AppInit").GetComponent<AddRemoveWindows>();
        wind.createPos = createDistance * gameObject.transform.position.normalized;

        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        proc.EnableRaisingEvents = false;
        proc.StartInfo.FileName = "mspaint.exe";
        proc.Start();
    }

    #endregion

    #region FUNCTIONS CAPSULA

    void nothingFunction()
    {
        if (pointer.hit.collider != null)
        {
            if (pointer.hit.collider.gameObject == gameObject)
            {
                rend.material.color = ColorSettings.capsuleSelectColor;
                pointer.sphereColor = ColorSettings.hoverActiveWindowColor;
                if (pointer.mouseLeftDown)
                {
                    function = functionList.Move;
                }
            }
            else rend.material.color = ColorSettings.capsuleInactiveColor;
        }
        else rend.material.color = ColorSettings.capsuleInactiveColor;
    }

    void moveFunction()
    {
        gameObject.transform.position = pointer.rayPointer.direction * startPosition.magnitude;
        if (gameObject.transform.position.y < -0.5f)
        {
            rend.material.color = ColorSettings.capsuleDestroyColor;
            pointer.sphereColor = ColorSettings.capsuleDestroyColor;
        }
        else
        {
            rend.material.color = ColorSettings.capsuleActiveColor;
            pointer.sphereColor = ColorSettings.capsuleActiveColor;
        }
        if (pointer.mouseLeftUp)
        {
            function = functionList.Create;
        }
    }

    void createFunction()
    {
        if (gameObject.transform.position.y >= -0.5f)
        {
            CreateWindow[type]();
        }
        gameObject.transform.localPosition = startPosition;
        function = functionList.Nothing;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        startPosition = gameObject.transform.localPosition;
        pointer = GameObject.Find("Pointer").GetComponent<Pointer>();
        rend = gameObject.GetComponent<Renderer>();
        function = functionList.Nothing;

        DoFunction = new Dictionary<functionList, Action>();
        DoFunction.Add(functionList.Nothing, nothingFunction);
        DoFunction.Add(functionList.Create, createFunction);
        DoFunction.Add(functionList.Move, moveFunction);

        CreateWindow = new Dictionary<typeCapsule, Action>();
        CreateWindow.Add(typeCapsule.Browser, browserCreate);
        CreateWindow.Add(typeCapsule.Sheets, sheetsCreate);
        CreateWindow.Add(typeCapsule.Notepad, notepadCreate);
        CreateWindow.Add(typeCapsule.Paint, paintCreate);
    }

    // Update is called once per frame
    void Update()
    {
        DoFunction[function]();
    }
}
