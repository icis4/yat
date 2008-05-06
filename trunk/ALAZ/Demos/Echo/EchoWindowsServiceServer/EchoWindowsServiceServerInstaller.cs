using System.ServiceProcess;
using System.ComponentModel;
using System.Configuration.Install;

[RunInstaller(true)]
public class EchoWindowsServiceServerInstaller : Installer
{

    private ServiceProcessInstaller processInstaller;
    private ServiceInstaller serviceInstaller;

    public EchoWindowsServiceServerInstaller()
    {

        processInstaller = new ServiceProcessInstaller();
        serviceInstaller = new ServiceInstaller();

        processInstaller.Account = ServiceAccount.LocalSystem;
        serviceInstaller.StartType = ServiceStartMode.Manual;
        serviceInstaller.ServiceName = "EchoWindowsServiceServer";

        Installers.Add(serviceInstaller);
        Installers.Add(processInstaller);

    }

}

