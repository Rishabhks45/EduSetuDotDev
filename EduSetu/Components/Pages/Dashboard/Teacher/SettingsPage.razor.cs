using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Helpers;
using EduSetu.Application.Features.Profile;
using EduSetu.Application.Features.Profile.Requests;
using EduSetu.Application.Features.TeacherRegister;
using EduSetu.Application.Features.TeacherRegister.Requests;
using EduSetu.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace EduSetu.Components.Pages.Dashboard.Teacher
{
    public partial class SettingsPage : BaseComponent
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IMediator Mediator { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;
        [Inject] private INotificationService NotificationService { get; set; } = default!;
        [Inject] private IFileUploadService FileUploadService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        private Session session = new Session();

        //Form Data
        private CoachingDetailsDto InstituteformData = new();
        private UpdateProfileDto UpdateProfileDto = new();

        private bool showCurrentPassword = false;
        private bool showNewPassword = false;
        private bool showConfirmPassword = false;

        // For Check Password validation
        bool isVisibleCheckBox = false;
        bool hasMinLength;
        bool hasUppercase;
        bool hasNumber;
        bool hasSpecialChar;
        bool isPasswordValid => hasMinLength && hasUppercase && hasNumber && hasSpecialChar;

        private bool isEditing = false;
        private string? ImageUrlToView;
        private bool showViewProfileImageModal = false;
        private bool isChangingPassword = false;
        private bool isCropping = false;
        private bool showCropModal = false;
        private bool isSaving = false;
        private IBrowserFile? selectedFile;
        private string cropImageError = string.Empty;
        private string? originalProfilePictureUrl;
        private string? DELETE = "DELETE";


        private string activeTab = "personal";
        private bool unsavedChanges = false;
        private bool showPasswordModal = false;
        private bool showDeleteModal = false;
        private string newSubject = "";
        private string newQualification = "";

        private UserSettings settings = new();
        private UserSettings originalSettings = new();

        private readonly List<SettingsTabItem> tabs = new()
    {
        new() { Id = "personal", Label = "Personal Info", Icon = "<path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z'></path>" },
        new() { Id = "Institution", Label = "Institution Details", Icon = "<path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z'></path>" },
        new() { Id = "preferences", Label = "Preferences", Icon = "<path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M12 6V4m0 16v-2m8-8h2M4 12H2m15.364 6.364l1.414 1.414M4.222 4.222l1.414 1.414M19.778 4.222l-1.414 1.414M6.636 19.778l-1.414 1.414M12 12a4 4 0 11-8 0 4 4 0 018 0z'></path>" },
        new() { Id = "security", Label = "Security", Icon = "<path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z'></path>" },
        new() { Id = "account", Label = "Account", Icon = "<path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z'></path>" }
    };

        protected async override Task OnInitializedAsync()
        {
            await ShowLoaderAsync();
            await CheckAuthenticationState();
            await LoadUserProfileAsync();
            await LoadCoachingProfileAsync();
            LoadInitialSettings();
            settings = DeepClone(originalSettings);

            await HideLoaderAsync();

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
            return CommonHelper.GetUserInitials(UpdateProfileDto?.FirstName, UpdateProfileDto?.LastName);
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

            UpdateProfileDto = response.Payload ?? new();
            originalProfilePictureUrl = UpdateProfileDto.ProfilePictureUrl;


        }
        private async Task LoadCoachingProfileAsync()
        {
            var response = await Mediator.Send(new GetCoachingProfileRequest(session));
            if (response.HasError)
            {
                ShowErrorModal(response.Errors);
                await HideLoaderAsync();
                return;
            }

            InstituteformData = response.Payload ?? new();
        }

        private void ClearProfilePicture()
        {
            UpdateProfileDto.ProfilePictureUrl = null;
            UpdateProfileDto.ImageBytes = null;
            selectedFile = null;
            NotificationService.Success("Profile picture marked for removal. Click 'Save Changes' to confirm.");
        }
        // Get Enum Discription
        private string GetEnumDescription<T>(T value) where T : Enum
        {
            var field = typeof(T).GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute != null ? attribute.Name! : value.ToString();
        }   
        private bool IsProfileImageExists()
        {
            if (string.IsNullOrEmpty(UpdateProfileDto.ProfilePictureUrl))
            {
                return false;
            }

            return FileUploadService.FileExists(UpdateProfileDto.ProfilePictureUrl);
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
                UpdateProfileDto.ImageBytes = croppedImageBase64;
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
        private async Task HandleSubmitAsync()
        {
            try
            {
                isSaving = true;

                string? fileToDelete = null;

                if (CropedImage != null && CropedImage.Length > 0)
                {
                    if (!string.IsNullOrEmpty(originalProfilePictureUrl))
                    {
                        fileToDelete = originalProfilePictureUrl;
                    }

                    UpdateProfileDto.ProfilePictureUrl = await FileUploadService.HandleFileUploadInByteAsync(CropedImage);
                }
                else if (UpdateProfileDto.ProfilePictureUrl == null && !string.IsNullOrEmpty(originalProfilePictureUrl))
                {
                    fileToDelete = originalProfilePictureUrl;
                }

                if (string.IsNullOrEmpty(UpdateProfileDto.FirstName) || string.IsNullOrEmpty(UpdateProfileDto.LastName) || string.IsNullOrEmpty(UpdateProfileDto.Email))
                {
                    NotificationService.Error("Please fill in all required fields.");
                    return;
                }

                // If validation passes, continue with the update
                var response = await Mediator.Send(new UpdateUserProfileRequest(UpdateProfileDto, session));
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
                await JSRuntime.InvokeVoidAsync("setHeaderUserInfo", UpdateProfileDto.FullName, UpdateProfileDto.Email, UpdateProfileDto.ProfilePictureUrl);

                isEditing = false;
                selectedFile = null;
                originalProfilePictureUrl = null;
            }
            finally
            {
                isEditing = false;
                isSaving = false;
                StateHasChanged(); // Ensure UI updates
            }
        }

        // Coaching Details Steps
        private async Task HandleCoachingDetailsSubmitAsync()
        {
            isLoading = true;
            InstituteformData.TeacherId = session.UserId;
            var Response = await Mediator.Send(new UpsertCoachingDetailsRequest(InstituteformData));

            if (!Response.HasError)
            {
                NotificationService.Success("Coaching updated successfully!");
                await LoadCoachingProfileAsync();
                isEditing = false;
                isLoading = false;
            }
            else
            {
                // Handle registration failure (e.g., show error message)
                if (Response.Errors.Count > 0)
                {
                    NotificationService.Error($"Coaching updated failed: {Response.Errors[0].Message}");
                    isLoading = false;
                    isEditing = false;
                }
                else
                {
                    NotificationService.Error("Coaching updated failed: Unknown error");
                    isEditing = false;
                    isLoading = false;
                }
            }
            isLoading = false;
        }

        private void HideCropModal()
        {
            showCropModal = false;
            cropImageError = string.Empty;

            // Clean up cropper when modal is closed
            _ = JSRuntime.InvokeVoidAsync("destroyCropper");
        }



















        private void LoadInitialSettings()
        {
            // In a real app, this would come from a database or API
            originalSettings = new UserSettings
            {
                Personal = new PersonalInfo
                {
                    Name = "Dr. Sarah Johnson",
                    Email = "sarah.johnson@university.edu",
                    Phone = "+1 (555) 123-4567",
                    Bio = "Computer Science professor with 10+ years of experience in algorithms and data structures. Passionate about teaching and research in machine learning.",
                    Avatar = "https://images.pexels.com/photos/3769021/pexels-photo-3769021.jpeg?auto=compress&cs=tinysrgb&w=150&h=150&dpr=2",
                    Location = "New York, USA",
                    Timezone = "America/New_York",
                    Language = "English"
                },
                Professional = new ProfessionalInfo
                {
                    Institution = "Delhi University",
                    Department = "Computer Science",
                    Designation = "Associate Professor",
                    EmployeeId = "EMP001234",
                    Subjects = new List<string> { "Data Structures", "Algorithms", "Machine Learning", "Database Systems" },
                    Experience = "10 years",
                    Qualifications = new List<string> { "Ph.D. Computer Science", "M.Tech Computer Science" }
                },
                Preferences = new PreferencesInfo
                {
                    Notifications = new NotificationSettings { Email = true, Push = true, Sms = false, Reminders = true },
                    Privacy = new PrivacySettings { ProfileVisibility = "public", ShowEmail = false, ShowPhone = false, AllowMessages = true },
                },
                Security = new SecurityInfo { TwoFactorEnabled = true, LastPasswordChange = "2024-01-15", ActiveSessions = 3 }
            };
        }

        private void ToggleEditMode()
        {
            isEditing = !isEditing;
            if (!isEditing) // If 'Cancel' was clicked
            {
                settings = DeepClone(originalSettings); // Revert changes
                unsavedChanges = false;
            }
        }

        private void HandleSave()
        {
            // In a real app, this would make an API call to save `settings`
            originalSettings = DeepClone(settings); // Persist changes to the original state
            isEditing = false;
            unsavedChanges = false;
            // Optional: Show a success toast/notification
        }

        private void AddSubject()
        {
            if (!string.IsNullOrWhiteSpace(newSubject) && !settings.Professional.Subjects.Contains(newSubject))
            {
                settings.Professional.Subjects.Add(newSubject);
                newSubject = "";
                unsavedChanges = true;
            }
        }

        private void HandleSubjectKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter") { AddSubject(); }
        }

        private void RemoveSubject(string subject)
        {
            settings.Professional.Subjects.Remove(subject);
            unsavedChanges = true;
        }

        private void AddQualification()
        {
            if (!string.IsNullOrWhiteSpace(newQualification) && !settings.Professional.Qualifications.Contains(newQualification))
            {
                settings.Professional.Qualifications.Add(newQualification);
                newQualification = "";
                unsavedChanges = true;
            }
        }

        private void HandleQualificationKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter") { AddQualification(); }
        }

        private void RemoveQualification(string qualification)
        {
            settings.Professional.Qualifications.Remove(qualification);
            unsavedChanges = true;
        }

        private T DeepClone<T>(T obj)
        {
            var json = JsonSerializer.Serialize(obj);
            return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Unable to clone object.");
        }

        // --- Data Models ---
        public class SettingsTabItem
        {
            public string Id { get; set; } = "";
            public string Label { get; set; } = "";
            public string Icon { get; set; } = "";
        }

        public class UserSettings
        {
            public PersonalInfo Personal { get; set; } = new();
            public ProfessionalInfo Professional { get; set; } = new();
            public PreferencesInfo Preferences { get; set; } = new();
            public SecurityInfo Security { get; set; } = new();
        }

        public class PersonalInfo
        {
            public string Name { get; set; } = "";
            public string Email { get; set; } = "";
            public string Phone { get; set; } = "";
            public string Bio { get; set; } = "";
            public string Avatar { get; set; } = "";
            public string Location { get; set; } = "";
            public string Timezone { get; set; } = "";
            public string Language { get; set; } = "";
        }

        public class ProfessionalInfo
        {
            public string Institution { get; set; } = "";
            public string Department { get; set; } = "";
            public string Designation { get; set; } = "";
            public string EmployeeId { get; set; } = "";
            public List<string> Subjects { get; set; } = new();
            public string Experience { get; set; } = "";
            public List<string> Qualifications { get; set; } = new();
        }

        public class PreferencesInfo
        {
            public NotificationSettings Notifications { get; set; } = new();
            public PrivacySettings Privacy { get; set; } = new();
        }

        public class NotificationSettings
        {
            public bool Email { get; set; }
            public bool Push { get; set; }
            public bool Sms { get; set; }
            public bool Reminders { get; set; }
        }

        public class PrivacySettings
        {
            public string ProfileVisibility { get; set; } = "public";
            public bool ShowEmail { get; set; }
            public bool ShowPhone { get; set; }
            public bool AllowMessages { get; set; }
        }

        public class SecurityInfo
        {
            public bool TwoFactorEnabled { get; set; }
            public string LastPasswordChange { get; set; } = "";
            public int ActiveSessions { get; set; }
        }
    }
}