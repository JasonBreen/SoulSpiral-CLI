using System;
using System.Collections.Generic;
using System.Text;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class InvalidFileReferenceException : System.ApplicationException
    {
        public InvalidFileReferenceException(string message)
            : base(message)
        {
        }
    }
}
