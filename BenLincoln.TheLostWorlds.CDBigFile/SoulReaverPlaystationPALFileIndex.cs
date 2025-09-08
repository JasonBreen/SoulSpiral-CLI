using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BF = BenLincoln.TheLostWorlds.CDBigFile;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class SoulReaverPlaystationPALFileIndex : BF.FileIndex
    {
        public SoulReaverPlaystationPALFileIndex(string name, BF.BigFile parentBigFile, BF.Index parentIndex, long offset)
            : base(name, parentBigFile, parentIndex, offset)
        {
        }

        //for the PAL Playstation version of SR
        protected override void AlternateIndexRead()
        {
            uint[][] entries;
            FileStream iStream;
            BinaryReader iReader;
            iStream = new FileStream(ParentBigFile.Path, FileMode.Open, FileAccess.Read);
            iReader = new BinaryReader(iStream);
            iStream.Seek(Offset, SeekOrigin.Begin);

            //get the number of entries in the index
            //B722 is the magic number for this file type
            int numEntries = iReader.ReadUInt16() ^ 0xB722;
            if (numEntries > MAX_ENTRIES)
            {
                iReader.Close();
                iStream.Close();
                mIsValidIndex = false;
            }
            else
            {
                //proceed to read the rest of the index - 4 bytes past the length indicator
                //XOR them with 0xB722B722 because someone made the PAL version strange
                entries = new uint[numEntries][];
                iStream.Seek(Offset + mFirstEntryOffset, SeekOrigin.Begin);
                for (int i = 0; i < numEntries; i++)
                {
                    entries[i] = new uint[mEntryLength];
                    for (int j = 0; j < mEntryLength; j++)
                    {
                        entries[i][j] = iReader.ReadUInt32() ^ 0xB722B722;
                    }
                    if (i > 0)
                    {
                        mLoadedPercent = (((float)i / numEntries) * READ_INDEX_PERCENT);
                    }
                }
                iReader.Close();
                iStream.Close();
                mEntries = entries;
            }
        }
    }
}
