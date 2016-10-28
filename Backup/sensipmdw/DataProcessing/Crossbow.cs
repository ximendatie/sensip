using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms ;
using System.Threading;
using SYRIS_XTIVE_STANDARD_READER_1N;
using System.IO.Ports;


namespace Sensip
{
    public class CBData
    {
        string message ;
        public CBData(string msg)
        {
            message = msg;
        }
        public string getData()
        {
            return message;
        }
    };


    

    class Crossbow : DataProcessing<CBData>
    {
        
        bool _isRunnin ;

        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        static bool _continue;
        static SerialPort _serialPort;

        string portName;


        public static string SetPortName(string defaultPortName)
        {
            return "COM19";
        }

        public static int SetPortBaudRate(int defaultPortBaudRate)
        {
            return 57600;
        }

        public static Parity SetPortParity(Parity defaultPortParity)
        {
            return defaultPortParity;
        }

        public static int SetPortDataBits(int defaultPortDataBits)
        {
            return defaultPortDataBits;
        }

        public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            return defaultPortStopBits;
        }

        public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            return defaultPortHandshake;
        }

        public Crossbow(string cPort)
        {
            portName = cPort;
            _serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            _serialPort.PortName = SetPortName(_serialPort.PortName);
            _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
            _serialPort.Parity = SetPortParity(_serialPort.Parity);
            _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _serialPort.Open();
            _continue = true;
            _isRunnin = true;
        }


        public bool CheckStatus()
        {
            return _isRunnin;
        }


        public override void Producer()
        {
            while (!_bStop)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    CBData cbd = new CBData(message);
                    ProducerData(cbd);
                }
                catch (TimeoutException) { }

                //Thread.Sleep(cycleSleepTime);
            }

            return;
        }

        public override void Consumer()
        {
            while (!_bStop)
            {
                CBData cbd = ConsumerData();
                string msg = cbd.getData();
                Console.Write("DATA:", msg+"\n");

            }
            return;
            
        }
        

    }
}
