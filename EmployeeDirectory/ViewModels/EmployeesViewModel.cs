using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Acr.UserDialogs;
using Microsoft.ProjectOxford.Face;
using EmployeeDirectory.ViewModels;

namespace EmployeeDirectory
{
	public class EmployeesViewModel : CognitiveViewModelBase
	{
		private string personGroupId;

		public EmployeesViewModel()
		{
			Title = "Persons";

			Employees = new ObservableCollection<Person>();
		}

		private ObservableCollection<Person> employees;
		public ObservableCollection<Person> Employees
		{
			get { return employees; }
			set { employees = value; OnPropertyChanged("Persons"); }
		}

		private Command findSimilarFaceCommand;
		public Command FindSimilarFaceCommand
		{
			get { return findSimilarFaceCommand ?? (findSimilarFaceCommand = new Command(async () => await ExecuteFindSimilarFaceCommandAsync())); }
		}

		
	}
}