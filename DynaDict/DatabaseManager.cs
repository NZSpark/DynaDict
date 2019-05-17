﻿using System;
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

namespace DynaDict
{
    class DatabaseManager
    {
        private const string DatabaseFileName = "./DynaDict.Sqlite.db";
        //private const string DatabaseSource = "data source=" + DatabaseFileName + ";Version=3;";
        private const string DatabaseSource = "Filename=" + DatabaseFileName; //Filename = -> relative path; Data Source= -> absolute path.
        SqliteConnection m_dbConnection = new SqliteConnection(DatabaseSource);
        public void InitializeDB()
        {
            CreateFile(DatabaseFileName);

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

            command.CommandText =
                "CREATE TABLE Vocabulary (" +
                "WordID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "WordName TEXT," +
                "Phonics TEXT," +
                "ChineseDefinition TEXT," +
                "EnglishDefinition TEXT)";
            command.ExecuteNonQuery();


            command.CommandText =
                "CREATE TABLE DictWord (" +
                "WordID INTEGER PRIMARY KEY," +
                "DictID INTEGER PRIMARY KEY)";
            command.ExecuteNonQuery();

            m_dbConnection.Close();

        }

        static public void CreateFile(string databaseFileName)
        {
            FileStream fs = File.Create(databaseFileName);
            fs.Close();
        }


        public void StoreDictToDB(DictDataModel ddm)
        {
            string sDictID = "";
            if (!File.Exists(DatabaseFileName))
                InitializeDB();

            m_dbConnection.Open();
      
            //insert DictList
            SqliteCommand command = new SqliteCommand("insert into DictList (DictName,DictDescription) values ('" + ddm.sDictName + "','" + ddm.sDictDescription +  "')", m_dbConnection);
            sDictID = command.ExecuteNonQuery().ToString();

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

                command.CommandText = "insert into Vocabulary (WordName,Phonics,ChineseDefinition,EnglishDefinition) values ('" + ddm.DictWordList[i].sVocabulary + "','" + ddm.DictWordList[i].sPhonics + "','" + sC + "','" + sE + "')";
                sWordID = command.ExecuteNonQuery().ToString();

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

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
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
                    ddm.sSourceLinks.Append(reader["URL"].ToString());
                    Console.WriteLine(reader["URL"].ToString());
                }
            }

            command.CommandText = "select * from Vocabulary where WordID in (select WordID from DictWord where DictID = '" + sDictID + "' )";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    VocabularyDataModel vdm = new VocabularyDataModel();
                    vdm.sVocabulary = reader["WordName"].ToString();
                    vdm.sPhonics = reader["Phonics"].ToString();
                    vdm.sEnglishDefinition = new List<string> (reader["EnglishDefinition"].ToString().Split(new char [] { '\n' }));
                    vdm.sChineseDefinition = new List<string>(reader["ChineseDefinition"].ToString().Split(new char [] { '\n' }));
                    ddm.DictWordList.Append(vdm);                    
                }
            }

            m_dbConnection.Close();
            return ddm;
        }

        public string [] GetDictNameList()
        {
            string[] dictNameList = { };

            m_dbConnection.Open();

            SqliteCommand command = new SqliteCommand("select DictName from DictList", m_dbConnection);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    dictNameList.Append(reader["DictName"].ToString());
                }
            }

            m_dbConnection.Close();
            return dictNameList;
        }
    }
}