using System;
using System.Collections.Generic;
using System.Text;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    class DecompressionException : System.ApplicationException
    {
        public DecompressionException(string message)
            : base(message)
        {
        }
    }
}
