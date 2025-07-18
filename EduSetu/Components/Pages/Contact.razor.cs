using System.ComponentModel.DataAnnotations;

namespace EduSetu.Components.Pages
{
    public partial class Contact
    {
        private ContactForm contactForm = new();
        private List<string> expandedFaqs = new();

        private List<FAQ> faqs = new()
    {
        new FAQ { Id = "1", Question = "How do I access premium content?", Answer = "Premium content is available to registered users with a subscription. You can upgrade your account from your profile settings." },
        new FAQ { Id = "2", Question = "Can I download study materials?", Answer = "Yes, most study materials can be downloaded for offline use. Look for the download button on each resource." },
        new FAQ { Id = "3", Question = "How do I report incorrect content?", Answer = "You can report incorrect content by clicking the 'Report' button on any resource or by contacting our support team." },
        new FAQ { Id = "4", Question = "Is the content verified by experts?", Answer = "Yes, all content is reviewed and verified by subject matter experts before being published on our platform." },
        new FAQ { Id = "5", Question = "Can I contribute content?", Answer = "We welcome contributions from qualified educators. Please contact us with your credentials and proposed content." }
    };

        private void HandleSubmit()
        {
            // In a real application, this would send the form data to a server
            Console.WriteLine($"Form submitted: {contactForm.FirstName} {contactForm.LastName} - {contactForm.Email}");

            // Reset form
            contactForm = new ContactForm();

            // Show success message (you could add a toast notification here)
        }

        private void ToggleFaq(string faqId)
        {
            if (expandedFaqs.Contains(faqId))
            {
                expandedFaqs.Remove(faqId);
            }
            else
            {
                expandedFaqs.Add(faqId);
            }
        }

        public class ContactForm
        {
            [Required(ErrorMessage = "First name is required")]
            [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
            public string FirstName { get; set; } = "";

            [Required(ErrorMessage = "Last name is required")]
            [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
            public string LastName { get; set; } = "";

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Please select a subject")]
            public string Subject { get; set; } = "";

            [Required(ErrorMessage = "Message is required")]
            [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
            public string Message { get; set; } = "";
        }

        public class FAQ
        {
            public string Id { get; set; } = "";
            public string Question { get; set; } = "";
            public string Answer { get; set; } = "";
        }
    }
}