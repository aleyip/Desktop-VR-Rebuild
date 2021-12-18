using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWebBrowser;
using WinCapture;

public class BrowserApp : MonoBehaviour
{
    GameObject webBrowser;

    protected bool mousePosChanged = true; //Flag especifica para extensão do formato
    private bool mousePosMoveChanged = true; //Flag usada somente na BaseApplication

    protected Pointer pointer;
    Vector3 oldMouseVec;
    //Ray lastRay;

    int oldWidth, oldHeight;

    bool closeFlag;

    Quaternion lastRotation;

    enum MouseFunction { nothing, move, changeDistance, close };
    MouseFunction function = MouseFunction.nothing;

    Dictionary<MouseFunction, Action> DoFunction;

    #region Miscellaneous

    public void Move(Vector3 pos)
    {
        webBrowser.transform.position = new Vector3(0, 0, pos.magnitude);
        webBrowser.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, Vector2.SignedAngle(new Vector2(webBrowser.transform.position.z, webBrowser.transform.position.x), new Vector2(pos.z, pos.x)));
        Vector3 vecPerp = Vector3.Cross(Vector3.down, webBrowser.transform.position);
        //roda em torno do eixo horizontal relativo
        webBrowser.transform.RotateAround(new Vector3(0, 0, 0), vecPerp, Vector3.SignedAngle(webBrowser.transform.position, pos, vecPerp));
    }

    public void Rotate(Vector3 rot)
    {
        webBrowser.transform.localEulerAngles = rot;
    }

    public void setName(string name)
    {
        webBrowser.name = name;
    }

    public void Scale(float scale)
    {
        webBrowser.transform.localScale *= scale;
    }
    
    private Vector3 GetPlayerPlaneMousePos()
    {
        Plane plane = new Plane(webBrowser.transform.position - Camera.main.transform.position, webBrowser.transform.position);
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        if (plane.Raycast(pointer.rayPointer, out dist))
        {
            return pointer.rayPointer.GetPoint(dist);
        }
        return Vector3.zero;
    }

    #endregion

    #region FUNCTIONS

    public void nothingFunction()
    {
        if (pointer.hit.collider != null)
        {
            if (pointer.hit.collider.gameObject == webBrowser.GetComponent<WebBrowser>().mainUIPanel.gameObject)
            {
                pointer.sphereColor = ColorSettings.hoverMoveColor;
                if (pointer.mouseLeftDown)
                {
                    oldMouseVec = pointer.hit.point;
                    function = MouseFunction.move;
                }
            }

            else if (pointer.activeObjectID == webBrowser.GetInstanceID())
            {
                pointer.sphereColor = ColorSettings.hoverActiveWindowColor;
                if (pointer.mouseMiddleDown) function = MouseFunction.changeDistance;
            }
            else
                pointer.sphereColor = ColorSettings.hoverInactiveWindowColor;
        }
        
    }

    public void changeDistanceFunction()
    {
        pointer.sphereColor = ColorSettings.changeDistanceColor;
        if (pointer.mouseMiddleDown) function = MouseFunction.nothing;

        if (webBrowser.transform.position.magnitude > -pointer.mouseWheelValue)
            webBrowser.transform.position += webBrowser.transform.position.normalized * pointer.mouseWheelValue;
    }

    public void moveFunction()
    {
        pointer.sphereColor = ColorSettings.hoverMoveColor;
        
        Vector3 MouseVecMove = GetPlayerPlaneMousePos();
        Vector3 relPos = webBrowser.transform.position - oldMouseVec;
        Vector3 endPos = oldMouseVec.magnitude * pointer.rayPointer.direction + relPos;
        //Usa Vector2 pq pega o angulo projetado em x-y, camera e pos inicial foram ajustada para permitir isso
        //roda em torno do eixo vertical
        webBrowser.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, Vector2.SignedAngle(new Vector2(webBrowser.transform.position.z, webBrowser.transform.position.x), new Vector2(endPos.z, endPos.x)));
        Vector3 vecPerp = Vector3.Cross(Vector3.down, webBrowser.transform.position);
        //roda em torno do eixo horizontal relativo
        webBrowser.transform.RotateAround(new Vector3(0, 0, 0), vecPerp, Vector3.SignedAngle(webBrowser.transform.position, endPos, vecPerp));
        oldMouseVec = MouseVecMove;

        if (pointer.transform.position.y < -2.5) closeFlag = true;
        else closeFlag = false;
        
        if (pointer.mouseLeftUp)
        {
            if (pointer.transform.position.y < -2.5) function = MouseFunction.close;
            else
            {
                function = MouseFunction.nothing;
                pointer.activeObjectID = webBrowser.GetInstanceID();
            }
        }
    }

    public void closeFunction()
    {
        Destroy(webBrowser);
        Destroy(this);
    }

    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        webBrowser = Instantiate(Resources.Load("InworldBrowser", typeof(GameObject))) as GameObject;

        Debug.Log($"Creating2: {webBrowser.GetInstanceID()} {webBrowser.name}");
        pointer = GameObject.Find("Pointer").GetComponent<Pointer>();
    }

    private void Start()
    {
        DoFunction = new Dictionary<MouseFunction, Action>();
        DoFunction.Add(MouseFunction.move, moveFunction);
        DoFunction.Add(MouseFunction.nothing, nothingFunction);
        DoFunction.Add(MouseFunction.close, closeFunction);
        DoFunction.Add(MouseFunction.changeDistance, changeDistanceFunction);

        closeFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        DoFunction[function]();

        if (pointer.hit.collider != null)
        {
            if (pointer.activeObjectID == webBrowser.GetInstanceID())
            {
                WebBrowser web = webBrowser.GetComponent<WebBrowser>();
                if(function == MouseFunction.nothing) web.ProcessAllInputs();
            }
        }

        if(closeFlag ) webBrowser.GetComponent<WebBrowser>().mainUIPanel.Background.color = ColorSettings.windowDestroyColor;
        else if (pointer.activeObjectID == webBrowser.GetInstanceID()) webBrowser.GetComponent<WebBrowser>().mainUIPanel.Background.color = ColorSettings.windowActiveColor;
        else webBrowser.GetComponent<WebBrowser>().mainUIPanel.Background.color = ColorSettings.windowInactiveColor;
    }
}
