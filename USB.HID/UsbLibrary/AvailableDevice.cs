using System;
using System.Collections.Generic;
using System.Text;

namespace UsbLibrary
{
    public class AvailableDevice : HIDDevice
    {
        public event DataRecievedEventHandler DataRecieved;
        public event DataSendEventHandler DataSend;

        public override InputReport CreateInputReport()
        {
            return new SpecifiedInputReport(this);
        }

        public static List<AvailableDevice> FindAvailableDevices()
        {
            // Retrieve and convert available devices.
            // The following manual conversion could be implemented more elegantly using
            // "Variance in Generic Types" as described in from "C# Programming Guide"
            List<AvailableDevice> castedDevices = new List<AvailableDevice>();
            List<HIDDevice> devices = FindDevices(typeof(AvailableDevice));

            foreach (HIDDevice device in devices)
                castedDevices.Add((AvailableDevice)device);

            return castedDevices;
        }

        protected override void HandleDataReceived(InputReport oInRep)
        {
            // Fire the event handler if assigned
            if (DataRecieved != null)
            {
                SpecifiedInputReport report = (SpecifiedInputReport)oInRep;
                DataRecieved(this, new DataRecievedEventArgs(report.Data));
            }
        }

        public void SendData(byte[] data)
        {
            SpecifiedOutputReport oRep = new SpecifiedOutputReport(this);	// create output report
            oRep.SendData(data);	// set the lights states
            try
            {
                Write(oRep); // write the output report
                if (DataSend != null)
                {
                    DataSend(this, new DataSendEventArgs(data));
                }
            }catch (HIDDeviceException)
            {
                // Device may have been removed!
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected override void Dispose(bool bDisposing)
        {
            if (bDisposing)
            {
                // to do's before exit
            }
            base.Dispose(bDisposing);
        }

    }
}
