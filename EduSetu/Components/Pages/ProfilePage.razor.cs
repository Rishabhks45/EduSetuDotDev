using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Helpers;
using EduSetu.Application.Features.Profile;
using EduSetu.Application.Features.Profile.Requests;
using EduSetu.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EduSetu.Components.Pages
{
    public partial class ProfilePage : BaseComponent
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IMediator Mediator { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;
        [Inject] private INotificationService NotificationService { get; set; } = default!;
        [Inject] private IFileUploadService FileUploadService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        private Session session = new Session();

        // form data
        private UpdateProfileDto record = new();

        private bool isEditing = false;
        private string? ImageUrlToView;
        private bool showViewProfileImageModal = false;
        private bool isCropping = false;
        private bool showCropModal = false;
        private bool isSaving = false;
        private IBrowserFile? selectedFile;
        private string cropImageError = string.Empty;
        private string? originalProfilePictureUrl;

        private UserProfile? user;
        private UserStats stats = new();
        private List<ActivityItem> recentActivities = new();
        private List<UploadItem> recentUploads = new();

        // State management for showing/hiding sections
        private string activeSection = "uploads"; // Default to uploads
        private bool showSettingsDropdown = false;

        // Profile editing state
        private bool isEditingProfile = false;  


        protected override async Task OnInitializedAsync()
        {
            await ShowLoaderAsync();

            await CheckAuthenticationState();
            await LoadUserProfileAsync();
            await HideLoaderAsync();

            // Simulate loading current user data (replace with real user fetching logic)
            user = new UserProfile
            {
                FirstName = "Jane", // Simulate current user's first name
                LastName = "Smith", // Simulate current user's last name
                Email = "jane.smith@example.com", // Simulate current user's email
                JoinDate = DateTime.Now.AddMonths(-8)
            };

            stats = new UserStats
            {
                NotesCount = 24,
                PYQsCount = 15,
                VideosCount = 8,
                DownloadsCount = 156,
                StudyHours = 127
            };

            recentActivities = new List<ActivityItem>
            {
                new() { Type = "upload", Description = "Uploaded Physics Notes for Class 12", TimeAgo = "2 hours ago" },
                new() { Type = "download", Description = "Downloaded Chemistry PYQs 2023", TimeAgo = "1 day ago" },
                new() { Type = "study", Description = "Completed 2 hours of study session", TimeAgo = "2 days ago" },
                new() { Type = "upload", Description = "Shared Mathematics Video Tutorial", TimeAgo = "3 days ago" }
            };

            recentUploads = new List<UploadItem>
            {
                new() { Title = "Physics Notes - Chapter 1", Type = "note", UploadDate = DateTime.Now.AddDays(-1), Downloads = 45, Rating = 4.8 },
                new() { Title = "Chemistry Lab Video", Type = "video", UploadDate = DateTime.Now.AddDays(-2), Downloads = 32, Rating = 4.6 },
                new() { Title = "Mathematics PYQs 2023", Type = "pyq", UploadDate = DateTime.Now.AddDays(-3), Downloads = 67, Rating = 4.9 },
                new() { Title = "Biology Diagrams", Type = "note", UploadDate = DateTime.Now.AddDays(-4), Downloads = 28, Rating = 4.7 }
            };
        }

        private async Task CheckAuthenticationState()
        {
            AuthenticationState authState = await AuthProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                var currentUserId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                session.UserId = Guid.TryParse(currentUserId, out Guid userId) ? userId : Guid.Empty;
            }
            else
            {
                NavigationManager.NavigateTo("/login", replace: true);
            }
        }

        private string GetUserInitials()
        {
            return CommonHelper.GetUserInitials(record?.FirstName, record?.LastName);
        }

        private async Task LoadUserProfileAsync()
        {
            var response = await Mediator.Send(new GetUserProfileRequest(session));
            if (response.HasError)
            {
                ShowErrorModal(response.Errors);
                await HideLoaderAsync();
                return;
            }

            record = response.Payload ?? new();
            originalProfilePictureUrl = record.ProfilePictureUrl;


        }
        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            try
            {
                isCropping = true;
                showCropModal = true;
                var base64Image = await JSRuntime.InvokeAsync<string>("compressAndReturnImageBase64", "profilePictureInput");
                isCropping = false;
                if (!string.IsNullOrEmpty(base64Image))
                {
                    await JSRuntime.InvokeVoidAsync("showCropper", base64Image); // Your cropper
                }
            }
            catch (Exception ex)
            {
                cropImageError = $"Client-side error: {ex.Message}";
            }
        }


        private byte[] CropedImage { get; set; }
        private async Task CropImageAsync()
        {
            try
            {
                isCropping = true;
                cropImageError = string.Empty;

                // Get cropped image from JavaScript cropper
                string? croppedImageBase64 = await JSRuntime.InvokeAsync<string>("getCroppedImageBase64", TimeSpan.FromSeconds(10));

                if (string.IsNullOrEmpty(croppedImageBase64))
                {
                    cropImageError = "Failed to get cropped image data";
                    return;
                }
                record.ImageBytes = croppedImageBase64;
                if (croppedImageBase64.Contains(","))
                    CropedImage = Convert.FromBase64String(croppedImageBase64.Split(',')[1]);
                HideCropModal();

                NotificationService.Success("Image cropped successfully!");
            }
            catch (TaskCanceledException)
            {
                cropImageError = "Image cropping operation timed out";
                NotificationService.Error(cropImageError);
            }
            catch (Exception ex)
            {
                cropImageError = $"Error cropping image: {ex.Message}";
                NotificationService.Error(cropImageError);
            }
            finally
            {
                isCropping = false;
            }
        }
        private void HideCropModal()
        {
            showCropModal = false;
            cropImageError = string.Empty;

            // Clean up cropper when modal is closed
            _ = JSRuntime.InvokeVoidAsync("destroyCropper");
        }

        private bool IsProfileImageExists()
        {
            if (string.IsNullOrEmpty(record.ProfilePictureUrl))
            {
                return false;
            }

            return FileUploadService.FileExists(record.ProfilePictureUrl);
        }

             

        private void HideViewProfileImageModal()
        {
            ImageUrlToView = null;
            showViewProfileImageModal = false;
        }






        private async Task HandleSubmitAsync()
        {
            isSaving = true;

            try
            {
                string? fileToDelete = null;

                if (CropedImage != null && CropedImage.Length > 0)
                {
                    if (!string.IsNullOrEmpty(originalProfilePictureUrl))
                    {
                        fileToDelete = originalProfilePictureUrl;
                    }

                    record.ProfilePictureUrl = await FileUploadService.HandleFileUploadInByteAsync(CropedImage);
                    //record.ImageBytes = null;
                }
                else if (record.ProfilePictureUrl == null && !string.IsNullOrEmpty(originalProfilePictureUrl))
                {
                    fileToDelete = originalProfilePictureUrl;
                }

                var response = await Mediator.Send(new UpdateUserProfileRequest(record, session));
                if (response.HasError)
                {
                    ShowErrorModal(response.Errors);
                    return;
                }

                if (!string.IsNullOrEmpty(fileToDelete))
                {
                    await FileUploadService.DeleteFileAsync(fileToDelete);
                }

                // Show success message
                NotificationService.Success("Profile updated successfully!");

                // Update header information via JavaScript

                isEditing = false;
                selectedFile = null;
                originalProfilePictureUrl = null;
            }
            finally
            {
                isSaving = false;
                isEditingProfile = false;
            }
        }



































        private void EditProfile()
        {
            isEditingProfile = true;            
        }
        

        private void CancelEditProfile()
        {
            isEditingProfile = false;           
        }
        
        private void UploadContent()
        {
            NavigationManager.NavigateTo("/profile/uploads");
        }

        private void SetActiveSection(string section)
        {
            activeSection = section;
            showSettingsDropdown = false; // Close dropdown when switching sections
        }

        private void ToggleSettingsDropdown()
        {
            showSettingsDropdown = !showSettingsDropdown;
        }

        private void ChangePassword()
        {
            SetActiveSection("change-password");
        }

        private void DeleteAccount()
        {
            SetActiveSection("delete-account");
        }

        private void CloseChangePassword()
        {
            SetActiveSection("none");
        }

        private void CloseDeleteAccount()
        {
            SetActiveSection("none");
        }

        public class UserProfile
        {
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Email { get; set; } = "";
            public DateTime? JoinDate { get; set; }
        }

        public class UserStats
        {
            public int NotesCount { get; set; }
            public int PYQsCount { get; set; }
            public int VideosCount { get; set; }
            public int DownloadsCount { get; set; }
            public int StudyHours { get; set; }
        }

        public class ActivityItem
        {
            public string Type { get; set; } = "";
            public string Description { get; set; } = "";
            public string TimeAgo { get; set; } = "";
        }

        public class UploadItem
        {
            public string Title { get; set; } = "";
            public string Type { get; set; } = "";
            public DateTime UploadDate { get; set; }
            public int Downloads { get; set; }
            public double Rating { get; set; }
        }
    }
}