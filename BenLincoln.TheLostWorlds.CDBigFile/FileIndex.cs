using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BF = BenLincoln.TheLostWorlds.CDBigFile;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class FileIndex : BF.Index
    {
        protected int mNameHashPosition;
        protected int mLengthPosition;
        //the raw dwords from the index reference for this index - used only for multi-index types
        protected uint[] mRawIndexData;

        #region Properties

        public int NameHashPosition
        {
            get
            {
                return mNameHashPosition;
            }
            set
            {
                mNameHashPosition = value;
            }
        }

        public int LengthPosition
        {
            get
            {
                return mLengthPosition;
            }
            set
            {
                mLengthPosition = value;
            }
        }

        public uint[] RawIndexData
        {
            get
            {
                return mRawIndexData;
            }
            set
            {
                mRawIndexData = value;
            }
        }

        #endregion

        public FileIndex(string name, BF.BigFile parentBigFile, BF.Index parentIndex, long offset)
            : base(name, parentBigFile, parentIndex, offset)
        {
        }

        public override void ReadIndex()
        {
            ReadEntries();
            int numFiles = mEntries.GetUpperBound(0) + 1;
            mFileCount = numFiles;
            if (IsValidIndex)
            {
                Files = new BF.File[numFiles];
                for (int i = 0; i < numFiles; i++)
                {
                    BF.File tFile = new BF.File(mParentBigFile, this, mEntries[i], mNameHashPosition, mOffsetPosition, mLengthPosition);
                    Files[i] = tFile;
                    if (i > 0)
                    {
                        mLoadedPercent = (((float)i / (float)numFiles) * READ_CONTENT_PERCENT) + READ_INDEX_PERCENT;
                    }
                }
            }
        }
    }
}
