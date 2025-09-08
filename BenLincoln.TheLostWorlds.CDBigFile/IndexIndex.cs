using System;
using System.Collections.Generic;
using System.Text;
using BF = BenLincoln.TheLostWorlds.CDBigFile;
using System.Threading;
using System.IO;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class IndexIndex : BF.Index
    {
        protected int mSubIndexType;

        #region Properties

        public int SubIndexType
        {
            get
            {
                return mSubIndexType;
            }
            set
            {
                mSubIndexType = value;
            }
        }

        #endregion

        public IndexIndex(string name, BF.BigFile parentBigFile, BF.Index parentIndex, long offset)
            : base(name, parentBigFile, parentIndex, offset)
        {
            mSubIndexType = 0;
        }

        public override void ReadIndex()
        {
            ReadEntries();
            int numIndices = mEntries.GetUpperBound(0) + 1;
            Indices = new BF.Index[numIndices];
            mFileCount = 0;
            for (int i = 0; i <= mEntries.GetUpperBound(0); i++)
            {
                BF.FileIndex tIndex = (BF.FileIndex)BF.Index.CreateIndex(mParentBigFile, mSubIndexType);
                tIndex.Name = "Sub-Index-" + string.Format("{0:D3}", i);
                tIndex.RawIndexData = mEntries[i];
                tIndex.Offset = mEntries[i][mOffsetPosition];
                //read subindex
                if (i > 0)
                {
                    mLoadedPercent = (((float)i / (float)mEntries.GetUpperBound(0)) * READ_CONTENT_PERCENT) + READ_INDEX_PERCENT;
                }
                //launch another sub-thread so that the loaded percent stays updated
                Thread siThread = new Thread(new ThreadStart(tIndex.ReadIndex));
                siThread.Start();
                do
                {
                    Thread.Sleep(1);
                }
                while (siThread.IsAlive);
                //tIndex.ReadIndex();
                Indices[i] = tIndex;
                mFileCount += tIndex.FileCount;
            }
        }
    }
}
