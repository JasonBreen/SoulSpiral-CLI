using System;
using System.Collections.Generic;
using System.Text;
using BF = BenLincoln.TheLostWorlds.CDBigFile;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class SoulReaver2AirForgeDemoFileIndex : BF.FileIndex
    {
        protected int mCompressedLengthPosition;

        #region Properties

        public int CompressedLengthPosition
        {
            get
            {
                return mCompressedLengthPosition;
            }
            set
            {
                mCompressedLengthPosition = value;
            }
        }

        #endregion

        public SoulReaver2AirForgeDemoFileIndex(string name, BF.BigFile parentBigFile, BF.Index parentIndex, long offset)
            : base(name, parentBigFile, parentIndex, offset)
        {
            mCompressedLengthPosition = 3;
        }

        public override void ReadIndex()
        {
            ReadEntries();
            int numFiles = mEntries.GetUpperBound(0) + 1;
            Files = new BF.File[numFiles];
            for (int i = 0; i < numFiles; i++)
            {
                BF.File tFile;
                if (mEntries[i][mCompressedLengthPosition] > 0)
                {
                    tFile = new BF.CompressedFile(mParentBigFile, this, mEntries[i], mNameHashPosition, mOffsetPosition, mLengthPosition, mCompressedLengthPosition);
                }
                else
                {
                    tFile = new BF.File(mParentBigFile, this, mEntries[i], mNameHashPosition, mOffsetPosition, mLengthPosition);
                }
                Files[i] = tFile;
                if (i > 0)
                {
                    mLoadedPercent = (((float)i / (float)numFiles) * READ_CONTENT_PERCENT) + READ_INDEX_PERCENT;
                }
            }
        }

    }
}
