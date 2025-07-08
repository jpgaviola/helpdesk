using Microsoft.EntityFrameworkCore;
using HelpdeskBlazor.Data;
using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public class TicketService : ITicketService
    {
        private readonly HelpdeskDbContext _context;

        public TicketService(HelpdeskDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            try
            {
                return await _context.Tickets
                    .Where(t => !t.IsDeleted)
                    .OrderByDescending(t => t.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllTicketsAsync: {ex.Message}");
                return new List<Ticket>();
            }
        }

        public async Task<List<TicketReportItem>> GenerateTicketReportAsync(DateTime startDate, DateTime endDate, string? majorConcern = null)
        {
            var query = _context.Tickets
                .Include(t => t.AssignedToUser)
                .Where(t => !t.IsDeleted &&
                           t.CreatedDate >= startDate &&
                           t.CreatedDate <= endDate);

            if (!string.IsNullOrEmpty(majorConcern))
            {
                query = query.Where(t => t.Category == majorConcern);
            }

            var tickets = await query.OrderBy(t => t.CreatedDate).ToListAsync();

            return tickets.Select(t => new TicketReportItem
            {
                RequestNo = $"2025-{t.Id:D5}",
                AppRefNo = t.AppReferenceNo ?? "N/A",
                RequestedDate = t.CreatedDate,
                RequestedBy = t.RequesterName ?? "Unknown",
                EndorsedTo = t.AssignedToUser?.Name ?? "N/A",
                TicketNo = $"2025-{t.Id:D5}",
                RequestType = "LEGAL",
                StartDateTime = t.CreatedDate,
                EndDateTime = t.ModifiedDate,
                ResponseDays = t.ModifiedDate.HasValue ?
                    (int)(t.ModifiedDate.Value - t.CreatedDate).TotalDays : 0,
                Concern = t.Description ?? "No description",
                MajorConcern = t.Category ?? "General"
            }).ToList();
        }

        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .Include(t => t.Attachments)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CreatedByUser)
                .Include(t => t.Signatories) // Add this line
                .Where(t => t.Id == id && !t.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            ticket.CreatedDate = DateTime.Now;
            ticket.IsDeleted = false;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket> UpdateTicketAsync(Ticket ticket)
        {
            ticket.ModifiedDate = DateTime.Now;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await GetTicketByIdAsync(id);
            if (ticket == null) return false;

            ticket.IsDeleted = true;
            ticket.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Ticket>> GetTicketsByStatusAsync(string status)
        {
            return await _context.Tickets
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .Where(t => !t.IsDeleted && t.Status == status)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetTicketsByPriorityAsync(string priority)
        {
            return await _context.Tickets
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .Where(t => !t.IsDeleted && t.Priority == priority)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetTicketsByUserAsync(int userId)
        {
            return await _context.Tickets
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .Where(t => !t.IsDeleted && (t.AssignedToUserId == userId || t.CreatedBy == userId))
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Ticket>> SearchTicketsAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await GetAllTicketsAsync();

            return await _context.Tickets
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .Where(t => !t.IsDeleted &&
                    (t.Subject.Contains(searchTerm) ||
                     t.Description.Contains(searchTerm) ||
                     t.Department.Contains(searchTerm) ||
                     t.Category.Contains(searchTerm) ||
                     (t.Company != null && t.Company.Contains(searchTerm)) ||
                     (t.AppReferenceNo != null && t.AppReferenceNo.Contains(searchTerm))))
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<Ticket> AssignTicketAsync(int ticketId, int userId)
        {
            var ticket = await GetTicketByIdAsync(ticketId);
            if (ticket == null) throw new ArgumentException("Ticket not found");

            ticket.AssignedToUserId = userId;
            ticket.ModifiedDate = DateTime.Now;

            if (ticket.Status == "Open")
                ticket.Status = "In Progress";

            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket> ChangeTicketStatusAsync(int ticketId, string status)
        {
            var ticket = await GetTicketByIdAsync(ticketId);
            if (ticket == null) throw new ArgumentException("Ticket not found");

            ticket.Status = status;
            ticket.ModifiedDate = DateTime.Now;

            if (status == "Resolved")
                ticket.ResolvedDate = DateTime.Now;
            else if (status == "Closed")
                ticket.ClosedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<TicketAttachment> AddAttachmentAsync(TicketAttachment attachment)
        {
            attachment.UploadedDate = DateTime.Now;

            _context.TicketAttachments.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<List<TicketAttachment>> GetTicketAttachmentsAsync(int ticketId)
        {

            var attachments = await _context.TicketAttachments
                .Include(ta => ta.UploadedByUser)
                .Where(ta => ta.TicketId == ticketId)
                .OrderByDescending(ta => ta.UploadedDate)
                .ToListAsync();



            if (attachments.Count == 0)
            {
                var allAttachments = await _context.TicketAttachments.ToListAsync();

                foreach (var att in allAttachments)
                {

                }
            }
            else
            {
                foreach (var attachment in attachments)
                {

                }
            }

            return attachments;
        }

        public async Task<TicketAttachment?> GetTicketAttachmentByIdAsync(int attachmentId)
        {

            var attachment = await _context.TicketAttachments
                .Include(ta => ta.UploadedByUser)
                .FirstOrDefaultAsync(ta => ta.Id == attachmentId);

            if (attachment == null)
            {

                // Debug: Show what attachments DO exist
                var allAttachments = await _context.TicketAttachments.Select(a => new { a.Id, a.FileName }).ToListAsync();

                foreach (var att in allAttachments)
                {

                }
            }
            else
            {

            }

            return attachment;
        }

        public async Task<byte[]> DownloadAttachmentAsync(int attachmentId)
        {

            var attachment = await _context.TicketAttachments
                .FirstOrDefaultAsync(ta => ta.Id == attachmentId);

            if (attachment == null)
            {
                throw new FileNotFoundException("Attachment not found");
            }
            string actualFilePath = attachment.FilePath;

            if (actualFilePath.StartsWith("/") && OperatingSystem.IsWindows())
            {
                actualFilePath = actualFilePath.Replace("/", "\\");

                string[] possibleBasePaths = {
            $"C:{actualFilePath}",
            $"D:{actualFilePath}",
            $".{actualFilePath}",
            $"{Environment.CurrentDirectory}{actualFilePath}",
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", actualFilePath.TrimStart('\\')),
            Path.Combine(Directory.GetCurrentDirectory(), actualFilePath.TrimStart('\\'))
        };

                foreach (string testPath in possibleBasePaths)
                {
                    if (File.Exists(testPath))
                    {
                        actualFilePath = testPath;
                        break;
                    }
                }
            }

            if (!File.Exists(actualFilePath))
            {
                var commonUploadDirs = new[] {
            Path.Combine(Directory.GetCurrentDirectory(), "uploads"),
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"),
            @"C:\uploads",
            @"D:\uploads"
        };

                foreach (var dir in commonUploadDirs)
                {
                    if (Directory.Exists(dir))
                    {

                        var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);

                        foreach (var file in files.Take(5))
                        {

                        }
                    }
                }

                throw new FileNotFoundException($"File not found at path: {actualFilePath}");
            }

            var fileBytes = await File.ReadAllBytesAsync(actualFilePath);

            return fileBytes;
        }

        public async Task<TicketComment> AddCommentAsync(TicketComment comment)
        {
            comment.CreatedDate = DateTime.Now;
            comment.IsDeleted = false;

            _context.TicketComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<TicketSignatory> AddTicketSignatoryAsync(TicketSignatory ticketSignatory)
        {
            ticketSignatory.CreatedDate = DateTime.Now;
            ticketSignatory.IsDeleted = false;

            _context.TicketSignatories.Add(ticketSignatory);
            await _context.SaveChangesAsync();
            return ticketSignatory;
        }

        public async Task<List<TicketSignatory>> GetTicketSignatoriesAsync(int ticketId)
        {
            return await _context.TicketSignatories
                .Where(ts => ts.TicketId == ticketId && !ts.IsDeleted)
                .OrderBy(ts => ts.CreatedDate)
                .ToListAsync();
        }
    }
}