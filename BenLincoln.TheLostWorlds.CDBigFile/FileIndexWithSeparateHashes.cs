using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BF = BenLincoln.TheLostWorlds.CDBigFile;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class FileIndexWithSeparateHashes : BF.FileIndex
    {
        protected uint[] mHashes;
        protected int mHashOffset = 0;

        #region Properties

        public uint[] Hashes
        {
            get
            {
                return mHashes;
            }
            set
            {
                mHashes = value;
            }
        }

        public int HashOffset
        {
            get
            {
                return mHashOffset;
            }
            set
            {
                mHashOffset = value;
            }
        }

        #endregion

        public FileIndexWithSeparateHashes(string name, BF.BigFile parentBigFile, BF.Index parentIndex, long offset)
            : base(name, parentBigFile, parentIndex, offset)
        {
        }

        public override void ReadIndex()
        {
            ReadEntries();
            int numFiles = mEntries.GetUpperBound(0) + 1;
            Files = new BF.File[numFiles];
            for (int i = 0; i < numFiles; i++)
            {
                BF.File tFile = new BF.File(mParentBigFile, this, mEntries[i], mHashes[i], mOffsetPosition, mLengthPosition);
                Files[i] = tFile;
                if (i > 0)
                {
                    mLoadedPercent = (((float)i / numFiles) * READ_CONTENT_PERCENT) + READ_INDEX_PERCENT;
                }
            }
        }

        protected override void ReadEntries()
        {
            uint[] hashes;
            uint[][] entries;
            FileStream iStream;
            BinaryReader iReader;

            try
            {
                iStream = new FileStream(ParentBigFile.Path, FileMode.Open, FileAccess.Read);
                iReader = new BinaryReader(iStream);
                iStream.Seek(Offset, SeekOrigin.Begin);

                //get the number of entries in the index
                int numEntries = iReader.ReadUInt16();
                //proceed to read the rest of the index - 4 bytes past the length indicator
                iStream.Seek(Offset + mFirstEntryOffset, SeekOrigin.Begin);
                hashes = new uint[numEntries];
                for (int i = 0; i < numEntries; i++)
                {
                    hashes[i] = iReader.ReadUInt32();
                }

                entries = new uint[numEntries][];
                for (int i = 0; i < numEntries; i++)
                {
                    entries[i] = new uint[mEntryLength];
                    for (int j = 0; j < mEntryLength; j++)
                    {
                        entries[i][j] = iReader.ReadUInt32();
                    }
                }

                iReader.Close();
                iStream.Close();
            }
            catch (Exception ex)
            {
                throw new BigFileIndexReadException
                    ("Failed to read index " + Name + "\r\n" +
                    "The specific error message is: \r\n" +
                    ex.Message
                    );
            }
            mHashes = hashes;
            mEntries = entries;
        }

    }
}
