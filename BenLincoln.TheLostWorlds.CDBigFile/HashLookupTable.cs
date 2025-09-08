using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.OleDb;
using BF = BenLincoln.TheLostWorlds.CDBigFile;

namespace BenLincoln.TheLostWorlds.CDBigFile
{
    public class HashLookupTable
    {
        protected string mName;
        protected string mPath;
        protected string mTableName;
        protected Hashtable mHashTable;

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

        public string Path
        {
            get
            {
                return mPath;
            }
            set
            {
                mPath = value;
            }
        }

        public string TableName
        {
            get
            {
                return mTableName;
            }
            set
            {
                mTableName = value;
            }
        }

        public Hashtable HashTable
        {
            get
            {
                return mHashTable;
            }
            set
            {
                mHashTable = value;
            }
        }


        #endregion

        public HashLookupTable(string name, string path, string tableName)
        {
            Name = name;
            Path = path;
            TableName = tableName;
            //LoadHashTable();
        }

        public string LookupHash(uint inHash)
        {
            if (mHashTable.Contains(inHash))
            {
                return (string)mHashTable[inHash];
            }
            return null;
        }

        public void LoadHashTable()
        {
            if (!(System.IO.File.Exists(mPath)))
            {
                throw new FileNotFoundException("Could not find the database file " + mPath + ".");
            }

            mHashTable = new Hashtable();

            OleDbConnection hashConn;
            OleDbCommand hashCommand;
            OleDbDataReader hashReader;

            hashConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mPath);
            hashCommand = new OleDbCommand("select HashNum, Path from " + mTableName, hashConn);
            try
            {
                hashConn.Open();
                hashReader = hashCommand.ExecuteReader();
                while (hashReader.Read())
                {
                    Double currentVal = hashReader.GetDouble(0);
                    uint tHashNum = (uint)(currentVal);
                    string tPath = hashReader.GetString(1);
                    mHashTable.Add(tHashNum, tPath);
                }
                hashReader.Close();
                hashConn.Close();
            }
            catch (OleDbException ex)
            {
                mHashTable = new Hashtable();
                throw new HashTableLoadException("An OLEDB error occurred while reading the hash lookup table " + mTableName +
                    " from the database file " + mPath + ". The specific error was:\r\n" + ex.Message);
            }
            catch (IOException ex)
            {
                mHashTable = new Hashtable();
                throw new HashTableLoadException("An I/O error occurred while reading the hash lookup table " + mTableName +
                    " from the database file " + mPath + ". The specific error was:\r\n" + ex.Message);
            }

        }

    }
}
