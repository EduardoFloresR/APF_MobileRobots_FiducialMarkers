/*
	TUIO C# Example - part of the reacTIVision project
	http://reactivision.sourceforge.net/

	Copyright (c) 2005-2009 Martin Kaltenbrunner <martin@tuio.org>

	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Intcur., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using TUIO;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class TuioDump : TuioListener
{
    private Socket socket;
    private bool markerDetected = false;
    private TuioObject marker0 = null;
    private TuioObject marker1 = null;
    private float avgX = 0;
    private float avgY = 0;
    private int markerCount = 0;
    private float scaleY = 0;
    private float scaleX = 0;
    private double rotSys = 0;

    // Puerto escala
    private readonly string direccion_01 = "127.0.0.1";
    private readonly int puerto_01 = 58621;
    // Puerto robot
    private readonly string direccion_04 = "127.0.0.4";
    private readonly int puerto_04 = 58624;
    // Puerto campo
    private readonly string direccion_05 = "127.0.0.6";
    private readonly int puerto_05 = 58626;

    public void addTuioObject(TuioObject tobj)
    {
        Console.WriteLine("add obj ");// + tobj.SymbolID + " " + tobj.SessionID + " " + tobj.X + " " + tobj.Y + " " + tobj.Angle);
        markerDetected = true;

        if (tobj.SymbolID == 0 || tobj.SymbolID == 1)
        {
            // Sumar las coordenadas de los marcadores con ID 0 y 1
            avgX += tobj.X;
            avgY += tobj.Y;
            markerCount++;

            if (tobj.SymbolID == 0)
            {
                marker0 = tobj;
            }
            else if (tobj.SymbolID == 1)
            {
                marker1 = tobj;
            }

            if (markerCount == 2)
            {
                // Calcular la distancia entre los marcadores en el eje Y
                scaleY = 800 / Math.Abs(marker0.Y - marker1.Y);
                scaleX = scaleY * (4 / 3);
                rotSys = 1000*(Math.Atan2(marker0.Y - marker1.Y, marker0.X - marker1.X)-Math.PI/2);

                Console.WriteLine($"Escala: X{scaleX}, Y{scaleY}");

                // Calcular el promedio de las coordenadas
                avgX /= 2;
                avgY /= 2;

                avgX = avgX*1000; // * scaleX;
                avgY = avgY*1000; // * scaleY;
                // Imprimir el nuevo origen de coordenadas
                Console.WriteLine($"Nuevo origen de coordenadas: X={avgX}, Y={avgY}");

                // Restablecer las variables para dejar de imprimir información de los marcadores con índices 0 y 1
                marker0 = null;
                marker1 = null;

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress ipaddress = IPAddress.Parse(direccion_01);
                IPEndPoint endpoint = new IPEndPoint(ipaddress, puerto_01);

                // Formateamos los valores de posición y orientación como enteros de 5 cifras
                string rSys_str = rotSys.ToString("00000");
                string sY_str = scaleY.ToString("00000");
                string cX_str = avgX.ToString("00000");
                string cY_str = avgY.ToString("00000");

                string config = $"M{rSys_str},{sY_str},{cX_str},{cY_str}";
                Console.WriteLine(config);
                byte[] buffer = Encoding.ASCII.GetBytes(config);
                socket.SendTo(buffer, endpoint);
            }
        }
        else
        {
            PrintMarkerInfo(tobj);
            SendDataToUDP(tobj);
        }
    }

    public void updateTuioObject(TuioObject tobj)
    {
        Console.WriteLine("set obj ");// + tobj.SymbolID + " " + tobj.SessionID + " " + tobj.X + " " + tobj.Y + " " + tobj.Angle + " " + tobj.MotionSpeed + " " + tobj.RotationSpeed + " " + tobj.MotionAccel + " " + tobj.RotationAccel);
        PrintMarkerInfo(tobj);
        SendDataToUDP(tobj);
    }

    private void PrintMarkerInfo(TuioObject tobj)
    {
        //Console.WriteLine($"M{tobj.SymbolID},{tobj.X},{tobj.Y},{tobj.Angle}");
    }

    public void removeTuioObject(TuioObject tobj)
    {
        Console.WriteLine("del obj " + tobj.SymbolID + " " + tobj.SessionID);
        markerDetected = false;
        SendDataToUDP(tobj);
    }

    public void addTuioCursor(TuioCursor tcur)
    {
        Console.WriteLine("add cur " + tcur.CursorID + " (" + tcur.SessionID + ") " + tcur.X + " " + tcur.Y);
    }

    public void updateTuioCursor(TuioCursor tcur)
    {
        Console.WriteLine("set cur " + tcur.CursorID + " (" + tcur.SessionID + ") " + tcur.X + " " + tcur.Y + " " + tcur.MotionSpeed + " " + tcur.MotionAccel);
    }

    public void removeTuioCursor(TuioCursor tcur)
    {
        Console.WriteLine("del cur " + tcur.CursorID + " (" + tcur.SessionID + ")");
    }

    public void addTuioBlob(TuioBlob tblb)
    {
        Console.WriteLine("add blb " + tblb.BlobID + " (" + tblb.SessionID + ") " + tblb.X + " " + tblb.Y + " " + tblb.Angle + " " + tblb.Width + " " + tblb.Height + " " + tblb.Area);
    }

    public void updateTuioBlob(TuioBlob tblb)
    {
        Console.WriteLine("set blb " + tblb.BlobID + " (" + tblb.SessionID + ") " + tblb.X + " " + tblb.Y + " " + tblb.Angle + " " + tblb.Width + " " + tblb.Height + " " + tblb.Area + " " + tblb.MotionSpeed + " " + tblb.RotationSpeed + " " + tblb.MotionAccel + " " + tblb.RotationAccel);
    }

    public void removeTuioBlob(TuioBlob tblb)
    {
        Console.WriteLine("del blb " + tblb.BlobID + " (" + tblb.SessionID + ")");
    }

    public void refresh(TuioTime frameTime)
    {
        //Console.WriteLine("refresh " + frameTime.getTotalMilliseconds());
    }

    private void SendDataToUDP(TuioObject tobj)
    {
        // No enviar información si el ID es 0 o 1
        if (tobj.SymbolID == 0 || tobj.SymbolID == 1)
        {
            return;
        }
        else if (tobj.SymbolID == 4)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ipaddress = IPAddress.Parse(direccion_04);
            IPEndPoint endpoint = new IPEndPoint(ipaddress, puerto_04);

            float IDmod = tobj.SymbolID;
            float Xmod = tobj.X * 1000; 
            float Ymod = tobj.Y * 1000; 
            float Amod = tobj.Angle * 1000;

            // Formateamos los valores de posición y orientación como enteros de 3 cifras
            string ID_str = IDmod.ToString("00000");
            string x_str = Xmod.ToString("00000");
            string y_str = Ymod.ToString("00000");
            string ang_str = Amod.ToString("00000");

            // Construimos la cadena de mensaje
            string mensaje = $"M{ID_str},{x_str},{y_str},{ang_str}";
            Console.WriteLine(mensaje);
            byte[] buffer = Encoding.ASCII.GetBytes(mensaje);
            socket.SendTo(buffer, endpoint);
        }
        else if (tobj.SymbolID == 5)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ipaddress = IPAddress.Parse(direccion_05);
            IPEndPoint endpoint = new IPEndPoint(ipaddress, puerto_05);

            float IDmod = tobj.SymbolID;
            float Xmod = tobj.X * 1000;  // Restar el promedio de X
            float Ymod = tobj.Y * 1000;  // Restar el promedio de Y
            float Amod = tobj.Angle * 1000;

            // Formateamos los valores de posición y orientación como enteros de 3 cifras
            string ID_str = IDmod.ToString("00000");
            string x_str = Xmod.ToString("00000");
            string y_str = Ymod.ToString("00000");
            string ang_str = Amod.ToString("00000");

            // Construimos la cadena de mensaje
            string mensaje = $"M{ID_str},{x_str},{y_str},{ang_str}";
            Console.WriteLine(mensaje);
            byte[] buffer = Encoding.ASCII.GetBytes(mensaje);
            socket.SendTo(buffer, endpoint);
        }
    }

    public static void Main(String[] argv)
    {
        TuioDump demo = new TuioDump();
        TuioClient client = null;

        switch (argv.Length)
        {
            case 1:
                int port = 0;
                port = int.Parse(argv[0], null);
                if (port > 0) client = new TuioClient(port);
                break;
            case 0:
                client = new TuioClient();
                break;
        }

        if (client != null)
        {
            client.addTuioListener(demo);
            client.connect();
            Console.WriteLine("listening to TUIO messages at port " + client.getPort());
        }
        else
        {
            Console.WriteLine("usage: java TuioDump [port]");
        }
    }
}

