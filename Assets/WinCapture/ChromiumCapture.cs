using System;
using UnityEngine;
using System.Collections;
using System.Text;
//using System.Diagnostics;
using MessageLibrary;
using UnityEngine.UI;
using System.Collections.Generic;
using WinCapture;
using SimpleWebBrowser;

namespace WinCapture
{
    public class ChromiumCapture : IDisposable
    {
        BrowserEngine mainEngine;

        string MemoryFile = "MainSharedMem";

        int Port = 8885;

        public int windowWidth
        {
            get {
                return mainEngine.kWidth; }
            set { mainEngine.kWidth = value; }
        }
        public int windowHeight
        {
            get { return mainEngine.kHeight; }
            set { mainEngine.kHeight = value; }
        }

        public void resize()
        {
            mainEngine.BrowserTexture = new Texture2D(windowWidth, windowHeight, TextureFormat.BGRA32, false);
        }

        public ChromiumCapture(int width, int height, string url, bool enableWebRTC=true)
        {
            mainEngine = new BrowserEngine();

            bool RandomMemoryFile = true;

            if (RandomMemoryFile)
            {
                Guid memid = Guid.NewGuid();
                MemoryFile = memid.ToString();
            }

            bool RandomPort = true;

            if (RandomPort)
            {
                System.Random r = new System.Random();
                Port = 8000 + r.Next(1000);
            }



            mainEngine.InitPlugin(width, height, MemoryFile, Port, url, enableWebRTC);
            //run initialization
            //if (JSInitializationCode.Trim() != "")
            //    mainEngine.RunJSOnce(JSInitializationCode);
        }


        public void SetUrl(string url)
        {
            mainEngine.SendNavigateEvent(url, false, false);
        }

        bool isFirst = true;
        public Texture2D GetChromiumTexture(out bool didChange)
        {
            didChange = false;
            if (isFirst)
            {
                didChange = true;
                isFirst = false;
            }
            else if (mainEngine.BrowserTexture.width != mainEngine.kWidth || mainEngine.BrowserTexture.height != mainEngine.kHeight)
            {
                didChange = true;
                mainEngine.BrowserTexture = new Texture2D(mainEngine.kWidth, mainEngine.kHeight, TextureFormat.BGRA32, false);
            }

            mainEngine.UpdateTexture();
            return mainEngine.BrowserTexture;
        }


        public void Dispose()
        {
            Cleanup();
        }

        ~ChromiumCapture()
        {
            Cleanup();
        }

        object cleanupLock = new object();
        bool didCleanup = false;
        void Cleanup()
        {
            lock (cleanupLock)
            {
                if (!didCleanup)
                {
                    didCleanup = true;
                    mainEngine.Shutdown();
                }
            }
        }



        ////// This is all stuff you can use to let the user interact with the chromium window //////
        ////// I'll add an example of how to use it all in a bit                               //////


        
        public void insertText(string text)
        {
            foreach (char c in text)
            {
                mainEngine.SendCharEvent((int)c, KeyboardEventType.CharKey);
            }
            ProcessKeyEvents();
        }

        private void ProcessKeyEvents()
        {
            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                CheckKey(k);
            }
        }

        private void CheckKey(KeyCode code)
        {
            if (Input.GetKeyDown(code))
                mainEngine.SendCharEvent((int)code, KeyboardEventType.Down);
            if (Input.GetKeyUp(KeyCode.Backspace))
                mainEngine.SendCharEvent((int)code, KeyboardEventType.Up);
        }

        void LeftButtonDown(int x, int y)
        {
            if (mainEngine.Initialized)
                    SendMouseButtonEvent(x, y, MouseButton.Left, MouseEventType.ButtonDown);
        }

        void LeftButtonUp(int x, int y)
        {
            if (mainEngine.Initialized)
                    SendMouseButtonEvent(x, y, MouseButton.Left, MouseEventType.ButtonUp);
        }

        int posX, posY;

        void OnMouseHover(int x, int y, bool flagHoldLeft, bool flagHoldMiddle, bool flagHoldRight, 
            bool flagDownLeft, bool flagDownMiddle, bool flagDownRight, 
            bool flagUpLeft, bool flagUpMiddle, bool flagUpRight)
        {
            if (mainEngine.Initialized)
            {
                ProcessScrollInput(x, y);

                if (posX != x || posY != y)
                {
                    MouseMessage msg = new MouseMessage
                    {
                        Type = MouseEventType.Move,
                        X = x,
                        Y = y,
                        GenericType = MessageLibrary.BrowserEventType.Mouse,
                        // Delta = e.Delta,
                        Button = MouseButton.None
                    };

                    if (flagHoldLeft)
                        msg.Button = MouseButton.Left;
                    if (flagHoldRight)
                        msg.Button = MouseButton.Right;
                    if (flagHoldMiddle)
                        msg.Button = MouseButton.Middle;

                    posX = x;
                    posY = x;
                    mainEngine.SendMouseEvent(msg);
                }

                //check other buttons...
                if (flagDownRight)
                    SendMouseButtonEvent(x, y, MouseButton.Right, MouseEventType.ButtonDown);
                if (flagUpRight)
                    SendMouseButtonEvent(x, y, MouseButton.Right, MouseEventType.ButtonUp);
                if (flagDownMiddle)
                    SendMouseButtonEvent(x, y, MouseButton.Middle, MouseEventType.ButtonDown);
                if (flagUpMiddle)
                    SendMouseButtonEvent(x, y, MouseButton.Middle, MouseEventType.ButtonUp);
            }
            // Debug.Log(pixelUV);
        }


        private void SendMouseButtonEvent(int x, int y, MouseButton btn, MouseEventType type)
        {
            MouseMessage msg = new MouseMessage
            {
                Type = type,
                X = x,
                Y = y,
                GenericType = MessageLibrary.BrowserEventType.Mouse,
                // Delta = e.Delta,
                Button = btn
            };
            mainEngine.SendMouseEvent(msg);
        }

        private void ProcessScrollInput(int px, int py)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            scroll = scroll * mainEngine.BrowserTexture.height;

            int scInt = (int)scroll;

            if (scInt != 0)
            {
                MouseMessage msg = new MouseMessage
                {
                    Type = MouseEventType.Wheel,
                    X = px,
                    Y = py,
                    GenericType = MessageLibrary.BrowserEventType.Mouse,
                    Delta = scInt,
                    Button = MouseButton.None
                };

                if (Input.GetMouseButton(0))
                    msg.Button = MouseButton.Left;
                if (Input.GetMouseButton(1))
                    msg.Button = MouseButton.Right;
                if (Input.GetMouseButton(1))
                    msg.Button = MouseButton.Middle;

                mainEngine.SendMouseEvent(msg);
            }
        }

        private Vector2 GetScreenCoords()
        {
            RaycastHit hit;
            if (
                !Physics.Raycast(
                    Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                return new Vector2(-1f, -1f);
            Texture tex = mainEngine.BrowserTexture;


            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x = (1 - pixelUV.x) * tex.width;
            pixelUV.y *= tex.height;
            return pixelUV;
        }
        
    }
}