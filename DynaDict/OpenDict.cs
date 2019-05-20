using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Environment = System.Environment;

namespace DynaDict
{
    public class OpenDict : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            var view = inflater.Inflate(Resource.Layout.fragment_opendicts, container, false);

            TextView tvWordName = view.FindViewById<TextView>(Resource.Id.tvWordName);
            TextView tvPhonics = view.FindViewById<TextView>(Resource.Id.tvPhonics);
            TextView tvChineseDefinition = view.FindViewById<TextView>(Resource.Id.tvChineseDefinition);
            TextView tvEnglishDefinition = view.FindViewById<TextView>(Resource.Id.tvEnglishDefinition);
            TextView tvSentences = view.FindViewById<TextView>(Resource.Id.tvSentences);

            DatabaseManager dm = new DatabaseManager();
            DictDataModel ddm = dm.GetDictFromDBByName("SecurityMDS");

            if (ddm.DictWordList.Count > 0)
            {
                tvWordName.Text = ddm.DictWordList[0].sVocabulary;
                tvPhonics.Text = ddm.DictWordList[0].sPhonics;
                tvChineseDefinition.Text = string.Join(Environment.NewLine,ddm.DictWordList[0].sChineseDefinition.ToArray());
                tvEnglishDefinition.Text = string.Join(Environment.NewLine, ddm.DictWordList[0].sEnglishDefinition.ToArray());
                tvSentences.Text = string.Join(Environment.NewLine, ddm.DictWordList[0].sSentences.ToArray());
            }

            return view; 

            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}