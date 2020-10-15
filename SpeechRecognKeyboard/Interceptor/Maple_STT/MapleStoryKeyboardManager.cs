using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Interceptor.Maple_STT
{
    public class SpeechKeyboardManager
    {
        public static bool IsContextCreated = false;

        private IntPtr context;
        private int deviceId;
        private Thread KeyInputThread;

        public SpeechKeyboardManager()
        {
            InitKeyboardDriver();
            InitThread();
        }

        private void InitKeyboardDriver()
        {
            CreateContext();
            InterceptionDriver.SetFilter(context, InterceptionDriver.IsKeyboard, (int)KeyboardFilterMode.All);
        }

        #region Thread

        private void InitThread()
        {
            KeyInputThread = new Thread(new ThreadStart(ReceiveKeyboardEvent));
        }

        private void ReceiveKeyboardEvent()
        {
            Stroke stroke = new Stroke();

            while (InterceptionDriver.Receive(context,deviceId = InterceptionDriver.Wait(context),ref stroke,1) > 0)
            {
                InterceptionDriver.Send(context, deviceId, ref stroke, 1);
            }
        }

        public void StartKeyboardCapture()
        {
            KeyInputThread?.Start();
        }

        public void AbortKeyboardCapture()
        {
            KeyInputThread?.Join();
        }

        #endregion

        public void DestroyContext()
        {
            if (IsContextCreated == true)
            {
                InterceptionDriver.DestroyContext(context);
                IsContextCreated = false;
            }
        }

        public void CreateContext()
        {
            if(IsContextCreated == false)
            {
                context = InterceptionDriver.CreateContext();
                IsContextCreated = true;
            }
        }

        //PressKey메서드 실행전 반드시 실행해야함
        public void GetKeyboardId()
        {
            deviceId = InterceptionDriver.Wait(context);
        }

        public void PressKey(Keys key)
        {
            Stroke stroke = new Stroke();

            stroke.Key = new KeyStroke();
            stroke.Key.Code = key;

            InterceptionDriver.Send(context, deviceId, ref stroke, 1);
            Thread.Sleep(1);
            ReleaseKey(key);
        }

        public void ReleaseKey(Keys key)
        {
            Stroke stroke = new Stroke();
            stroke.Key = new KeyStroke();

            stroke.Key.Code = key;
            stroke.Key.State = KeyState.Up;

            InterceptionDriver.Send(context, deviceId, ref stroke, 1);
        }
    }
}
