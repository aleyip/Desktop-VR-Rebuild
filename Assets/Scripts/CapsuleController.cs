using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    Pointer pointer;

    Renderer rend;

    Vector3 startPosition;

    enum typeCapsule { Browser, Notepad, Paint, Sheets};
    [SerializeField]
    typeCapsule type;

    enum functionList { Nothing, Move, Create};
    functionList function;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = gameObject.transform.position;
        pointer = GameObject.Find("Pointer").GetComponent<Pointer>();
        rend = gameObject.GetComponent<Renderer>();
        function = functionList.Nothing;
    }

    // Update is called once per frame
    void Update()
    {
        switch (function)
        {
            case functionList.Nothing:
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
                break;
            case functionList.Move:
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
                    gameObject.transform.position = startPosition;
                    function = functionList.Create;
                }
                break;
            case functionList.Create:
                function = functionList.Nothing;
                break;

        }
        
    }
}
