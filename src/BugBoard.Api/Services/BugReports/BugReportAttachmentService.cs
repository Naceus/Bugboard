using BugBoard.Api.Data;
using BugBoard.Api.Models.BugReports;
using Microsoft.EntityFrameworkCore;

namespace BugBoard.Api.Services.BugReports
{
    public class BugReportAttachmentService : IBugReportAttachmentService
    {
        private const int MaxFiles = 5;
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf",
            ".png",
            ".jpg",
            ".jpeg",
            ".webp"
        };
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "image/png",
            "image/jpeg",
            "image/webp"
        };

        private static readonly byte[] PdfSignature =
        {
            0x25, 0x50, 0x44, 0x46
        };

        private static readonly byte[] PngSignature =
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
        };

        private static readonly byte[] JpegSignature =
        {
            0xFF, 0xD8, 0xFF
        };

        private static readonly byte[] RiffSignature =
        {
            0x52, 0x49, 0x46, 0x46
        };

        private static readonly byte[] WebpSignature =
        {
            0x57, 0x45, 0x42, 0x50
        };

        private readonly BugBoardDbContext _context;
        private readonly IWebHostEnvironment _environment;


        public BugReportAttachmentService(BugBoardDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        /// <summary>
        /// Validates and stores uploaded files for a bug report, then persists their attachment metadata.
        /// </summary>
        /// <param name="bugReportId">The bug report the uploaded files belong to.</param>
        /// <param name="files">The uploaded files to validate and store.</param>
        /// <param name="uploadedByUserId">The id of the user who uploaded the files.</param>
        public async Task SaveAttachmentsAsync(int bugReportId, IReadOnlyCollection<IFormFile> files, string? uploadedByUserId)
        {
            var bugReport = await _context.BugReports
                .AnyAsync(b => b.Id == bugReportId);

            if (!bugReport)
            {
                throw new InvalidOperationException("Bug Report was not found.");
            }
            if (files.Count == 0)
            {
                return;
            }

            await ValidateAttachmentsAsync(files);

            var storageDirectory = GetStorageDirectory(bugReportId);
            Directory.CreateDirectory(storageDirectory);
            
            List<string> savedFilePaths = new List<string>();


            try
            {
                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var storedFileName = CreateStoredFileName(extension);

                    var filePath = Path.Combine(storageDirectory, storedFileName);

                    using var fileStream = File.Create(filePath);
                    await file.CopyToAsync(fileStream);

                    savedFilePaths.Add(filePath);
                    var attachment = CreateAttachment(file, bugReportId, storedFileName, uploadedByUserId);
                    _context.BugReportAttachments.Add(attachment);

                }

                await _context.SaveChangesAsync();
            }

            catch
            {
                DeleteSaveFiles(savedFilePaths);
                throw;
            }


        }


        private async Task ValidateAttachmentsAsync(IReadOnlyCollection<IFormFile> files)
        {
            if (files.Count > MaxFiles)
            {
                throw new InvalidOperationException("A maximum of 5 attachments is allowed.");
            }


            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    throw new InvalidOperationException($"The file '{file.FileName}' is empty.");
                }
                if (file.Length > MaxFileSizeInBytes)
                {
                    throw new InvalidOperationException($"The file '{file.FileName}' exceeds the maximum size of 5 MB.");
                }

                var extension = Path.GetExtension(file.FileName);

                if (!AllowedExtensions.Contains(extension))
                {
                    throw new InvalidOperationException($"The file '{file.FileName}' has an unsupported file extension.");
                }

                if (!AllowedContentTypes.Contains(file.ContentType))
                {
                    throw new InvalidOperationException($"The file '{file.FileName}' has an unsupported content type.");
                }

                if (!await IsValidFileSignature(file, extension))
                {
                    throw new InvalidOperationException($"{file.FileName} does not match the expected file format.");
                }
            }
        }

        /// <summary>
        /// Validates that the uploaded file header matches the expected signature for its extension.
        /// </summary>
        /// <param name="file">The uploaded file to inspect.</param>
        /// <param name="extension">The already validated file extension.</param>
        /// <returns>True when the file header matches the expected format; otherwise false.</returns>
        private async Task<bool> IsValidFileSignature(IFormFile file, string extension)
        {
            byte[] header = new byte[12];

            using var stream    = file.OpenReadStream();
            var bytesRead       = await stream.ReadAsync(header);

            switch (extension)
            {
                case ".pdf":
                    return HeaderStartsWith(header, bytesRead, PdfSignature);
                case ".png":
                    return HeaderStartsWith(header, bytesRead, PngSignature);
                case ".jpg":
                    return HeaderStartsWith(header, bytesRead, JpegSignature);
                case ".jpeg":
                    return HeaderStartsWith(header, bytesRead, JpegSignature);
                case ".webp":
                    return HeaderStartsWith(header, bytesRead, RiffSignature) && HeaderMatchesAt(header, bytesRead, WebpSignature, 8);
                default:
                    return false;
            }
            
        }


        private bool HeaderStartsWith(byte[] header, int bytesRead, byte[] signature)
        {
         
            if (bytesRead < signature.Length)
            {
                return false;
            }

            for (int i = 0; i < signature.Length; i++)
            {
                if (header[i] != signature[i])
                {
                    return false;
                }
                
            }

            return true;
        }

        /// <summary>
        /// Checks whether a file header contains a specific byte signature at the given offset.
        /// </summary>
        /// <param name="header">The bytes read from the beginning of the file.</param>
        /// <param name="bytesRead">The number of bytes actually read into the header buffer.</param>
        /// <param name="signature">The expected byte signature.</param>
        /// <param name="offset">The position in the header where the signature must start.</param>
        /// <returns>True when the signature matches at the offset; otherwise false.</returns>
        private bool HeaderMatchesAt(byte[] header, int bytesRead, byte[] signature, int offset)
        {
            if (bytesRead < signature.Length + offset)
            {
                return false;
            }
            for (int i = 0; i < signature.Length; i++)
            {
                if (header[offset + i] != signature[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Builds the protected storage directory path for attachments of a bug report.
        /// </summary>
        /// <param name="bugReportId">The bug report id used to isolate uploaded files per report.</param>
        /// <returns>The absolute directory path where attachments for the bug report are stored.</returns>
        private string GetStorageDirectory(int bugReportId)
        {
            return Path.Combine(
                _environment.ContentRootPath,
                "App_Data",
                "uploads",
                "bug-reports",
                bugReportId.ToString());
        }

        private string CreateStoredFileName(string extension)
        {
            return Guid.NewGuid().ToString("N") + extension;
        }

        private BugReportAttachment CreateAttachment(IFormFile file, int bugReportId, string storedFileName, string? uploadedByUserId)
        {
            BugReportAttachment attachment = new()
            {
                BugReportId = bugReportId,
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                UploadedByUserId = uploadedByUserId
            };

            return attachment;
        }

        /// <summary>
        /// Deletes files that were already written to disk during a failed attachment save operation.
        /// </summary>
        /// <param name="filePaths">The absolute paths of files that should be removed.</param>
        private void DeleteSaveFiles(IEnumerable<string> filePaths)
        {
            foreach (string path in filePaths)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
