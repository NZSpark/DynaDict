using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DynaDict
{
    class DictDataModel
    {
        public string sDictName = "embryo";
        public string sDictDescription = "Gene editing.";
        public List<string> sSourceLinks = new List<string> {
            "https://www.theguardian.com/science/2018/nov/28/scientist-in-china-defends-human-embryo-gene-editing",
            "https://www.theguardian.com/science/2018/nov/26/worlds-first-gene-edited-babies-created-in-china-claims-scientist"
        };
        public List<VocabularyDataModel> DictWordList = new List<VocabularyDataModel>() ;

        //look up word in dictionary, if word exists, return the defination of word, otherwise, return null.
        public VocabularyDataModel LookupWord(string sWord)
        {
            foreach(var v in DictWordList)
            {
                if (v.sVocabulary.Equals(sWord))
                    return v;
            }
            return null;
        }

        public void UpdateDictWord()
        {
            if(sSourceLinks.Count >0)
            {
                foreach(var s in sSourceLinks)
                {
                    //get all words in a url
                    List<string> tmpWordList = GetWordListFromString(LoadWebPage(s));
                    foreach (var v in tmpWordList)
                    {
                        //add word into dictionary if it does not exist.
                        if (LookupWord(v).Equals(null))
                        {

                        }
                    }
                }
            }
        }
        public string LoadWebPage(string sWebPageURL)
        {
            string sResult = null;
            WebResponse response = null;
            StreamReader reader = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sWebPageURL);
                request.Method = "GET";
                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                sResult = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                // handle error
                System.Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();
            }
            return sResult;
        }

        //Generate a list of unique words. 
        public List<string> GetWordListFromString(string sInput)
        {
            char [] sResult = sInput.ToArray();
           
            for (int i = 0; i < sResult.Length; i++)
            {
                //English word [A~Za~z];
                if ((sResult[i] > 'A' && sResult[i] < 'Z') || (sResult[i] > 'a' && sResult[i] < 'z'))
                    continue;
                sResult[i] = ' ';
            }

            string [] sWordList = new string(sResult).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            sWordList = sWordList.Distinct().ToArray();
            return new List<string>(sWordList);
        }



    }

    class VocabularyDataModel
    {
        public string sVocabulary = "embryo";
        public string sPhonics = "ˈembrēˌō";

        public bool bShowChineseDefinition = true;

        public List<string> sEnglishDefinition = new List<string> {
            "an unborn or unhatched offspring in the process of development." ,
            "the part of a seed that develops into a plant, consisting (in the mature embryo of a higher plant) of a plumule, a radicle, and one or two cotyledons."
        };
        public List<string> sChineseDefinition = new List<string> {"胎" };
        public List<string> sSentences = new List<string>  { "They are engaging in an embryo research." };

        public string GetDefinition(string sWord)
        {
            string sDefinition = "";

            return sDefinition;
        }

        //http://www.dict.cn/embryo
        public void ExtractDefinitionFromDicCN(string sInput)
        {
            List<string> sED = new List<string> { "" };
            sEnglishDefinition = sED;
            sChineseDefinition = sED;
            sSentences = sED;
        }

    }
}