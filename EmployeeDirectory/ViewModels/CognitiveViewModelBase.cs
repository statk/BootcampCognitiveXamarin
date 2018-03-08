using Acr.UserDialogs;
using Microsoft.ProjectOxford.Face;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectory.ViewModels
{
    public class CognitiveViewModelBase : BaseViewModel
    {
        private readonly IFaceServiceClient _faceServiceClient;

        public CognitiveViewModelBase()
        {
            _faceServiceClient = new FaceServiceClient(AppStrings.AzureConnectionString);
        }
        protected async Task ExecuteFindSimilarFaceCommandAsync(string personGroupId)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                // Take a photo
                MediaFile photo = await Snapshot();

                using (var stream = photo.GetStream())
                {
                    // Upload our photo and see who it is!
                    var faces = await _faceServiceClient.DetectAsync(stream);
                    var faceIds = faces.Select(face => face.FaceId).ToArray();

                    var results = await _faceServiceClient.IdentifyAsync(faceIds, personGroupId: personGroupId);
                    var result = results[0].Candidates[0].PersonId;

                    var person = await _faceServiceClient.GetPersonInPersonGroupAsync(personGroupId, result);

                    UserDialogs.Instance.ShowSuccess($"Person identified is {person.Name}.");
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private static async Task<MediaFile> Snapshot()
        {
            MediaFile photo;

            await CrossMedia.Current.Initialize();

            // Take photo
            if (CrossMedia.Current.IsCameraAvailable)
            {
                photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "Persons Directory",
                    Name = "photo.jpg"
                });
            }
            else
            {
                photo = await CrossMedia.Current.PickPhotoAsync();
            }

            return photo;
        }

        private async Task RegisterEmployee(string title, Person employee)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                MediaFile photo = await Snapshot();

                // Step 1 - Create Face List
                var personGroupId = Guid.NewGuid().ToString();
                await _faceServiceClient.CreatePersonGroupAsync(personGroupId, title);

                // Step 2 - Add people to face list

                var p = await _faceServiceClient.CreatePersonAsync(personGroupId, employee.Name);
                await _faceServiceClient.AddPersonFaceAsync(personGroupId, p.PersonId, employee.PhotoUrl);

                // Step 3 - Train face group
                await _faceServiceClient.TrainPersonGroupAsync(personGroupId);
            }

            catch (Exception ex)
            {
                UserDialogs.Instance.ShowError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
