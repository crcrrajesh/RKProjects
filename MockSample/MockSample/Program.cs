using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockSample
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public interface IFirmware
    {
        void SendCommand(string name);
        bool IsOkToSendCommand(string name);
        string GetLastExecutedCommand();
    }

    public class CommandProcessor
    {
        private readonly IFirmware _firmware;

        public CommandProcessor(IFirmware firmware)
        {
            _firmware = firmware;
        }

       

        public int SendCommand(string name)
        {
            if (_firmware.IsOkToSendCommand(name))
            {
                _firmware.SendCommand(name);
                return 1;
            }
            return 0;
        }
    }
}
