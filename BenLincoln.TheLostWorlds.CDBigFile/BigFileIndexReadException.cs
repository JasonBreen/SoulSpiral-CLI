using System;
using System.Collections.Generic;
using System.Text;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class BigFileIndexReadException : System.ApplicationException
    {
        public BigFileIndexReadException(string message)
            : base(message)
        {
        }
    }
}
