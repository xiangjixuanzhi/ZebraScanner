using CoreScanner;
using System;

namespace BojayScanner
{
    public class CoreScannerClass
    {
        //Instantiate CoreScanner Class
        CCoreScannerClass cCoreScannerClass = new CCoreScannerClass();

        public static bool flag = true;
        public static string myscanner_data = " ";
        public static int Status = 1;
        public static int opcode_reg = 1001;
        public static int opcode_pull = 2011;
        public static int opcode_release = 2012;
        public static string outxml = " ";

        public int OpenScanner()
        {
            //Call Open API
            short[] scannerTypes = new short[1];        // Scanner Types you are interested in
            scannerTypes[0] = 1;                        // 1 for all scanner types, 2 for SNAPI model
            short numberOfScannerTypes = 1;             // Size of the scannerTypes array
            int status = Status;                        // Extended API return code
            cCoreScannerClass.Open(0, scannerTypes, numberOfScannerTypes, out status);
            if (status == 0)
            {
                //Console.WriteLine("CoreScanner API: Open Successful");
                return 0;
            }
            else
            {
                //Console.WriteLine("CoreScanner API: Open Failed");
                return -1;
            }
        }


        public int CloseScanner()
        {
            int status = Status;                            
            cCoreScannerClass.Close(0, out status);
            if (status == 0)
            {
                //Console.WriteLine("CoreScanner API: Close Successful");
                return 0;
            }
            else
            {
                //Console.WriteLine("CoreScanner API: Close Failed");
                return -1;
            }
        }


        public static void OnBarcodeEvent(short eventType, ref string pscanData)
        {
            string barcode = pscanData;
            myscanner_data = barcode;
            if (myscanner_data != " ")
            {
                flag = false;
            }
            //Console.WriteLine(barcode);

        }


        public string PullTrigger(int timeout)
        {
            //Register event for barcode
            int status = Status;
            string inXML = "<inArgs>" +
            "<cmdArgs>" +
            "<arg-int>1</arg-int>" +            // Number of events you want to subscribe
            "<arg-int>1</arg-int>" +            // Comma separated event IDs
            "</cmdArgs>" +
            "</inArgs>";
            cCoreScannerClass.ExecCommand(opcode_reg, ref inXML, out outxml, out status);
            //Console.WriteLine(outXML);
            if (status != 0)
            {
                //Console.WriteLine("CoreScanner API: ExecCommand register Successful");
                return "Register event for barcode fail";
            }

            // pull triger
            int status_pull = Status;
            string inXML_pull = "<inArgs>" +
            "<scannerID>1</scannerID>" +    // Specified Scanner ID
            "</inArgs>";
            cCoreScannerClass.ExecCommand(opcode_pull, ref inXML_pull, out outxml, out status_pull);
            //Console.WriteLine(outXML);
            if (status_pull != 0)
            {
                //Console.WriteLine("DEVICE_PULL_TRIGGER Successful!");
                return "pull trigger fail";
            }

            // Subscribe for barcode events in cCoreScannerClass
            cCoreScannerClass.BarcodeEvent += new
            _ICoreScannerEvents_BarcodeEventEventHandler(OnBarcodeEvent);
            // Let's subscribe for events  

            // Return
            //获取当前Ticks
            long currentTicks = DateTime.Now.Ticks;
            DateTime dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long currentMillis = (currentTicks - dtFrom.Ticks) / 10000;

            while (flag)
            {
                //获取当前Ticks1
                long currentTicks1 = DateTime.Now.Ticks;
                DateTime dtFrom1 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                long currentMillis1 = (currentTicks1 - dtFrom.Ticks) / 10000;
                if ((currentMillis1 - currentMillis) > (timeout*1000))
                {
                    return "timeout";
                }
                /*
                if (myscanner_data == " ")
                {
                    return "timeout";
                }
                else
                {
                    return myscanner_data;
                }*/
            }
            return myscanner_data;
            //Thread.Sleep(timeout * 1000);
            //ReleaseTrigger();

        }



        public int ReleaseTrigger()
        {
            // Release trigger
            int status = Status;
            string inXML = "<inArgs>" +
            "<scannerID>1</scannerID>" +    // Specified Scanner ID
            "</inArgs>";
            cCoreScannerClass.ExecCommand(opcode_release, ref inXML, out outxml, out status);
            //Console.WriteLine(outXML);
            if (status == 0)
            {
                return 0;
                //Console.WriteLine("DEVICE_PULL_TRIGGER Successful!");
            }
            else
            {
                return -1;
                //Console.WriteLine("DEVICE_PULL_TRIGGER Failed!");
            }
        }
    }
}
