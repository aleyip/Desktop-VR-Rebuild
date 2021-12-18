using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using WinCapture;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class BaseApplication : MonoBehaviour
{
    Shader windowShader;

    private GameObject goParent;
    protected WindowCapture windowsRender;
    protected GameObject windowObject;
    private BaseAppUI ui;

    public float windowScale = 0.001f;

    protected Int32 oldMousePos;
    protected bool mousePosChanged = true; //Flag especifica para extensão do formato
    private bool mousePosMoveChanged = true; //Flag usada somente na BaseApplication

    protected Pointer pointer;
    Vector3 oldMouseVec;

    int oldWidth, oldHeight;
    Int32 xMouse, yMouse;

    bool closeFlag;

    Quaternion lastRotation;

    enum MouseFunction { nothing, move, resizeHor, resizeVert, resizeDiag, changeDistance, close};
    MouseFunction function = MouseFunction.nothing;

    Dictionary<MouseFunction, Action> DoFunction;

    #region Miscellaneous

    public void passWindow(WindowCapture window)
    {
        windowsRender = window;
        goParent.name = window.windowInfo.title;
        Debug.Log(windowObject.GetInstanceID());
        pointer.activeObjectID = windowObject.GetInstanceID();
        ui.setName(goParent.name);
    }

    public void Move(Vector3 pos)
    {
        goParent.transform.position = new Vector3(0, 0, pos.magnitude);
        goParent.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, Vector2.SignedAngle(new Vector2(goParent.transform.position.z, goParent.transform.position.x), new Vector2(pos.z, pos.x)));
        Vector3 vecPerp = Vector3.Cross(Vector3.down, goParent.transform.position);
        //roda em torno do eixo horizontal relativo
        goParent.transform.RotateAround(new Vector3(0, 0, 0), vecPerp, Vector3.SignedAngle(windowObject.transform.position, pos, vecPerp));
    }

    public void Rotate(Vector3 rot)
    {
        goParent.transform.localEulerAngles = rot;
    }

    private Vector3 GetPlayerPlaneMousePos()
    {
        Plane plane = new Plane(goParent.transform.position - Camera.main.transform.position, goParent.transform.position);
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        if (plane.Raycast(pointer.rayPointer, out dist))
        {
            return pointer.rayPointer.GetPoint(dist);
        }
        return Vector3.zero;
    }

    #endregion

    #region Windows Function

    public void nothingFunction()
    {
        if (xMouse >= 0 && xMouse <= windowsRender.windowWidth && yMouse >= 0 && yMouse <= windowsRender.windowHeight)
        {
            if ((xMouse <= 8 && (yMouse <= 8 || yMouse >= windowsRender.windowHeight - 8)) ||
                 (xMouse >= windowsRender.windowWidth - 8 && (yMouse <= 8 || yMouse >= windowsRender.windowHeight - 8)))
            {
                pointer.sphereColor = ColorSettings.hoverDiagRescaleColor;
                if (pointer.mouseLeftDown) function = MouseFunction.resizeDiag;
            }
            else if ((xMouse <= 8 || xMouse >= windowsRender.windowWidth - 8))
            {
                pointer.sphereColor = ColorSettings.hoverHorRescaleColor;
                if (pointer.mouseLeftDown) function = MouseFunction.resizeHor;
            }
            else if ((yMouse <= 8 || yMouse >= windowsRender.windowHeight - 8))
            {
                pointer.sphereColor = ColorSettings.hoverVertRescaleColor;
                if (pointer.mouseLeftDown) function = MouseFunction.resizeVert;
            }
            else if (yMouse < 27 || function == MouseFunction.move)
            {
                pointer.sphereColor = ColorSettings.hoverMoveColor;
                if (pointer.mouseLeftDown) function = MouseFunction.move;
            }
            else
            {
                if (pointer.activeObjectID == windowObject.GetInstanceID())
                    pointer.sphereColor = ColorSettings.hoverActiveWindowColor;
                else
                    pointer.sphereColor = ColorSettings.hoverInactiveWindowColor;
            }
            if (pointer.mouseLeftDown)
            {
                oldMouseVec = pointer.hit.point;
                oldWidth = windowsRender.windowWidth;
                oldHeight = windowsRender.windowHeight;
                lastRotation = Quaternion.FromToRotation(windowObject.transform.position, new Vector3(0, windowObject.transform.position.y, windowObject.transform.position.z));
                lastRotation = Quaternion.FromToRotation(new Vector3(0, windowObject.transform.position.y, windowObject.transform.position.z), new Vector3(0, -1, 0)) * lastRotation;
            }
            if (pointer.mouseMiddleDown) function = MouseFunction.changeDistance;
        }
    }

    public void changeDistanceFunction()
    {
        pointer.sphereColor = ColorSettings.changeDistanceColor;
        if (pointer.mouseMiddleDown) function = MouseFunction.nothing;

        if (goParent.transform.position.magnitude > -pointer.mouseWheelValue)
            goParent.transform.position += goParent.transform.position.normalized * pointer.mouseWheelValue;
    }

    public void moveFunction()
    {
        pointer.sphereColor = ColorSettings.hoverMoveColor;
        Vector3 MouseVecMove = GetPlayerPlaneMousePos();
        Vector3 relPos = windowObject.transform.position - oldMouseVec;
        Vector3 endPos = oldMouseVec.magnitude * pointer.rayPointer.direction + relPos;
        //Usa Vector2 pq pega o angulo projetado em x-y, camera e pos inicial foram ajustada para permitir isso
        //roda em torno do eixo vertical
        goParent.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, Vector2.SignedAngle(new Vector2(goParent.transform.position.z, goParent.transform.position.x), new Vector2(endPos.z, endPos.x)));
        Vector3 vecPerp = Vector3.Cross(Vector3.down, goParent.transform.position);
        //roda em torno do eixo horizontal relativo
        goParent.transform.RotateAround(new Vector3(0, 0, 0), vecPerp, Vector3.SignedAngle(goParent.transform.position, endPos, vecPerp));
        oldMouseVec = MouseVecMove;

        if (pointer.transform.position.y < -2.5) closeFlag = true;
        else closeFlag = false;

        if (pointer.mouseLeftUp)
        {
            if (pointer.transform.position.y < -2.5) function = MouseFunction.close;
            else function = MouseFunction.nothing;
        }
    }

    public void resizeHorFunction()
    {
        pointer.sphereColor = ColorSettings.hoverHorRescaleColor;
        Vector3 MouseVecHor = GetPlayerPlaneMousePos();
        float relIncHor = Math.Abs((lastRotation * (MouseVecHor - windowObject.transform.position)).x / (lastRotation * (oldMouseVec - windowObject.transform.position)).x);
        Win32Funcs.MoveWindow(windowsRender.hwnd, 0, 0, (int)(relIncHor * oldWidth), oldHeight, true);

        if (pointer.mouseLeftUp) function = MouseFunction.nothing;
    }

    public void resizeDiagFunction()
    {
        pointer.sphereColor = ColorSettings.hoverDiagRescaleColor;
        Vector3 MouseVecDiag = GetPlayerPlaneMousePos();
        float relIncDiagHor = Math.Abs((lastRotation * (MouseVecDiag - windowObject.transform.position)).x / (lastRotation * (oldMouseVec - windowObject.transform.position)).x);
        float relIncDiagVert = Math.Abs((lastRotation * (MouseVecDiag - windowObject.transform.position)).z / (lastRotation * (oldMouseVec - windowObject.transform.position)).z);
        Win32Funcs.MoveWindow(windowsRender.hwnd, 0, 0, (int)(relIncDiagHor * oldWidth), (int)(relIncDiagVert * oldHeight), true);

        if (pointer.mouseLeftUp) function = MouseFunction.nothing;
    }

    public void resizeVertFunction()
    {
        pointer.sphereColor = ColorSettings.hoverVertRescaleColor;
        Vector3 MouseVecVert = GetPlayerPlaneMousePos();
        //Debug.Log($"{lastRotation * MouseVecVert} {windowObject.transform.position} {lastRotation * windowObject.transform.position}");
        float relIncVert = Math.Abs((lastRotation * (MouseVecVert - windowObject.transform.position)).z / (lastRotation * (oldMouseVec - windowObject.transform.position)).z);
        Win32Funcs.MoveWindow(windowsRender.hwnd, 0, 0, oldWidth, (int)(relIncVert * oldHeight), true);

        if (pointer.mouseLeftUp) function = MouseFunction.nothing;
    }

    public void closeFunction()
    {
        int processId;
        Win32Funcs.GetWindowThreadProcessId(windowsRender.hwnd, out processId);
        Process p = Process.GetProcessById(processId);
        p.Kill();
        //Muda de funcao para não tentar executar o p.Kill novamente, estava dando problema por causa que o Update rodava de novo.
        function = MouseFunction.nothing;
    }

    #endregion


    // Start is called before the first frame update
    protected void Awake()
    {
        windowShader = Shader.Find("WinCapture/WindowShader");

        goParent = Instantiate(Resources.Load("BaseAppPrefab", typeof(GameObject))) as GameObject;
        goParent.transform.localScale = new Vector3(windowScale, windowScale, windowScale);

        windowObject = goParent.transform.Find("Main").gameObject;
        windowObject.transform.GetComponent<Renderer>().material = new Material(windowShader);
        Debug.Log(windowObject.name);
        pointer = GameObject.Find("Pointer").GetComponent<Pointer>();
        windowObject.transform.localEulerAngles = new Vector3(0, -180, 0);

        ui = goParent.transform.Find("UI").gameObject.GetComponent<BaseAppUI>();
        ui.InitPrefabLinks();
        ui.setColorUI(Color.green);
    }

    protected void Start()
    {
        DoFunction = new Dictionary<MouseFunction, Action>();
        DoFunction.Add(MouseFunction.changeDistance, changeDistanceFunction);
        DoFunction.Add(MouseFunction.move, moveFunction);
        DoFunction.Add(MouseFunction.nothing, nothingFunction);
        DoFunction.Add(MouseFunction.resizeDiag, resizeDiagFunction);
        DoFunction.Add(MouseFunction.resizeHor, resizeHorFunction);
        DoFunction.Add(MouseFunction.resizeVert, resizeVertFunction);
        DoFunction.Add(MouseFunction.close, closeFunction);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (pointer.hit.collider != null)
            if (pointer.hit.collider.gameObject == windowObject)
            {
                //Inicio CONVERSOR
                //Converte direcao em ponto xy na janela
                xMouse = (Int32)(pointer.hit.textureCoord.x * windowsRender.windowWidth);
                yMouse = (Int32)((1.0f - pointer.hit.textureCoord.y) * windowsRender.windowHeight);
                Int32 mousePos = (Int32)((yMouse << 16) | xMouse);
                if (oldMousePos != mousePos && xMouse >= 0 && yMouse >= 0 && xMouse <= windowsRender.windowWidth && yMouse <= windowsRender.windowHeight)
                {
                    mousePosMoveChanged = true;
                    mousePosChanged = true;
                }
                oldMousePos = mousePos;
                //Fim CONVERSOR
            }
            else
            {
                xMouse = -1;
                yMouse = -1;
            }
        else
        {
            xMouse = -1;
            yMouse = -1;
        }

        DoFunction[function]();

        if(closeFlag) ui.setColorUI(ColorSettings.windowDestroyColor);
        else if (pointer.activeObjectID == windowObject.GetInstanceID() || function == MouseFunction.move) ui.setColorUI(ColorSettings.windowActiveColor);
        else ui.setColorUI(ColorSettings.windowInactiveColor);


        bool didChangeTex;
        if (windowObject != null)
        {
            Texture2D windowTexture = windowsRender.GetWindowTexture(out didChangeTex);
            //addTextureBorder(ref windowTexture, Color.blue, 30, 3, 3, 3);
            if (didChangeTex)
            {
                windowObject.GetComponent<Renderer>().material.mainTexture = windowTexture;
                Debug.Log($"Dim: {windowsRender.windowWidth} {windowsRender.windowHeight}");
            }
            //windowObject.transform.localScale = new Vector3(windowsRender.windowWidth * windowScale, 0.1f, windowsRender.windowHeight * windowScale);
            windowObject.transform.localScale = new Vector3(windowsRender.windowWidth, 0.1f, windowsRender.windowHeight);
            ui.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(windowObject.transform.localScale.x, windowObject.transform.localScale.z);
            
        }
    }

    private void OnDestroy()
    {
        if(goParent != null)
            UnityEngine.Object.Destroy(goParent);
        if(windowsRender != null)
            windowsRender.Dispose();
    }
}
