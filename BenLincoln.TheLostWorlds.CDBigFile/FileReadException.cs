using System;
using System.Collections.Generic;
using System.Text;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class FileReadException : System.ApplicationException
    {
        public FileReadException(string message)
            : base(message)
        {
        }
    }
}
