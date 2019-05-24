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
                        tvWordName.Text = ddm.DictWordList[_CurrentWordID].sVocabulary;
                        tvPhonics.Text = ddm.DictWordList[_CurrentWordID].sPhonics;
                        tvChineseDefinition.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sChineseDefinition.ToArray());
                        tvEnglishDefinition.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sEnglishDefinition.ToArray());
                        tvSentences.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sSentences.ToArray());

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
                tvWordName.Text = ddm.DictWordList[_CurrentWordID].sVocabulary;
                tvPhonics.Text = ddm.DictWordList[_CurrentWordID].sPhonics;
                tvChineseDefinition.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sChineseDefinition.ToArray());
                tvEnglishDefinition.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sEnglishDefinition.ToArray());
                tvSentences.Text = string.Join(Environment.NewLine, ddm.DictWordList[_CurrentWordID].sSentences.ToArray());
            }

            return view; 

            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}