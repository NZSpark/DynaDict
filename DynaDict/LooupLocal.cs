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
using Android.Views.InputMethods;
using Android.Widget;
using Environment = System.Environment;

namespace DynaDict
{
    public class LooupLocal : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var view = inflater.Inflate(Resource.Layout.fragment_lookuplocal, container, false);
            EditText etWordToLookup = view.FindViewById<EditText>(Resource.Id.etWordToLookup);
            TextView tvWordName = view.FindViewById<TextView>(Resource.Id.tvWordName);
            TextView tvPhonics = view.FindViewById<TextView>(Resource.Id.tvPhonics);
            TextView tvChineseDefinition = view.FindViewById<TextView>(Resource.Id.tvChineseDefinition);
            TextView tvEnglishDefinition = view.FindViewById<TextView>(Resource.Id.tvEnglishDefinition);
            TextView tvSentences = view.FindViewById<TextView>(Resource.Id.tvSentences);
            CheckBox cbOnline = view.FindViewById<CheckBox>(Resource.Id.cbOnline);

            Button btLookupLocal = view.FindViewById<Button>(Resource.Id.btLookupLocal);

            //trigger lookup action when hit on Enter key.
            etWordToLookup.KeyPress += (s, e) =>
            {
                if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
                {
                    e.Handled = true;
                    btLookupLocal.CallOnClick();
                };
            };


            btLookupLocal.Click += delegate
            {
                if (etWordToLookup.Text.Equals(""))
                    return;
                DatabaseManager dm = new DatabaseManager();
                VocabularyDataModel vdm = dm.GetWordDefinition(etWordToLookup.Text);
                if(vdm is null)
                    if(cbOnline.Checked)
                    {
                        DictDataModel ddm = new DictDataModel();
                        vdm = ddm.LookupWordOnline(etWordToLookup.Text);
                        if (vdm is null)
                            return;
                        dm.SaveWordToDict("NewWord", vdm);
                    }
                tvWordName.Text = vdm.sVocabulary;
                tvPhonics.Text = vdm.sPhonics;
                tvChineseDefinition.Text = string.Join(Environment.NewLine, vdm.sChineseDefinition.ToArray());
                tvEnglishDefinition.Text = string.Join(Environment.NewLine, vdm.sEnglishDefinition.ToArray());
                tvSentences.Text = string.Join(Environment.NewLine, vdm.sSentences.ToArray());
                //hide keyboard.
                InputMethodManager imm = (InputMethodManager)view.Context.GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.NotAlways);
            };
            return view;
        }
    }
}