using System.Collections.ObjectModel;
using Xamarin.Forms;
using EmployeeDirectory.ViewModels;
using Microsoft.ProjectOxford.Face;

namespace EmployeeDirectory
{
	public class VolonteerViewModel : CognitiveViewModelBase
	{
		private const string personGroupId = "Msp volonteer";

		public VolonteerViewModel()
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
			get { return findSimilarFaceCommand ?? (findSimilarFaceCommand = new Command(async () => await ExecuteFindSimilarFaceCommandAsync(personGroupId))); }
		}


    }
}