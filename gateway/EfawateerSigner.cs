using Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EfawateerGateway
{
    public class EfawateerSigner : IEfawateerSigner
    {
        private readonly X509Certificate2 _signCertificate;
        private readonly X509Certificate2 _verifyCertificate;

        public EfawateerSigner(string signCertThumb, string verifyCertThumb)
        {
            _signCertificate = GetCertificate(signCertThumb);
            _verifyCertificate = GetCertificate(verifyCertThumb);
        }

        public void CheckCerificate()
        {
            if (!_signCertificate.HasPrivateKey)
                throw new Exception("Отсутствует закрытый ключ в сертификате для подписи");
        }


        private X509Certificate2 GetCertificate(string thumbprint)
        {
            X509Store store = new X509Store(StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (certs.Count > 0)
                    return certs[0];

                store.Close();
                store = new X509Store(StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (certs.Count > 0)
                    return certs[0];
                
                throw new ApplicationException("valid certificate not found with thumbprint: " + thumbprint);

            }
            finally
            {
                store.Close();
            }
        }



        public string SignData(string body)
        {
            var rsaProv = (RSACryptoServiceProvider)_signCertificate.PrivateKey;
            byte[] data = rsaProv.SignData(Encoding.Unicode.GetBytes(body), CryptoConfig.MapNameToOID("sha256"));
            return Convert.ToBase64String(data);
        }

        public bool VerifyData(string toString, byte[] signature)
        {
            var rsaProv = (RSACryptoServiceProvider)_verifyCertificate.PublicKey.Key;
            return rsaProv.VerifyData(Encoding.Unicode.GetBytes(toString), CryptoConfig.MapNameToOID("sha256"), signature);
        }
    }
}
