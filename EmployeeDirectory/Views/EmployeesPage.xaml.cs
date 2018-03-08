using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace EmployeeDirectory
{
	public partial class EmployeesPage : ContentPage
	{
		public EmployeesPage()
		{
			InitializeComponent();

            var vm = SimpleIoc.Default.GetInstanceWithoutCaching<EmployeesViewModel>();
            BindingContext = vm;
		}
	}
}