using System.Collections.ObjectModel;
using Xamarin.Forms;
using EmployeeDirectory.ViewModels;
using Microsoft.ProjectOxford.Face;
using System.Diagnostics;
using System.Windows.Input;
using System;
using Plugin.Media.Abstractions;

namespace EmployeeDirectory
{
	public class VolonteerViewModel : CognitiveViewModelBase
	{
		private const string personGroupId = "Msp volonteer";
        public string OutputString { get; private set; }

        public ImageSource Photo => ImageSource.FromStream(() => _photo?.GetStream());
        private MediaFile _photo;

        public VolonteerViewModel(IFaceServiceClient faceServiceClient) : base(faceServiceClient)
		{
			Title = "Persons";
            AnalizeFaceCommand = new Command(AnalizeFace);
            FindSimilarFaceCommand = new Command(FindSimilarFace);
            OutputString = string.Empty;
        }

        private async void AnalizeFace(object obj)
        {
            _photo = await Snapshot();
            OnPropertyChanged(nameof(Photo));
            OutputString = await FaceDescription(_photo);
            OnPropertyChanged(nameof(OutputString));
        }

		public ICommand FindSimilarFaceCommand { get; private set; }
		

        private async void FindSimilarFace(object obj)
        {
            _photo = await Snapshot();
            OnPropertyChanged(nameof(Photo));
            OutputString = await ExecuteFindSimilarFaceCommandAsync(personGroupId, _photo);
            OnPropertyChanged(nameof(OutputString));
        }

        public ICommand AnalizeFaceCommand { get; private set; }
    }
}