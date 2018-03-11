using Acr.UserDialogs;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
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
        private const string groupId = "sample_group";

        public CognitiveViewModelBase(IFaceServiceClient faceServiceClient)
        {
            _faceServiceClient = faceServiceClient;
        }
        protected async Task<string> ExecuteFindSimilarFaceCommandAsync(string personGroupId, MediaFile photo)
        {
            if (IsBusy)
                return string.Empty;

            IsBusy = true;

            try
            {
                using (var stream = photo.GetStream())
                {
                    // Upload our photo and see who it is!
                    var faces = await _faceServiceClient.DetectAsync(stream,
                       true,
                       true,
                       new FaceAttributeType[] {
                            FaceAttributeType.Gender,
                            FaceAttributeType.Age,
                            FaceAttributeType.Emotion
                       });
                    var faceIds = faces.Select(face => face.FaceId).ToArray();

                    var results = await _faceServiceClient.IdentifyAsync(faceIds, largePersonGroupId: groupId);
                    var result = results[0].Candidates[0].PersonId;

                    var person = await _faceServiceClient.GetPersonInLargePersonGroupAsync(groupId, result);

                    UserDialogs.Instance.ShowSuccess($"Person identified is {person.Name}.");

                    return person.Name;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                UserDialogs.Instance.ShowError(ex.Message);
                return string.Empty;
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
                    await _faceServiceClient.GetLargePersonGroupAsync(groupId);
                }
                catch
                {

                    await _faceServiceClient.CreateLargePersonGroupAsync(groupId, title);
                }

                // Step 2 - Add people to face list

                var p = await _faceServiceClient.CreatePersonInLargePersonGroupAsync(groupId, employee.Name);
                await _faceServiceClient.AddPersonFaceInLargePersonGroupAsync(groupId, p.PersonId, photo.GetStream());

                // Step 3 - Train face group
                await _faceServiceClient.TrainLargePersonGroupAsync(groupId);
                await _faceServiceClient.GetLargePersonGroupTrainingStatusAsync(groupId);
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

        public async Task<string> FaceDescription(MediaFile photo)
        {
            if (IsBusy)
                return string.Empty;

            IsBusy = true;

            using (var stream = photo.GetStream())
            {
                // Upload our photo and see who it is!
                var faces = await _faceServiceClient.DetectAsync(stream,
                   true,
                   true,
                   new FaceAttributeType[] {
                            FaceAttributeType.Gender,
                            FaceAttributeType.Age,
                            FaceAttributeType.Emotion,
                            FaceAttributeType.Hair
                   });
                var face = faces.First();
                StringBuilder sb = new StringBuilder();

                sb.Append("Face: ");

                // Add the gender, age, and smile.
                sb.Append(face.FaceAttributes.Gender);
                sb.Append(", ");
                sb.Append(face.FaceAttributes.Age);
                sb.Append(", ");
                sb.Append(String.Format("smile {0:F1}%, ", face.FaceAttributes.Smile * 100));

                // Add the emotions. Display all emotions over 10%.
                sb.Append("Emotion: ");
                EmotionScores emotionScores = face.FaceAttributes.Emotion;
                if (emotionScores.Anger >= 0.1f) sb.Append(String.Format("anger {0:F1}%, ", emotionScores.Anger * 100));
                if (emotionScores.Contempt >= 0.1f) sb.Append(String.Format("contempt {0:F1}%, ", emotionScores.Contempt * 100));
                if (emotionScores.Disgust >= 0.1f) sb.Append(String.Format("disgust {0:F1}%, ", emotionScores.Disgust * 100));
                if (emotionScores.Fear >= 0.1f) sb.Append(String.Format("fear {0:F1}%, ", emotionScores.Fear * 100));
                if (emotionScores.Happiness >= 0.1f) sb.Append(String.Format("happiness {0:F1}%, ", emotionScores.Happiness * 100));
                if (emotionScores.Neutral >= 0.1f) sb.Append(String.Format("neutral {0:F1}%, ", emotionScores.Neutral * 100));
                if (emotionScores.Sadness >= 0.1f) sb.Append(String.Format("sadness {0:F1}%, ", emotionScores.Sadness * 100));
                if (emotionScores.Surprise >= 0.1f) sb.Append(String.Format("surprise {0:F1}%, ", emotionScores.Surprise * 100));

                // Add glasses.
                sb.Append(face.FaceAttributes.Glasses);
                sb.Append(", ");

                // Add hair.
                sb.Append("Hair: ");

                // Display baldness confidence if over 1%.
                if (face.FaceAttributes.Hair.Bald >= 0.01f)
                    sb.Append(String.Format("bald {0:F1}% ", face.FaceAttributes.Hair.Bald * 100));

                // Display all hair color attributes over 10%.
                HairColor[] hairColors = face.FaceAttributes.Hair.HairColor;
                foreach (HairColor hairColor in hairColors)
                {
                    if (hairColor.Confidence >= 0.1f)
                    {
                        sb.Append(hairColor.Color.ToString());
                        sb.Append(String.Format(" {0:F1}% ", hairColor.Confidence * 100));
                    }
                }

                IsBusy = false;
                // Return the built string.
                return sb.ToString();
            }
        }
    }
}
