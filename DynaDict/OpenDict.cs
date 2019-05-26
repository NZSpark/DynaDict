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
        private float _viewX;
        private int _CurrentWordID = 0;

        Button btDelete;
        Button btPass;

        TextView tvWordName ;
        TextView tvPhonics;
        TextView tvChineseDefinition;
        TextView tvEnglishDefinition;
        TextView tvSentences;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        private void ResetControlText(VocabularyDataModel vdm)
        {
            if (vdm is null)
                return;
            /*
            tvWordName = view.FindViewById<TextView>(Resource.Id.tvWordName);
            tvPhonics = view.FindViewById<TextView>(Resource.Id.tvPhonics);
            tvChineseDefinition = view.FindViewById<TextView>(Resource.Id.tvChineseDefinition);
            tvEnglishDefinition = view.FindViewById<TextView>(Resource.Id.tvEnglishDefinition);
            tvSentences = view.FindViewById<TextView>(Resource.Id.tvSentences);
            */

            tvWordName.Text = vdm.sVocabulary;
            tvPhonics.Text = vdm.sPhonics;
            tvChineseDefinition.Text = string.Join(Environment.NewLine, vdm.sChineseDefinition.ToArray());
            tvEnglishDefinition.Text = string.Join(Environment.NewLine, vdm.sEnglishDefinition.ToArray());
            tvSentences.Text = string.Join(Environment.NewLine, vdm.sSentences.ToArray());

            return;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            var view = inflater.Inflate(Resource.Layout.fragment_opendicts, container, false);

             btDelete = view.FindViewById<Button>(Resource.Id.btOpenDictDelete);
             btPass = view.FindViewById<Button>(Resource.Id.btOpenDictPass);

             tvWordName = view.FindViewById<TextView>(Resource.Id.tvWordName);
             tvPhonics = view.FindViewById<TextView>(Resource.Id.tvPhonics);
             tvChineseDefinition = view.FindViewById<TextView>(Resource.Id.tvChineseDefinition);
             tvEnglishDefinition = view.FindViewById<TextView>(Resource.Id.tvEnglishDefinition);
             tvSentences = view.FindViewById<TextView>(Resource.Id.tvSentences);

            TextView tvOpenDictDictName = view.FindViewById<TextView>(Resource.Id.tvOpenDictDictName);
            DatabaseManager dm = new DatabaseManager();
            string sDictName = dm.GetDefaultValueByKey("DictName");
            if (sDictName.Equals("")) return view;
            tvOpenDictDictName.Text = "Dictionary: " + sDictName + ", " + dm.GetTotalWordsNumberByDictName(sDictName) + " words.";
            DictDataModel ddm = dm.GetDictFromDBByName(sDictName);
            if (ddm is null)
            {
                tvWordName.Text = "There is no dictionary. Please create one first!";
                return view;
            }


            btDelete.Click += delegate
            {
                dm.SaveWordToDict("Trash", ddm.DictWordList[_CurrentWordID]);
                dm.RemoveWordFromDict(ddm.sDictName, ddm.DictWordList[_CurrentWordID].sVocabulary);
                ddm.DictWordList.Remove(ddm.DictWordList[_CurrentWordID]);
                if (_CurrentWordID == ddm.DictWordList.Count)
                    _CurrentWordID = 0;
                ResetControlText(ddm.DictWordList[_CurrentWordID]);
            };

            btPass.Click += delegate
            {
                if (ddm.sDictName.Equals("PassDict")) return; //To move to itself is forbidden.
                dm.SaveWordToDict("PassDict", ddm.DictWordList[_CurrentWordID]);
                dm.RemoveWordFromDict(ddm.sDictName, ddm.DictWordList[_CurrentWordID].sVocabulary);
                ddm.DictWordList.Remove(ddm.DictWordList[_CurrentWordID]);
                if (_CurrentWordID == ddm.DictWordList.Count)
                    _CurrentWordID = 0;
                ResetControlText(ddm.DictWordList[_CurrentWordID]);
            };

            view.Touch += (s, e) =>
            {
                var handled = false;

                switch (e.Event.Action)
                {
                    case MotionEventActions.Down:
                        _viewX = e.Event.GetX();
                        handled = true;
                        break;
                    case MotionEventActions.Up:
                        var left = (int)(_viewX - e.Event.GetX() );
                        if (left > 250)
                        {
                            _CurrentWordID++;
                            if (_CurrentWordID == ddm.DictWordList.Count)
                                _CurrentWordID = 0;

                        }
                        if (left < -250)
                        {
                            _CurrentWordID--;
                            if (_CurrentWordID < 0)
                                _CurrentWordID = ddm.DictWordList.Count - 1;
                        }


                        /*
                        tvWordName.Text = ddm.DictWordList[_CurrentWordID].sVocabulary;
                        tvPhonics.Text = ddm.DictWordList[_CurrentWordID].sPhonics;
                        tvChineseDefinition.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sChineseDefinition.ToArray());
                        tvEnglishDefinition.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sEnglishDefinition.ToArray());
                        tvSentences.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sSentences.ToArray());
                        */
                        ResetControlText(ddm.DictWordList[_CurrentWordID]);

                        handled = true;
                        break;
                }



                /*
                if (e.Event.Action == MotionEventActions.Down)
                {
                    // do stuff
                    handled = true;
                }
                else if (e.Event.Action == MotionEventActions.Up)
                {
                    // do other stuff
                    handled = true;
                }
                */

                e.Handled = handled;
            };


            if (ddm.DictWordList.Count > 0)
            {
                _CurrentWordID = 0;


                ResetControlText(ddm.DictWordList[_CurrentWordID]);
                /*
                tvWordName.Text = ddm.DictWordList[_CurrentWordID].sVocabulary;
                tvPhonics.Text = ddm.DictWordList[_CurrentWordID].sPhonics;
                tvChineseDefinition.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sChineseDefinition.ToArray());
                tvEnglishDefinition.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sEnglishDefinition.ToArray());
                tvSentences.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sSentences.ToArray());
                */
                               
            }

            return view; 

            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}