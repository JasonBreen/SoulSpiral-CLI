using System;
using System.Collections.Generic;
using System.Text;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class FileSizeMismatchException : System.ApplicationException
    {
        public FileSizeMismatchException(string message)
            : base(message)
        {
        }
    }
}
