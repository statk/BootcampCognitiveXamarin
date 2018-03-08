using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using EmployeeDirectory.ViewModels;
using Foundation;
using GalaSoft.MvvmLight.Ioc;
using UIKit;

namespace EmployeeDirectory.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

            var locator = SimpleIoc.Default;

            locator.Register<EmployeesViewModel>();
            locator.Register<NewEmployeeViewModel>();

            LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}

