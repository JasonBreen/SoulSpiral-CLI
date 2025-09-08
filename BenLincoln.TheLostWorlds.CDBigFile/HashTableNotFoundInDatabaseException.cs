using System;
using System.Collections.Generic;
using System.Text;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class HashTableNotFoundInDatabaseException : System.ApplicationException
    {
        public HashTableNotFoundInDatabaseException(string message)
            : base(message)
        {
        }
    }
}
