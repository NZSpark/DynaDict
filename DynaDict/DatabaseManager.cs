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
        SqliteCommand command;

        public DatabaseManager()
        {
            if (!File.Exists(DatabaseFileName))
                InitializeDB();
            m_dbConnection.Open();
            command = new SqliteCommand("select DictName from DictList", m_dbConnection);
        }

        ~DatabaseManager()
        {
            m_dbConnection.Close();
        }

        public void InitializeDB()
        {
            //m_dbConnection always is open by constructor of DatabaseManager class.
            if (m_dbConnection.State == System.Data.ConnectionState.Open)
                m_dbConnection.Close();

            //all database data will be deleted.
            if (File.Exists(DatabaseFileName))
                File.Delete(DatabaseFileName);

            //when database is not exist, open action will create a new database.
            m_dbConnection.Open();

            string SQL_CREATE_DefaultValue =
               "CREATE TABLE DefaultValue (" +
               "Key TEXT NOT NULL PRIMARY KEY," +
               "Value TEXT)";
            SqliteCommand command = new SqliteCommand(SQL_CREATE_DefaultValue, m_dbConnection);
            command.ExecuteNonQuery();


            command.CommandText  =
               "CREATE TABLE DictList (" +
               "DictID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
               "DictName TEXT, " + 
               "DictDescription TEXT)";
            //SqliteCommand command = new SqliteCommand(SQL_CREATE_DictList, m_dbConnection);
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

        public string GetTotalWordsNumberByDictName(string sDictName)
        {
            command.CommandText = "select count(*) from DictWord where DictID in (select DictID from DictList where DictName = '" + sDictName + "')";
            var reader1 = command.ExecuteScalar();
            if (reader1 == null)
                return "0";
            return reader1.ToString();
        }
        public void RemoveDictByName(string sDictName)
        {
            command.CommandText = "select DictID from DictList where DictName = '" + sDictName + "'";
            var reader1 = command.ExecuteScalar();
            if (reader1 == null)
                return ;
            string sDictID = reader1.ToString();


            command.CommandText = "delete from DictWord where DictID = '" + sDictID + "'";
            command.ExecuteNonQuery();

            command.CommandText = "delete from URLList where DictID = '" + sDictID + "'";
            command.ExecuteNonQuery();

            command.CommandText = "delete from DictList where DictID = '" + sDictID + "'";
            command.ExecuteNonQuery();

        }

        public void RemoveWordFromDict(string sDictName, string sWord)
        {
            command.CommandText = "select DictID from DictList where DictName = '" + sDictName + "'";
            var reader1 = command.ExecuteScalar();
            if (reader1 == null)
                return;
            string sDictID = reader1.ToString();
            string sWordID = GetWordID(sWord);

            if (!sWordID.Equals(""))
            {
                command.CommandText = "delete from DictWord where DictID = '" + sDictID + "' and WordID ='" + sWordID + "'";
                command.ExecuteNonQuery();
            }

        }

        public List<string> GetWordListByDictName(string sDictName)
        {
            List<string> wordList = new List<string>();
            //SqliteCommand command = new SqliteCommand("select * from DictList where DictName = '" + DictName + "'" , m_dbConnection);
            command.CommandText = "select DictID from DictList where DictName = '" + sDictName + "'";
            var reader1 = command.ExecuteScalar();
            if (reader1 == null)
                return wordList;
            string sDictID = reader1.ToString();

 
            command.CommandText = "select WordName from Vocabulary where WordID in (select WordID from DictWord where DictID = '" + sDictID + "' )";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    wordList.Add(reader["WordName"].ToString());
                }
            }

            return wordList;
        }

        public bool SaveWordToDict(string sDictName, VocabularyDataModel vdm)
        {
            string sDictID = "";
            string sWordID = "";

            //m_dbConnection.Open();

            //insert DictList
            //SqliteCommand command = new SqliteCommand("insert into DictList (DictName,DictDescription) values ('" + ddm.sDictName + "','" + ddm.sDictDescription +  "')", m_dbConnection);
            command.CommandText = "select DictID from DictList where DictName = '" + sDictName + "'";
            var reader = command.ExecuteScalar();

            if (reader == null)
            {
                command.CommandText = "insert into DictList (DictName,DictDescription) values ('" + sDictName + "','Default')";
                command.ExecuteNonQuery().ToString();
                command.CommandText = "select last_insert_rowid()";
                sDictID = ((Int64)command.ExecuteScalar()).ToString();
            }
            else
                sDictID = reader.ToString();

            if (GetWordDefinition(vdm.sVocabulary) is null)
            {
                string sWordObject = JsonConvert.SerializeObject(vdm);
                command.CommandText = "insert into Vocabulary (WordName,WordObject) values ('"
                    + vdm.sVocabulary + "','"
                    + sWordObject.Replace("'", "''") + "')";
                command.ExecuteNonQuery().ToString();

                command.CommandText = "select last_insert_rowid()";
                sWordID = ((Int64)command.ExecuteScalar()).ToString();
            }
            else
            {
                sWordID = GetWordID(vdm.sVocabulary);
            }

            if (!sWordID.Equals(""))
            {
                command.CommandText = "insert into DictWord (WordID,DictID) values ('" + sWordID + "','" + sDictID + "')";
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        public VocabularyDataModel GetWordDefinition(string sWord)
        {
            //m_dbConnection.Open();

            //SqliteCommand command = new SqliteCommand("select WordObject from Vocabulary where WordName = '" + sWord + "'", m_dbConnection);
            command.CommandText = "select WordObject from Vocabulary where WordName = '" + sWord + "'";
            var reader = command.ExecuteScalar(); //ExecuteScalar returns the first column of the first row;
            //m_dbConnection.Close();

            if (reader == null)
                return null;

            VocabularyDataModel vdm = JsonConvert.DeserializeObject<VocabularyDataModel>(reader.ToString());

            return vdm;
        }

        public string GetWordID(string sWord)
        {
            //m_dbConnection.Open();

            //SqliteCommand command = new SqliteCommand("select WordObject from Vocabulary where WordName = '" + sWord + "'", m_dbConnection);
            command.CommandText = "select WordID from Vocabulary where WordName = '" + sWord + "'";
            var reader = command.ExecuteScalar(); //ExecuteScalar returns the first column of the first row;
            //m_dbConnection.Close();

            if (reader == null)
                return "";

            return reader.ToString();
        }

        public void StoreDictToDB(DictDataModel ddm)
        {
            string sDictID = "";

            //m_dbConnection.Open();

            //insert DictList
            //SqliteCommand command = new SqliteCommand("insert into DictList (DictName,DictDescription) values ('" + ddm.sDictName + "','" + ddm.sDictDescription +  "')", m_dbConnection);
            command.CommandText = "select DictID from DictList where DictName = '" + ddm.sDictName + "'";
            var reader = command.ExecuteScalar();

            if (reader == null)
            {
                command.CommandText = "insert into DictList (DictName,DictDescription) values ('" + ddm.sDictName + "','" + ddm.sDictDescription + "')";
                command.ExecuteNonQuery().ToString();
                command.CommandText = "select last_insert_rowid()";
                sDictID = ((Int64)command.ExecuteScalar()).ToString();
            }
            else
                sDictID = reader.ToString();


            //insert URLList
            for (int i = 0; i < ddm.sSourceLinks.Count; i++)
            {
                command.CommandText = "select DictID from URLList where URL = '" + ddm.sSourceLinks[i] + "'";
                reader = command.ExecuteScalar();

                if (reader != null && reader.ToString().Equals(sDictID))
                {
                    continue;
                }
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
                command.CommandText = "select WordID from Vocabulary where WordName = '" + ddm.DictWordList[i].sVocabulary + "'";
                reader = command.ExecuteScalar();

                if (reader != null)
                {
                    sWordID = reader.ToString();
                }
                else
                {
                    string sWordObject = JsonConvert.SerializeObject(ddm.DictWordList[i]);
                    command.CommandText = "insert into Vocabulary (WordName,WordObject) values ('"
                        + ddm.DictWordList[i].sVocabulary + "','"
                        + sWordObject.Replace("'", "''") + "')";
                    command.ExecuteNonQuery().ToString();

                    command.CommandText = "select last_insert_rowid()";
                    sWordID = ((Int64)command.ExecuteScalar()).ToString();
                }

                try
                {
                    command.CommandText = "insert into DictWord (WordID,DictID) values ('" + sWordID + "','" + sDictID + "')";
                    command.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    //duplicate record.
                    System.Console.WriteLine(e.ToString());
                }
            }

            //m_dbConnection.Close();
        }

        public DictDataModel GetDictFromDBByName(string DictName)
        {
            DictDataModel ddm = new DictDataModel();
            ddm.sDictName = DictName;
            string sDictID = "";
            //m_dbConnection.Open();

            //SqliteCommand command = new SqliteCommand("select * from DictList where DictName = '" + DictName + "'" , m_dbConnection);
            command.CommandText = "select DictID from DictList where DictName = '" + DictName + "'";
            var reader1 = command.ExecuteScalar();
            if (reader1 == null)
                return null;
            sDictID = reader1.ToString();
       
            command.CommandText = "select URL from URLList where DictID = '" + sDictID + "'";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if(!ddm.sSourceLinks.Contains(reader["URL"].ToString()))
                    ddm.sSourceLinks.Add(reader["URL"].ToString());
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

            //m_dbConnection.Close();
            return ddm;
        }

        public List<string> GetDictNameList()
        {
            List <string> dictNameList = new List<string>();

            //m_dbConnection.Open();

            //SqliteCommand command = new SqliteCommand("select DictName from DictList", m_dbConnection);
            command.CommandText = "select DictName from DictList";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    dictNameList.Add(reader["DictName"].ToString());
                }
            }

            //m_dbConnection.Close();
            return dictNameList;
        }

        public string GetDefaultValueByKey(string sKey)
        {
            try
            {
                command.CommandText = "select Value from DefaultValue where Key='" + sKey + "'";
                var reader = command.ExecuteScalar();
                if (reader == null)
                    return "";
                return reader.ToString();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return "";
            }
        }

        public void SetDefaultValue(string sKey,string sValue)
        {

            command.CommandText = "select Value from DefaultValue where Key='" + sKey + "'";
            var reader = command.ExecuteScalar();

            if (reader == null)
                command.CommandText = "insert into DefaultValue ( Key, Value) Values ('" + sKey + "','" + sValue + "')";
            else
                command.CommandText = "update DefaultValue set Value = '" + sValue + "' where Key='" + sKey + "'";

            command.ExecuteNonQuery();

        }
    }
}