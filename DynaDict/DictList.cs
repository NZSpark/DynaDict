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
    public class DictList : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_dictlist, container, false);
            //EditText etDictName = view.FindViewById<EditText>(Resource.Id.etDictListDictName);
            TextView tvHint = view.FindViewById<TextView>(Resource.Id.tvHint);

            Button btDelete = view.FindViewById<Button>(Resource.Id.btDelete);
            Button btOpenDict = view.FindViewById<Button>(Resource.Id.btOpenDict);

            RadioGroup rgDictList = view.FindViewById<RadioGroup>(Resource.Id.rgDictList);

            int iSelectedID = 0;

            DatabaseManager dm = new DatabaseManager();
            string sDictName = dm.GetDefaultValueByKey("DictName");

            btOpenDict.Click += delegate
            {
                Fragment fragment = new OpenDict();
                FragmentManager.BeginTransaction().Replace(Resource.Id.flContent, fragment).Commit();
            };
                //delete dictionary. name can be set manually
                btDelete.Click += delegate
            {
                for(int i = 0;i<rgDictList.ChildCount;i++)
                {
                    if (rgDictList.GetChildAt(i).Id == iSelectedID)
                    {
                        if (!sDictName.Equals(""))
                        {                            
                            dm.RemoveDictByName(sDictName);
                            dm.SetDefaultValue("DictName", "");
                        }
                        rgDictList.RemoveViewAt(i);
                        break;
                    }
                }
                btDelete.Enabled = false;
                dm.RemoveDictByName(sDictName);
                DictDataModel ddm = dm.GetDictFromDBByName(sDictName);

            };
            btDelete.Enabled = false;

            List<string> dictList = dm.GetDictNameList();

            for (int i = 0; i < dictList.Count; i++) {
                RadioButton rbNew = new RadioButton(view.Context);
                rbNew.Id= View.GenerateViewId();//start from 1.
                //set attributes "wrap_content" dynamically.(but it has been set default.
                //LinearLayout.LayoutParams parmsWrapContent = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                //rbNew.LayoutParameters = parmsWrapContent;

                rbNew.Text = dictList[i];
                if (rbNew.Text.Equals(sDictName)) 
                    iSelectedID = rbNew.Id;
                rgDictList.AddView(rbNew);
            }

            rgDictList.CheckedChange += (sender, e) => {

                btDelete.Enabled = true;
                iSelectedID = e.CheckedId;

                RadioButton rbNew = view.FindViewById<RadioButton>(e.CheckedId);
                if (rbNew is null)
                    return;
                sDictName = rbNew.Text;
                dm.SetDefaultValue("DictName", sDictName);
                tvHint.Text = "Dictionary: " + sDictName + " is selected! " + dm.GetTotalWordsNumberByDictName(sDictName) + " words.";

            };

            /*
            for (int i = 0; i < rgDictList.ChildCount; i++)
            {
                RadioButton rbNew = view.FindViewById<RadioButton>(rgDictList.GetChildAt(i).Id);
                if(rbNew.Text.Equals(sDictName))
                {
                    iSelectedID = rgDictList.GetChildAt(i).Id;
                    rgDictList.Check(iSelectedID);
                    break;
                }
            }
            */
            rgDictList.Check(iSelectedID);

            return view;

            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}