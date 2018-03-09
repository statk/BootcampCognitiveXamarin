using EmployeeDirectory.Views;
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

            var vm = SimpleIoc.Default.GetInstanceWithoutCaching<VolonteerViewModel>();
            BindingContext = vm;
		}

        private async void AddNewVolonteerClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NewEmployeePage());
        }
    }
}