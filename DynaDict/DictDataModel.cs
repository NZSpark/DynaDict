using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using System.Text;
using System.Xml;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HtmlAgilityPack;

namespace DynaDict
{
    class DictDataModel
    {
        public string sDictName = "embryo";
        public string sDictDescription = "Gene editing.";
        public List<string> sSourceLinks = new List<string>();
        public List<VocabularyDataModel> DictWordList = new List<VocabularyDataModel>() ;

        //look up word in dictionary, if word exists, return the defination of word, otherwise, return null.
        public VocabularyDataModel LookupWordLocal(string sWord)
        {
            VocabularyDataModel vdm = null;
            //lookup in dictionary.
            foreach (var v in DictWordList)
            {
                if (v.sVocabulary.Equals(sWord))
                    return v;
            }
            //lookup in whole database.
            DatabaseManager dm = new DatabaseManager();
            vdm = dm.GetWordDefinition(sWord);
            if (vdm is null)
                return null;
            DictWordList.Add(vdm);
            return vdm;
        }

        public VocabularyDataModel LookupWordOnline(string sWord)
        {
            VocabularyDataModel vdm = new VocabularyDataModel();
            if(vdm.ExtractDefinitionFromDicCN(sWord))
                return vdm;
            return null;
        }


        public void UpdateDictWord()
        {

            List<string> passList = new List<string>();
            DatabaseManager dm = new DatabaseManager();
            //meaningless words should be passed.
            passList.AddRange(dm.GetWordListByDictName("Trash"));
            //familiar words should be passed.
            passList.AddRange(dm.GetWordListByDictName("PassDict"));

            if (sSourceLinks.Count >0)
            {
                foreach(var s in sSourceLinks)
                {
                    //get all words in a url
                    List<string> tmpWordList = GetWordListFromString(LoadWebPage(s));
                    //List<string> tmpWordList = new List<string> { "hello", "world" }; //test www search
                    //List<string> tmpWordList = new List<string> { "join" };           //test mobile search
                    if (tmpWordList is null)
                        continue;

                    foreach (var v in tmpWordList)
                    {
                        if (passList.Contains(v))
                            continue;
                        //add word into dictionary if it does not exist.
                        if (LookupWordLocal(v) == null)
                        {
                            VocabularyDataModel vdm = LookupWordOnline(v);
                            if (vdm != null)
                                DictWordList.Add(vdm);
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
                request.Timeout = 10000;
                response = request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                sResult = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                // handle error
                System.Console.WriteLine(e.ToString());
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
            if (sInput == null)
                return null;
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
        public string sVocabulary ;
        public string sPhonics;

        public bool bShowChineseDefinition = true;

        public List<string> sEnglishDefinition = new List<string>();
        public List<string> sChineseDefinition = new List<string>();
        public List<string> sSentences = new List<string>();

        //http://www.dict.cn/embryo
        public bool ExtractDefinitionFromDicCNWWW(string sWord)
        {
            DictDataModel ddm = new DictDataModel();
            string sDicCNIn = ddm.LoadWebPage("http://www.dict.cn/" + sWord);
            //string sDicCNIn = ddm.LoadWebPage("http://m.dict.cn/msearch.php?q=" + sWord);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(sDicCNIn);

            sVocabulary = sWord;

            //extrict phonics
            List<HtmlNode> photicList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("phonetic"))).ToList();
            if (photicList.Count > 0)
            {
                if(photicList[0].Descendants("bdo").ToList().Count > 0)
                    sPhonics = photicList[0].Descendants("bdo").ToList()[0].InnerText;
            }

            //extrict Chinese definition
            List<HtmlNode> divCNList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("basic clearfix"))).ToList();
            if (divCNList.Count > 0)
            {
                if(divCNList[0].Descendants("li").ToList().Count > 0)
                sChineseDefinition.Add(divCNList[0].Descendants("li").ToList()[0].InnerText.Replace("\t", ""));
            }
            divCNList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("layout dual"))).ToList();
            if (divCNList.Count > 0)
            {
                var liCN = divCNList[0].Descendants("li").ToList();
                foreach (var li in liCN)
                {
                    sChineseDefinition.Add(li.InnerText.Replace("\t",""));
                }
            }

            //extrict English definition
            List<HtmlNode> divENList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("layout en"))).ToList();
            if (divENList.Count > 0)
            {
                var liEN = divENList[0].Descendants("li").ToList();
                foreach (var li in liEN)
                {
                    sEnglishDefinition.Add(li.InnerText.Replace("\t", ""));
                }
            }

            //extrict sentences
            List<HtmlNode> divSTList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("layout sort"))).ToList();
            if (divSTList.Count > 0)
            {
                var liST = divSTList[0].Descendants("li").ToList();
                foreach (var li in liST)
                {
                    sSentences.Add(li.InnerText);
                }
            }

            return true;
        }

        //http://m.dict.cn/msearch.php?q=embryo, Mobile
        public bool ExtractDefinitionFromDicCN(string sWord)
        {
            DictDataModel ddm = new DictDataModel();
            //string sDicCNIn = ddm.LoadWebPage("http://www.dict.cn/" + sWord);
            string sDicCNIn = ddm.LoadWebPage("http://m.dict.cn/msearch.php?q=" + sWord.ToLower());//only search lower case word.

            if (sDicCNIn is null)
                return false;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(sDicCNIn);

            sVocabulary = sWord;

            //extrict phonics
            List<HtmlNode> photicList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("phonetic"))).ToList();
            if (photicList.Count > 0)
            {
                if (photicList[0].Descendants("bdo").ToList().Count > 0)
                    sPhonics = photicList[0].Descendants("bdo").ToList()[0].InnerText;
            }

            //extrict Chinese definition
            List<HtmlNode> divCNList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("layout basic"))).ToList();
            if (divCNList.Count > 0)
            {
                if (divCNList[0].Descendants("li").ToList().Count > 0)
                    sChineseDefinition.Add(divCNList[0].Descendants("li").ToList()[0].InnerText.Replace("\t", ""));
            }
            else
            {
                DatabaseManager dm = new DatabaseManager();
                dm.SaveWordToDict("Trash", this);
                return false;
            }
            divCNList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("layout dual"))).ToList();
            if (divCNList.Count > 0)
            {
                var liCN = divCNList[0].Descendants("li").ToList();
                foreach (var li in liCN)
                {
                    sChineseDefinition.Add(li.InnerText.Replace("\t", ""));
                }
            }

            //extrict English definition
            List<HtmlNode> divENList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("layout en"))).ToList();
            if (divENList.Count > 0)
            {
                var liEN = divENList[0].Descendants("li").ToList();
                foreach (var li in liEN)
                {
                    sEnglishDefinition.Add(li.InnerText.Replace("\t", ""));
                }
            }

            //extrict sentences
            List<HtmlNode> divSTList = doc.DocumentNode.Descendants().Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("layout sort"))).ToList();
            if (divSTList.Count > 0)
            {
                var liST = divSTList[0].Descendants("li").ToList();
                foreach (var li in liST)
                {
                    sSentences.Add(li.InnerText);
                }
            }

            return true;
        }

    }
}