using System.Net;
using System.Net.Mail;

namespace ClinicManager.Services;

public class LocalFileStorageService : IFileStorageService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".pdf"];
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveMedicalDocumentAsync(IFormFile file, int patientId)
    {
        if (file.Length == 0) throw new InvalidOperationException("Pusty plik.");
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext)) throw new InvalidOperationException("Dozwolone są tylko jpg, png i pdf.");

        var uploads = Path.Combine(_environment.WebRootPath, "uploads", "patients", patientId.ToString());
        Directory.CreateDirectory(uploads);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploads, fileName);
        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream);

        return $"/uploads/patients/{patientId}/{fileName}";
    }
}

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName, CancellationToken cancellationToken)
    {
        var section = _configuration.GetSection("Smtp");
        using var message = new MailMessage(section["From"] ?? "noreply@clinic.local", to, subject, body);
        message.Attachments.Add(new Attachment(new MemoryStream(attachment), attachmentName, "application/pdf"));

        using var client = new SmtpClient(section["Host"] ?? "localhost", section.GetValue<int>("Port"))
        {
            EnableSsl = section.GetValue<bool>("EnableSsl")
        };

        var user = section["User"];
        var password = section["Password"];
        if (!string.IsNullOrWhiteSpace(user))
        {
            client.Credentials = new NetworkCredential(user, password);
        }

        await client.SendMailAsync(message, cancellationToken);
        _logger.LogInformation("Sent report e-mail to {Email}", to);
    }
}
