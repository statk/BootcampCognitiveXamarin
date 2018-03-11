using EmployeeDirectory.Views;
using GalaSoft.MvvmLight.Ioc;
using System;

using Xamarin.Forms;

namespace EmployeeDirectory
{
	public partial class EmployeesPage : ContentPage
	{
        private readonly VolonteerViewModel _vm;

		public EmployeesPage()
		{
			InitializeComponent();

            _vm = SimpleIoc.Default.GetInstanceWithoutCaching<VolonteerViewModel>();
           
		}

        private async void AddNewVolonteerClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NewEmployeePage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_vm == null)
                return;

            BindingContext = _vm;


        }
    }
}