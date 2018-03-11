using CommonServiceLocator;
using EmployeeDirectory.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.ProjectOxford.Face;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace EmployeeDirectory
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

            InotialiseIoc();


			MainPage = new NavigationPage(new EmployeesPage());
		}

        private void InotialiseIoc()
        {
            var container = SimpleIoc.Default;

            container.Register<VolonteerViewModel>();
            container.Register<NewVolonteerViewModel>();

            container.Register<IFaceServiceClient>(() => new FaceServiceClient(AppStrings.AzureConnectionString, AppStrings.AzureConnectionEndpoint));


            ServiceLocator.SetLocatorProvider(()=> container);
        }

        protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}

