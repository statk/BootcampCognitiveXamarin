using Acr.UserDialogs;
using Microsoft.ProjectOxford.Face;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectory.ViewModels
{
    public class CognitiveViewModelBase : BaseViewModel
    {
        /// <summary>
        /// tuto: https://docs.microsoft.com/fr-fr/azure/cognitive-services/face/quickstarts/csharp
        /// </summary>
        private readonly IFaceServiceClient _faceServiceClient;
        private const string groupId = "cb7fd53c16eb47cd9e31dda435abbc4d";

        public CognitiveViewModelBase()
        {
            _faceServiceClient = new FaceServiceClient(AppStrings.AzureConnectionString, AppStrings.AzureConnectionEndpoint);
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

                    var results = await _faceServiceClient.IdentifyAsync(personGroupId, faceIds);
                    var result = results[0].Candidates[0].PersonId;

                    var person = await _faceServiceClient.GetPersonAsync(personGroupId, result);

                    UserDialogs.Instance.ShowSuccess($"Person identified is {person.Name}.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                UserDialogs.Instance.ShowError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<MediaFile> Snapshot()
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

        public async Task RegisterEmployee(string title, Person employee, MediaFile photo)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {

                // Step 1 - Create Face List
                try
                {
                    await _faceServiceClient.GetPersonGroupAsync(groupId);
                }
                catch (Exception)
                {

                    await _faceServiceClient.CreatePersonGroupAsync(groupId, title);
                }
                

                //if (group == null)
                   

                // Step 2 - Add people to face list

                var p = await _faceServiceClient.CreatePersonInPersonGroupAsync(groupId, employee.Name);
                await _faceServiceClient.AddPersonFaceInPersonGroupAsync(groupId, p.PersonId, photo.GetStream());

                // Step 3 - Train face group
                await _faceServiceClient.TrainPersonGroupAsync(groupId);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                UserDialogs.Instance.ShowError(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
