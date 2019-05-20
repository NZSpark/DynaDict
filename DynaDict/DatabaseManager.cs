using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Environment = System.Environment;
using SQLitePCL;
using Newtonsoft.Json;

namespace DynaDict
{
    class DatabaseManager
    {
        //path must be absolute path with FileName.
        private static string DatabaseFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/DynaDict.Sqlite.db";
        //private const string DatabaseSource = "data source=" + DatabaseFileName + ";Version=3;";
        private static string DatabaseSource = "Filename=" + DatabaseFileName; //Filename = -> relative path; Data Source= -> absolute path.
        SqliteConnection m_dbConnection = new SqliteConnection(DatabaseSource);
        public void InitializeDB()
        {
            if(File.Exists(DatabaseFileName))
                File.Delete(DatabaseFileName);

            m_dbConnection.Open();

            string SQL_CREATE_DictList =
               "CREATE TABLE DictList (" +
               "DictID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
               "DictName TEXT, " + 
               "DictDescription TEXT)";
            SqliteCommand command = new SqliteCommand(SQL_CREATE_DictList, m_dbConnection);
            command.ExecuteNonQuery();

            command.CommandText =
                "CREATE TABLE URLList (" +
                "ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "DictID INTEGER," +
                "URL TEXT)";
            command.ExecuteNonQuery();

            /*
            command.CommandText =
                "CREATE TABLE Vocabulary (" +
                "WordID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "WordName TEXT," +
                "Phonics TEXT," +
                "ChineseDefinition TEXT," +
                "EnglishDefinition TEXT)";
            */
            command.CommandText =
                "CREATE TABLE Vocabulary (" +
                "WordID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "WordName TEXT," +
                "WordObject TEXT)"; //the class VocabularyDataModel object will be stored as a JSON string.
            command.ExecuteNonQuery();


            command.CommandText =
                "CREATE TABLE DictWord (" +
                "WordID INTEGER ," +
                "DictID INTEGER ," +
                " PRIMARY KEY(WordID,DictID))";
            command.ExecuteNonQuery();

            m_dbConnection.Close();

        }

        static public void CreateFile(string databaseFileName)
        {
            FileStream fs = File.Create(databaseFileName);
            fs.Close();
        }

        public VocabularyDataModel GetWordDefinition(string sWord)
        {
            m_dbConnection.Open();

            SqliteCommand command = new SqliteCommand("select WordObject from Vocabulary where WordName = '" + sWord + "'", m_dbConnection);
            var reader = command.ExecuteScalar(); //ExecuteScalar returns the first column of the first row;
            m_dbConnection.Close();

            if (reader == null)
                return null;

            VocabularyDataModel vdm = JsonConvert.DeserializeObject<VocabularyDataModel>(reader.ToString());

            return vdm;
        }

        public void StoreDictToDB(DictDataModel ddm)
        {
            string sDictID = "";

            m_dbConnection.Open();
      
            //insert DictList
            SqliteCommand command = new SqliteCommand("insert into DictList (DictName,DictDescription) values ('" + ddm.sDictName + "','" + ddm.sDictDescription +  "')", m_dbConnection);
            command.ExecuteNonQuery().ToString();

            command.CommandText = "select last_insert_rowid()";
            sDictID = ((Int64)command.ExecuteScalar()).ToString();


            //insert URLList
            for (int i = 0; i < ddm.sSourceLinks.Count; i++)
            {
                command.CommandText = "insert into URLList (DictID, URL) values ('" + sDictID + "','" + ddm.sSourceLinks[i] + "')";
                command.ExecuteNonQuery();
            }

            //insert Vocabulary and DictWord
            for (int i = 0; i < ddm.DictWordList.Count; i++)
            {

                string sWordID = "";

                /*
                string sE = "";
                string sC = "";

                for(int j = 0; j < ddm.DictWordList[i].sChineseDefinition.Count; j++)
                {
                    sC = sC + ddm.DictWordList[i].sChineseDefinition[j] + "\n";
                }

                for (int j = 0; j < ddm.DictWordList[i].sEnglishDefinition.Count; j++)
                {
                    sE = sE + ddm.DictWordList[i].sEnglishDefinition[j] + "\n";
                }
                

                command.CommandText = "insert into Vocabulary (WordName,Phonics,ChineseDefinition,EnglishDefinition) values ('" 
                    + ddm.DictWordList[i].sVocabulary + "','" 
                    + ddm.DictWordList[i].sPhonics.Replace("'", "''") + "','" 
                    + sC.Replace("'", "''") + "','" 
                    + sE.Replace("'", "''") + "')";
                */
                string sWordObject = JsonConvert.SerializeObject(ddm.DictWordList[i]);
                command.CommandText = "insert into Vocabulary (WordName,WordObject) values ('"
                    + ddm.DictWordList[i].sVocabulary + "','"
                    + sWordObject.Replace("'", "''") + "')";
                command.ExecuteNonQuery().ToString();

                command.CommandText = "select last_insert_rowid()";
                sWordID = ((Int64)command.ExecuteScalar()).ToString();

                command.CommandText = "insert into DictWord (WordID,DictID) values ('" + sWordID + "','" + sDictID + "')";
                command.ExecuteNonQuery();
            }

            m_dbConnection.Close();
        }

        public DictDataModel GetDictFromDBByName(string DictName)
        {
            DictDataModel ddm = new DictDataModel();
            
            string sDictID = "";
            m_dbConnection.Open();
                        
            SqliteCommand command = new SqliteCommand("select * from DictList where DictName = '" + DictName + "'" , m_dbConnection);

            /*
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    sDictID = reader["DictID"].ToString();
                    ddm.sDictDescription = reader["DictDescription"].ToString();
                    Console.WriteLine(sDictID + ":" + ddm.sDictDescription);
                }
            }
            */

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    sDictID = reader["DictID"].ToString();
                    ddm.sDictDescription = reader["DictDescription"].ToString();
                    Console.WriteLine(sDictID + ":" + ddm.sDictDescription);
                }
            }

            command.CommandText = "select URL from URLList where DictID = '" + sDictID + "'";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    ddm.sSourceLinks.Add(reader["URL"].ToString());
                    Console.WriteLine(reader["URL"].ToString());
                }
            }

            command.CommandText = "select * from Vocabulary where WordID in (select WordID from DictWord where DictID = '" + sDictID + "' )";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    /*
                    VocabularyDataModel vdm = new VocabularyDataModel();
                    vdm.sVocabulary = reader["WordName"].ToString();
                    vdm.sPhonics = reader["Phonics"].ToString();
                    vdm.sEnglishDefinition = new List<string> (reader["EnglishDefinition"].ToString().Split(new char [] { '\n' }));
                    vdm.sChineseDefinition = new List<string>(reader["ChineseDefinition"].ToString().Split(new char [] { '\n' }));
                    */
                    VocabularyDataModel vdm = JsonConvert.DeserializeObject<VocabularyDataModel>(reader["WordObject"].ToString());
                    ddm.DictWordList.Add(vdm);                    
                }
            }

            m_dbConnection.Close();
            return ddm;
        }

        public List<string> GetDictNameList()
        {
            List <string> dictNameList = new List<string>();

            m_dbConnection.Open();

            SqliteCommand command = new SqliteCommand("select DictName from DictList", m_dbConnection);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    dictNameList.Add(reader["DictName"].ToString());
                }
            }

            m_dbConnection.Close();
            return dictNameList;
        }
    }
}