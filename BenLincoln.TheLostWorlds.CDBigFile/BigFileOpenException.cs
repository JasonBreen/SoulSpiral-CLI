using System;
using System.Collections.Generic;
using System.Text;
using BF = BenLincoln.TheLostWorlds.CDBigFile;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class BigFileOpenException : System.ApplicationException
    {
        public BigFileOpenException(string message)
            : base(message)
        {
        }
    }
}
