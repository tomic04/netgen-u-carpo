//using System;
//using System.IO.Ports;
//using System.Text;

//public class SerialPortManager
//{
//    public SerialPort SerialPort { get; private set; }

//    //public void Initialize(string portName = "COM3", int baudRate = 9600)
//    //{
//    //    SerialPort = new SerialPort(portName, baudRate)
//    //    {
//    //        Parity = Parity.None,
//    //        StopBits = StopBits.One,
//    //        DataBits = 8,
//    //        Handshake = Handshake.None,
//    //        RtsEnable = true
//    //    };

//    //    SerialPort.DataReceived += SerialPort_DataReceived;
//    //    SerialPort.Open();
//    //}

//    //private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
//    //{
//    //    SerialPort sp = (SerialPort)sender;
//    //    string indata = sp.ReadExisting();
//    //    OnDataReceived(indata);
//    //}

//    public event Action<string> DataReceived;

//    //protected virtual void OnDataReceived(string data)
//    //{
//    //    DataReceived?.Invoke(data);
//    //}

//    //public void Close()
//    //{
//    //    if (SerialPort != null && SerialPort.IsOpen)
//    //    {
//    //        SerialPort.Close();
//    //    }
//    //}
//}