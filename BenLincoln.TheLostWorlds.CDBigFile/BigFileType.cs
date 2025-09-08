using System;
using System.Collections.Generic;
using System.Text;
using BF = BenLincoln.TheLostWorlds.CDBigFile;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class BigFileType
    {
        protected string mName;
        protected string mDescription;
        protected int mMasterIndexType;
        protected BF.FileType[] mFileTypes;
        protected BF.HashLookupTable mHashLookupTable;

        #region Properties

        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        public string Description
        {
            get
            {
                return mDescription;
            }
            set
            {
                mDescription = value;
            }
        }

        public int MasterIndexType
        {
            get
            {
                return mMasterIndexType;
            }
            set
            {
                mMasterIndexType = value;
            }
        }

        public BF.FileType[] FileTypes
        {
            get
            {
                return mFileTypes;
            }
            set
            {
                mFileTypes = value;
            }
        }

        public BF.HashLookupTable HashLookupTable
        {
            get
            {
                return mHashLookupTable;
            }
            set
            {
                mHashLookupTable = value;
            }
        }

        #endregion

        public BigFileType()
        {
            mName = "Unknown";
            mDescription = "Unknown BigFile type";
            mMasterIndexType = 0;
            mHashLookupTable = null;
        }

    }
}
