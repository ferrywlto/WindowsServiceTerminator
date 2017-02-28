using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Xml;
using System.Windows.Forms;

namespace ServiceManager
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    if (args[0] == "-l")
                    {
                        //list service mode
                        ServiceController[] services = ServiceController.GetServices();
                        foreach (ServiceController service in services)
                            Console.WriteLine("Service Name: " + service.ServiceName + " | Friendly Name: " + service.DisplayName);
                    }
                    else
                        Console.WriteLine("Invaild Parameters.");
                }
                else
                {
                    //close service mode
                    XmlDocument srvCloseDoc = new XmlDocument();
                    srvCloseDoc.PreserveWhitespace = false;
                    srvCloseDoc.Load(Application.StartupPath + @"\config.xml");
                    if (srvCloseDoc.InnerXml != null)
                    {
                        List<string> srvNames = new List<string>();
                        //get list of service to close
                        XmlNodeList srvCloseList = srvCloseDoc.DocumentElement.GetElementsByTagName("entry");
                        foreach (XmlNode node in srvCloseList)
                            srvNames.Add(node.Attributes["name"].Value);

                        ServiceController[] services = ServiceController.GetServices();
                        foreach (ServiceController service in services)
                        {
                            if (srvNames.Contains(service.ServiceName) && service.CanStop)
                            {
                                srvNames.Remove(service.ServiceName);
                                //service.Status = ServiceControllerStatus.Running
                                service.Stop();
                                service.WaitForStatus(ServiceControllerStatus.Stopped);
                                Console.WriteLine("Service: " + service.DisplayName + " has stopped.");
                            }

                        }
                        Console.WriteLine("All possible services has been stopped successfully.");
                    }
                    else
                    {
                        Console.WriteLine("File not found or invaild.");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
