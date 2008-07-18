using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

using ALAZ.SystemEx.SocketsEx;

namespace EchoCryptService
{
    
    public class EchoCryptService : BaseCryptoService
    {

        #region Methods

        public override void OnSymmetricAuthenticate(HostType hostType, out RSACryptoServiceProvider serverKey)
        {

            /*
             * A RSACryptoServiceProvider is needed to encrypt and send session key.
             * In server side you need public and private key to decrypt session key.
             * In client side tou need only public key to encrypt session key.
             * 
             * You can create a RSACryptoServiceProvider from a string (file, registry), a CspParameters or a certificate.
             * The following certificate and instructions is in MakeCert folder.
             * 
            */

            //----- Using string!
            /*
             
            serverKey = new RSACryptoServiceProvider();
            serverKey.FromXMLString(<XML key string>);
             
            */

            //----- Using CspParameters!
            CspParameters param = new CspParameters();
            param.KeyContainerName = "ALAZ_ECHO_SERVICE";
            serverKey = new RSACryptoServiceProvider(param);

            /*
             
            //----- Using Certificate Store!
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2 certificate = store.Certificates.Find(X509FindType.FindBySubjectName, "ALAZ Library", true)[0];

            serverKey = new RSACryptoServiceProvider();

            if (hostType == HostType.htClient)
            {
                serverKey = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            }
            else
            {
                serverKey.FromXmlString(certificate.PrivateKey.ToXmlString(true));
            }

            store.Close();
             
            */

        }

        public override void OnSSLServerAuthenticate(out X509Certificate2 certificate, out bool clientAuthenticate, ref bool checkRevocation)
        {

            //----- Set server sertificate, client authentication and certificate revocation!
            //----- The following certificate and instructions is in MakeCert folder.

            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName, "ALAZ Library", false);

            certificate = certs[0];

            clientAuthenticate = false;
            checkRevocation = false;

            store.Close();

        }

        public override void OnSSLClientAuthenticate(out string serverName, ref X509Certificate2Collection certs, ref bool checkRevocation)
        {
            
            serverName = "ALAZ Library";

            /*
             
            //----- Using client certificate!
            //----- The following certificate and instructions is in MakeCert folder.

            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            certs = store.Certificates.Find(X509FindType.FindBySubjectName, serverName, true);
            checkRevocation = false;
             
            store.Close();
             
            */

        }

        #endregion

    }

}
