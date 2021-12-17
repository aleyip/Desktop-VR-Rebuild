using System;
using System.Collections;
using System.Collections.Generic;
using WinCapture;
using UnityEngine;

public class PaintApp : BaseApplication
{
    int lastHit;

    // Update is called once per frame
    new void Update()
    {
        if (pointer.activeObjectID == windowObject.GetInstanceID())
        {
            var windChild = Win32Funcs.FindWindowEx(windowsRender.windowInfo.hwnd, IntPtr.Zero, "MSPaintView", null);
            //parte do principio que a primeira janela child é a que precisamos pra desenhar
            windChild = Win32Funcs.FindWindowEx(windChild, IntPtr.Zero, null, null);
            if (windChild != IntPtr.Zero)
            {
                int foo;

                Int32 upLeftPos;
                {
                    //Pega a posicao do canto superior esquerdo do canvas para posicionar corretamente o cursor
                    Win32Types.RECT upLeftWindow, upLeftCanvas;
                    Win32Funcs.GetWindowRect(windowsRender.windowInfo.hwnd, out upLeftWindow);
                    Win32Funcs.GetWindowRect(windChild, out upLeftCanvas);
                    Int32 xBorder = upLeftCanvas.Left - upLeftWindow.Left;
                    Int32 yBorder = upLeftCanvas.Top - upLeftWindow.Top;
                    upLeftPos = (Int32)((yBorder << 16) | xBorder);
                }
                if (mousePosChanged)
                {
                    foo = Win32Funcs.PostMessage(windChild, Win32Types.command.WM_MOUSEMOVE, 0, oldMousePos - upLeftPos);
                    //Debug.Log(foo);
                    mousePosChanged = false;
                }

                if (pointer.mouseLeftDown)
                {
                    Debug.Log("Pressed primary button");
                    foo = Win32Funcs.PostMessage(windChild, Win32Types.command.WM_LBUTTONDOWN, 0, oldMousePos - upLeftPos);
                    //Debug.Log(foo);
                }
                else if (pointer.mouseLeftUp)
                {
                    Debug.Log("Unpressed primary button");
                    foo = Win32Funcs.PostMessage(windChild, Win32Types.command.WM_LBUTTONUP, 0, oldMousePos - upLeftPos);
                    //Debug.Log(foo);
                }
                else if (Input.anyKeyDown)
                {
                    string str = Input.inputString;
                    //Debug.Log("Trying click " + str);
                    foreach (char c in str)
                    {
                        foo = Win32Funcs.PostMessage(windChild, Win32Types.command.WM_CHAR, c, 0x0001);
                        //Debug.Log(foo);
                    }
                }
            }
            else
                Debug.Log("FALSE");
        }


        base.Update();
    }
}
