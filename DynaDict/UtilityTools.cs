
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
    public class UtilityTools : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_tools, container, false);


            Button btInitializeDB = view.FindViewById<Button>(Resource.Id.btInitailizeDB);
            btInitializeDB.Click += delegate
            {
                DatabaseManager dbm = new DatabaseManager();
                dbm.InitializeDB();
            };

            // Use this to return your custom view for this Fragment
            return view; 
        }
    }
}
