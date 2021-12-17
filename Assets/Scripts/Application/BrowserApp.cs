using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleWebBrowser;
using WinCapture;

public class BrowserApp : MonoBehaviour
{
    GameObject webBrowser;

    public float windowScale = 0.001f;

    protected bool mousePosChanged = true; //Flag especifica para extensão do formato
    private bool mousePosMoveChanged = true; //Flag usada somente na BaseApplication

    protected Pointer pointer;
    Vector3 oldMouseVec;
    //Ray lastRay;

    bool leftMouseHold;

    int oldWidth, oldHeight;

    Quaternion lastRotation;

    enum MouseFunction { nothing, move, resizeHor, resizeVert, resizeDiag, changeDistance };
    MouseFunction function = MouseFunction.nothing;

    public void Move(Vector3 pos)
    {
        webBrowser.transform.position = pos;
    }

    public void setName(string name)
    {
        webBrowser.name = name;
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

    // Start is called before the first frame update
    void Awake()
    {
        webBrowser = Instantiate(Resources.Load("InworldBrowser", typeof(GameObject))) as GameObject;

        Debug.Log($"Creating2: {webBrowser.GetInstanceID()} {webBrowser.name}");
        pointer = GameObject.Find("Pointer").GetComponent<Pointer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (pointer.hit.collider != null)
        {

            if (pointer.hit.collider.gameObject == webBrowser.GetComponent<WebBrowser>().mainUIPanel.gameObject
                || function == MouseFunction.move)
            {
                pointer.sphereColor = ColorSettings.hoverMoveColor;
            }

            if (pointer.hit.collider.gameObject == webBrowser.GetComponent<WebBrowser>().mainUIPanel.gameObject)
            {
                if (pointer.mouseLeftDown && function == MouseFunction.nothing)
                {
                    leftMouseHold = true;
                    oldMouseVec = pointer.hit.point;
                    function = MouseFunction.move;
                }
            }
            if (pointer.mouseLeftUp && (function == MouseFunction.move))
            {
                function = MouseFunction.nothing;
                pointer.activeObjectID = webBrowser.GetInstanceID();
            }

            if (pointer.hit.collider.gameObject == webBrowser)
            {
                //Inicio HOVER
                //aplicacao de cores na esfera
                if (function == MouseFunction.changeDistance)
                    pointer.sphereColor = ColorSettings.changeDistanceColor;
                else if (pointer.activeObjectID == webBrowser.GetInstanceID())
                    pointer.sphereColor = ColorSettings.hoverActiveWindowColor;
                else
                    pointer.sphereColor = ColorSettings.hoverInactiveWindowColor;
                //Fim HOVER

                //Inicio FUNCOES JANELA
                //Determina area das janelas que executa cada funcao

                if (pointer.mouseMiddleDown)
                {
                    if (pointer.activeObjectID == webBrowser.GetInstanceID())
                    {
                        if (function == MouseFunction.nothing)
                            function = MouseFunction.changeDistance;
                        else if (function == MouseFunction.changeDistance)
                            function = MouseFunction.nothing;
                    }
                }
            }



            //Implementacao das funcoes da janela
            switch (function)
            {
                case MouseFunction.move:
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
                    break;
                case MouseFunction.changeDistance:
                    //Impede que chega no 0,0,0
                    if (webBrowser.transform.position.magnitude > -pointer.mouseWheelValue)
                        webBrowser.transform.position += webBrowser.transform.position.normalized * pointer.mouseWheelValue;
                    break;
            }
            //Fim FUNCOES JANELA

            if (pointer.activeObjectID == webBrowser.GetInstanceID())
            {
                WebBrowser web = webBrowser.GetComponent<WebBrowser>();
                if(function == MouseFunction.nothing) web.ProcessAllInputs();
            }
        }

        if (pointer.activeObjectID == webBrowser.GetInstanceID()) webBrowser.GetComponent<WebBrowser>().mainUIPanel.Background.color = ColorSettings.windowActiveColor;
        else webBrowser.GetComponent<WebBrowser>().mainUIPanel.Background.color = ColorSettings.windowInactiveColor;
    }
}
