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

            EditText etDictName =(EditText)view.FindViewById(Resource.Id.etDictName);
            EditText etURL = view.FindViewById<EditText>(Resource.Id.etURL);
            EditText etURLList = view.FindViewById<EditText>(Resource.Id.etURLList);
            Button btAdd = view.FindViewById<Button>(Resource.Id.btAdd);
            Button btCreate = view.FindViewById<Button>(Resource.Id.btCreate);
            btAdd.Click += delegate
            {
                etURLList.Text += etURL.Text + "\r\n";
            };

            btCreate.Click += delegate
            {
                DictDataModel ddm = new DictDataModel();
                foreach (string s in etURLList.Text.Split("\r\n"))
                {
                    if (!s.Equals(""))
                    {
                        ddm.sSourceLinks.Add(s);
                    }
                }
                ddm.sDictName = etDictName.Text;

            };

            // Use this to return your custom view for this Fragment
            return view;

            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}