using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Security
{
    public interface IHashingService
    {
        (byte[] Hash, byte[] Salt) Hash(string input);
        bool Verify(string input, byte[] hash, byte[] salt);
    }
}
