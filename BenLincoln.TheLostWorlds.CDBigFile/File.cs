using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BF = BenLincoln.TheLostWorlds.CDBigFile;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class File : IComparable
    {
        protected string mName;
        protected string mDirectoryName;
        protected string mFileExtension;
        protected BF.BigFile mParentBigFile;
        //the raw dwords from the index reference for this file
        protected uint[] mRawIndexData;
        protected byte[] mRawHeaderData;
        //offset in the parent bigfile
        protected long mOffset;
        //length in the bigfile (NOT uncompressed length for compressed files)
        protected int mLength;
        protected uint mHashedName;
        protected BF.Index mParentIndex;
        protected BF.Directory mParentDirectory;
        protected BF.FileType mType;
        protected bool mCanBeReplaced;
        protected bool mIsValidReference;

        //number of bytes of the header to read
        //shouldn't be too large because there will be a LOT of these
        protected const int RAWHEADERSIZE = 16;

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

        public string DirectoryName
        {
            get
            {
                return mDirectoryName;
            }
            set
            {
                mDirectoryName = value;
            }
        }

        public string FileExtension
        {
            get
            {
                return mFileExtension;
            }
            set
            {
                mFileExtension = value;
            }
        }


        public BF.BigFile ParentBigFile
        {
            get
            {
                return mParentBigFile;
            }
            set
            {
                mParentBigFile = value;
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

        public byte[] RawHeaderData
        {
            get
            {
                return mRawHeaderData;
            }
            set
            {
                mRawHeaderData = value;
            }
        }

        public long Offset
        {
            get
            {
                return mOffset;
            }
            set
            {
                mOffset = value;
            }
        }

        public int Length
        {
            get
            {
                return mLength;
            }
            set
            {
                mLength = value;
            }
        }

        public uint HashedName
        {
            get
            {
                return mHashedName;
            }
            set
            {
                mHashedName = value;
            }
        }

        public BF.Index ParentIndex
        {
            get
            {
                return mParentIndex;
            }
            set
            {
                mParentIndex = value;
            }
        }

        public BF.Directory ParentDirectory
        {
            get
            {
                return mParentDirectory;
            }
            set
            {
                mParentDirectory = value;
            }
        }

        public BF.FileType Type
        {
            get
            {
                return mType;
            }
            set
            {
                mType = value;
            }
        }

        public bool CanBeReplaced
        {
            get
            {
                return mCanBeReplaced;
            }
            set
            {
                mCanBeReplaced = value;
            }
        }

        public bool IsValidReference
        {
            get
            {
                return mIsValidReference;
            }
            set
            {
                mIsValidReference = value;
            }
        }


        #endregion

        public File(BF.BigFile parent, BF.Index parentIndex, uint[] rawIndexData, int hashNamePosition, int offsetPosition, int lengthPosition)
        {
            mParentBigFile = parent;
            mParentIndex = parentIndex;
            mRawIndexData = rawIndexData;
            mHashedName = rawIndexData[hashNamePosition];
            mOffset = rawIndexData[offsetPosition] * mParentIndex.OffsetMultiplier;
            mLength = (int)rawIndexData[lengthPosition];
            CheckFileDataForSanity();
            if (mIsValidReference)
            {
                GetHeaderData();
                mType = GetFileType();
                mCanBeReplaced = true;
            }
            else
            {
                mType = BF.FileType.FromType(BF.FileType.FILE_TYPE_Invalid);
            }
            GetNameComponents();
        }

        public File(BF.BigFile parent, BF.Index parentIndex, uint[] rawIndexData, uint hashedName, int offsetPosition, int lengthPosition)
        {
            mParentBigFile = parent;
            mParentIndex = parentIndex;
            mRawIndexData = rawIndexData;
            mHashedName = hashedName;
            mOffset = rawIndexData[offsetPosition] * mParentIndex.OffsetMultiplier;
            mLength = (int)rawIndexData[lengthPosition];
            CheckFileDataForSanity();
            if (mIsValidReference)
            {
                GetHeaderData();
                mType = GetFileType();
                mCanBeReplaced = true;
            }
            else
            {
                mType = BF.FileType.FromType(BF.FileType.FILE_TYPE_Invalid);
            }
            GetNameComponents();
        }

        protected virtual void CheckFileDataForSanity()
        {
            mIsValidReference = true;

            if (mOffset < 0)
            {
                mIsValidReference = false;
            }
            if (mLength < 1)
            {
                mIsValidReference = false;
            }
            if (mOffset > mParentBigFile.FileSize)
            {
                mIsValidReference = false;
            }
            if (mLength > mParentBigFile.FileSize)
            {
                mIsValidReference = false;
            }
            if ((mOffset + mLength) > mParentBigFile.FileSize)
            {
                mIsValidReference = false;
            }

            if (!mIsValidReference)
            {
                mCanBeReplaced = false;
            }
            //if (isBadReference)
            //{
            //    throw new InvalidFileReferenceException
            //        (
            //        "File " + mName + " is defined using invalid data in the index." + 
            //        "Offset read as: " + mOffset + ", Length read as: " + mLength
            //        );
            //}
        }

        protected void GetHeaderData()
        {
            FileStream iStream;
            //if the file is shorter than the standard header length, don't read more bytes than exist in the file
            int headerSize = Math.Min(RAWHEADERSIZE, mLength);
            mRawHeaderData = new byte[headerSize];

            iStream = new FileStream(mParentBigFile.Path, FileMode.Open, FileAccess.Read);
 
            iStream.Seek(mOffset, SeekOrigin.Begin);
            iStream.Read(mRawHeaderData, 0, headerSize);

            iStream.Close();
        }

        protected void GetNameComponents()
        {
            string name = "";
            string path = "";
            string ext = "";
            string baseName = GetFileName();

            //if the name has an extension already, it takes precedence
            if (baseName.Contains("."))
            {
                int last = baseName.LastIndexOf('.') +1;
                ext = baseName.Substring(last, baseName.Length - last);
                //remove the extension from the base name so it doesn't interfere with the next step
                //also remove the unnecessary dot
                baseName = baseName.Substring(0, last - 1);
            }
            //otherwise, get the extension from the file type
            else
            {
                ext = mType.FileExtension;
            }

            //if the name has a path already, it takes precedence
            if (baseName.Contains("\\"))
            {
                int last = baseName.LastIndexOf('\\') +1;
                path = baseName.Substring(0, last);
                //if the path starts with a backslash, remove it for consistency
                if (path.Substring(0, 1) == "\\")
                {
                    path = path.Substring(1, path.Length - 1);
                }
                //remove the path from the basename so we're left with just the filename
                baseName = baseName.Substring(last, baseName.Length - last);
            }
            //otherwise, set to default
            else
            {
                path = "Default\\";
            }

            name = baseName;

            //set all the instance variables
            mName = name.Trim();
            mDirectoryName = path.Trim();
            mFileExtension = ext.Trim();
        }

        protected virtual string GetFileName()
        {
            string name = "";
            if (!mIsValidReference)
            {
                name = "Invalid-" + string.Format("{0:X8}", mHashedName);
                return name.Trim();
            }
            //check the bigfile for a hash table first
            if (mParentBigFile.HashLookupTable != null)
            {
                name = mParentBigFile.HashLookupTable.LookupHash(mHashedName);
            }

            //if the file type supports it, read the internal file name
            if ((name == "") || (name == null))
            {
                name = mType.GetInternalName(this);
            }

            //if the name is still unknown, name it after the hash
            if ((name == "") || (name == null))
            {
                name = string.Format("{0:X8}", mHashedName);
            }

            return name.Trim();
        }

        protected virtual BF.FileType GetFileType()
        {
            BF.FileType ftUnknown = new FileType();

            foreach (BF.FileType type in mParentBigFile.FileTypes)
            {
                if (type.CheckType(this))
                {
                    return type;
                }
            }
            return ftUnknown;
        }

        public virtual string GetInfo()
        {
            string info = GetGenericInfo() + GetRawIndexInfo() + GetRawHeaderInfo();
            if (!mIsValidReference)
            {
                info = "[[Invalid File Reference]]\r\n" + info;
            }
            return info;
        }

        protected virtual string GetGenericInfo()
        {
            return
                "File Information\r\n---\r\n" +
                "Name: " + mName + "." + mFileExtension + "\r\n" +
                "Offset: " + mOffset + "\r\n" +
                "Length: " + mLength + "\r\n" +
                "Type Name: " + mType.Name + "\r\n" +
                "Type Description: " + mType.Description + "\r\n"
                ;
        }

        protected virtual string GetRawIndexInfo()
        {
            string info = "---\r\nRaw Data In Index:\r\n";
            foreach (uint rawData in mRawIndexData)
            {
                info += string.Format("Hex: {0:X8} Dec: {0:000000000000}\r\n", rawData);
            }
            return info;
        }

        protected virtual string GetRawHeaderInfo()
        {
            if (mRawHeaderData == null)
            {
                return "";
            }
            string info = "---\r\nRaw Header Data:\r\n";
            info += "Hex:   ";
            foreach (byte currentByte in mRawHeaderData)
            {
                info += string.Format("{0:X2}   ", currentByte);
            }
            info += "\r\n";
            info += "Dec:   ";
            foreach (byte currentByte in mRawHeaderData)
            {
                info += string.Format("{0:000}  ", currentByte);
            }
            info += "\r\n";
            info += "ASCII: ";
            byte[] printable = sanitizeByteArray(mRawHeaderData, (byte)95);
            for (int i = 0; i <= printable.GetUpperBound(0); i++)
            {
                info += new string(Encoding.ASCII.GetChars(printable, i, 1)) + "    ";
            }
            info += "\r\n";
            return info;
        }

        //exports this file
        public virtual void Export(string path)
        {
            FileStream iStream;
            FileStream oStream;
            byte[] byteBuffer = new byte[mLength];

            iStream = new FileStream(mParentBigFile.Path, FileMode.Open, FileAccess.Read);
            oStream = new FileStream(path, FileMode.Create, FileAccess.Write);

            iStream.Seek(mOffset, SeekOrigin.Begin);
            iStream.Read(byteBuffer, 0, mLength);
            oStream.Write(byteBuffer, 0, mLength);

            oStream.Close();
            iStream.Close();
        }

        //replaces the file with one of equal size
        public virtual void Replace(string path)
        {
            long FileSize;
            try
            {
                FileInfo fInfo = new FileInfo(path);
                FileSize = fInfo.Length;
            }
            catch (Exception ex)
            {
                throw new BigFileOpenException
                    ("Unable to open the file " + path + " to determine its size.\r\n" +
                    "The specific error message is: \r\n" +
                    ex.Message
                    );
            }

            if (FileSize != mLength)
            {
                throw new FileSizeMismatchException
                    (
                    "The file " + path + " is not of the same size as the file " + mName + 
                    " that a replace was attempted on."
                    );
            }

            FileStream iStream;
            FileStream oStream;
            byte[] byteBuffer = new byte[mLength];

            oStream = new FileStream(mParentBigFile.Path, FileMode.Open, FileAccess.Write);
            iStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            iStream.Seek(0, SeekOrigin.Begin);
            oStream.Seek(mOffset, SeekOrigin.Begin);
            iStream.Read(byteBuffer, 0, mLength);
            oStream.Write(byteBuffer, 0, mLength);

            oStream.Close();
            iStream.Close();
        }


        //for sorting
        public int CompareTo(object compObj)
        {
            if (!(compObj is BF.File))
            {
                throw new InvalidCastException("Can't compare BigFile File class with other classes.");
            }
            BF.File compFile = (BF.File)compObj;
            return this.Name.CompareTo(compFile.Name);
        }


        protected byte[] sanitizeByteArray(byte[] inArray, byte defaultChar)
        {
            byte[] outArray;
            outArray = new byte[inArray.GetUpperBound(0) + 1];

            //sanitize name
            for (int s = 0; s <= inArray.GetUpperBound(0); s++)
            {
                //non-printable ASCII chars
                if (inArray[s] < 32)
                {
                    outArray[s] = defaultChar;
                }
                else if (inArray[s] > 126)
                {
                    outArray[s] = defaultChar;
                }
                //still valid
                else
                {
                    outArray[s] = inArray[s];
                }
            }
            return outArray;
        }
    }
}
