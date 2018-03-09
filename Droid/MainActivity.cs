using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using GalaSoft.MvvmLight.Ioc;
using EmployeeDirectory.ViewModels;

namespace EmployeeDirectory.Droid
{
	[Activity(Label = "EmployeeDirectory.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

            var locator = SimpleIoc.Default;

            locator.Register<VolonteerViewModel>();
            locator.Register<NewVolonteerViewModel>();

            LoadApplication(new App());

			Acr.UserDialogs.UserDialogs.Init(this);
		}
	}
}

