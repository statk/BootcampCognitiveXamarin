using EmployeeDirectory.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EmployeeDirectory.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewEmployeePage : ContentPage
    {
        private readonly NewVolonteerViewModel _vm;
        public NewEmployeePage()
        {
            InitializeComponent();

            _vm = SimpleIoc.Default.GetInstanceWithoutCaching<NewVolonteerViewModel>();
            BindingContext = _vm;
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.PopToList += OnPopToListPage;

        }



        private async void OnPopToListPage(object sender, Person person)
        {
            await Navigation.PopModalAsync();
        }

    }
}