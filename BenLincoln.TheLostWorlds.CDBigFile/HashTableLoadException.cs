using System;
using System.Collections.Generic;
using System.Text;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class HashTableLoadException : System.ApplicationException
    {
        public HashTableLoadException(string message)
            : base(message)
        {
        }
    }
}
