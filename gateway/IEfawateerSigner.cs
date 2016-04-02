using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Gateways
{
    public interface IEfawateerSigner
    {
        void CheckCerificate();
        string SignData(string body);
        bool VerifyData(string toString, byte[] signature);
    }

    
}