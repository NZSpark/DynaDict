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

namespace DynaDict
{
    public class CreateDict : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_createdict, container, false); 

            EditText etDictName = view.FindViewById<EditText>(Resource.Id.etDictName);
            EditText etURL = view.FindViewById<EditText>(Resource.Id.etURL);
            EditText etURLList = view.FindViewById<EditText>(Resource.Id.etURLList);
            TextView tvResult = view.FindViewById<TextView>(Resource.Id.tvResult);
            Button btAdd = view.FindViewById<Button>(Resource.Id.btAdd);
            Button btCreate = view.FindViewById<Button>(Resource.Id.btCreate);
            btAdd.Click += delegate
            {
                etURLList.Text += etURL.Text + "\r\n";
            };

            btCreate.Click += delegate
            {
                btCreate.Enabled = false;
                DatabaseManager dm = new DatabaseManager();
                dm.SetDefaultValue("DictName", etDictName.Text);
                DictDataModel ddm = dm.GetDictFromDBByName(etDictName.Text);
                if (ddm is null)
                {
                    ddm = new DictDataModel();
                    ddm.sDictName = etDictName.Text;
                }
                //only for test.
                //etURLList.Text = "http://localhost:8080";

                    foreach (string s in etURLList.Text.Split("\r\n"))
                {
                    if (!s.Equals(""))
                    {
                        if(!ddm.sSourceLinks.Contains(s))
                            ddm.sSourceLinks.Add(s);
                    }
                }

                ddm.UpdateDictWord();
                dm.StoreDictToDB(ddm);
                tvResult.Text = "Dictionary: " + ddm.sDictName + " is created. Total " + ddm.DictWordList.Count + " words added.";
                Toast.MakeText(view.Context, "Dictionary is created!", ToastLength.Long).Show();
                btCreate.Enabled = true;
            };

            // Use this to return your custom view for this Fragment
            return view;

            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}