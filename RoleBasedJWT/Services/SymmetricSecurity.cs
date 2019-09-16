using System.Security.Cryptography.X509Certificates;

namespace RoleBasedJWT.Services
{
    internal class SymmetricSecurity : X509Certificate2
    {
        public SymmetricSecurity(byte[] rawData) : base(rawData)
        {
        }
    }
}