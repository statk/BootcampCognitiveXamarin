using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EmployeeDirectory.ViewModels
{
    public class NewVolonteerViewModel : CognitiveViewModelBase
    {
        private const string personGroupId = "Msp volonteer";

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public ImageSource Photo => ImageSource.FromStream(()=>_photo?.GetStream());
        private MediaFile _photo;

        public ICommand PhotoCommand { get; private set; }
        public ICommand NewVolonteerCommand { get; private set; }

        public event EventHandler PopToList;


        public NewVolonteerViewModel()
        {
            PhotoCommand = new Command(TakePhoto);
            NewVolonteerCommand = new Command(RegisterVolonteer);
            
        }

        private async void RegisterVolonteer(object obj)
        {
            var volonteer = new Person
            {
                FirstName = FirstName,
                LastName = LastName,
                Title = personGroupId
            };

            await RegisterEmployee(personGroupId, volonteer, _photo);
            PopToList?.Invoke(this, EventArgs.Empty);

        }

        private async void TakePhoto(object obj)
        {
            _photo = await Snapshot().ConfigureAwait(false);

            OnPropertyChanged(nameof(Photo));
        }
    }
}
