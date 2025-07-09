using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        public bool CanAccessAllTickets(User user)
        {
            return user.Role == "legalstaff" || user.Role == "legalcounsel" || user.Role == "requester";
        }

        public bool CanCreateRequests(User user)
        {
            return user.Role == "legalstaff" || user.Role == "requester";
        }

        public bool CanAccessUsers(User user)
        {
            return user.Role == "legalstaff";
        }

        public bool CanViewTicket(User user, Ticket ticket)
        {
            switch (user.Role)
            {
                case "legalstaff":
                    return true;

                case "legalcounsel":
                    return ticket.AssignedToUserId == user.Id || ticket.CreatedBy == user.Id;

                case "requester":
                    return ticket.CreatedBy == user.Id || ticket.RequesterEmail == user.Email;

                default:
                    return false;
            }
        }

        public bool CanViewDocumentRequest(User user, DocumentRequest documentRequest)
        {
            switch (user.Role)
            {
                case "legalstaff":
                    return true;

                case "legalcounsel":
                    return documentRequest.CreatedBy == user.Id || documentRequest.RequesterEmail == user.Email;

                case "requester":
                    return documentRequest.CreatedBy == user.Id || documentRequest.RequesterEmail == user.Email;

                default:
                    return false;
            }
        }

        public bool CanEditTicket(User user, Ticket ticket)
        {
            switch (user.Role)
            {
                case "legalstaff":
                    return true;

                case "legalcounsel":
                    return ticket.AssignedToUserId == user.Id;

                case "requester":
                    return (ticket.CreatedBy == user.Id || ticket.RequesterEmail == user.Email)
                           && ticket.Status == "Pending";

                default:
                    return false;
            }
        }

        public bool CanDeleteTicket(User user, Ticket ticket)
        {
            return user.Role == "legalstaff";
        }

        public bool CanAssignTicket(User user, Ticket ticket)
        {
            return user.Role == "legalstaff";
        }

        public bool CanManageUsers(User user)
        {
            return user.Role == "legalstaff";
        }

        public bool CanAccessPage(User user, string pagePath)
        {
            switch (pagePath.ToLower())
            {
                case "/tickets":
                    return CanAccessAllTickets(user);

                case "/create":
                case "/tickets/create":
                case "/documents/create":
                    return CanCreateRequests(user);

                case "/users":
                    return CanAccessUsers(user);

                default:
                    return true;
            }
        }

        public bool CanChangeTicketStatus(User user, Ticket ticket)
        {
            switch (user.Role)
            {
                case "legalstaff":
                    return true;
                case "legalcounsel":
                    return ticket.AssignedToUserId == user.Id;
                case "requester":
                    // Requester can only change status under specific conditions
                    return CanRequesterChangeTicketStatus(user, ticket);
                default:
                    return false;
            }
        }

        private bool CanRequesterChangeTicketStatus(User user, Ticket ticket)
        {
            bool isRequester = ticket.CreatedBy == user.Id || ticket.RequesterEmail == user.Email;
            if (!isRequester) return false;

            return (ticket.Status == "Rejected") || (ticket.Status == "Closed");
        }

        public List<string> GetAllowedStatusChanges(User user, string currentStatus, string requestType)
        {
            var allowedStatuses = new List<string>();

            if (user == null) return allowedStatuses;

            switch (user.Role.ToLower())
            {
                case "legalstaff":
                    allowedStatuses.AddRange(new[] { "Acknowledge", "Endorse", "Closed", "Executed", "Rejected" });
                    break;

                case "legalcounsel":
                    allowedStatuses.AddRange(new[] { "Closed", "Rejected" });
                    break;

                case "requester":
                    if (currentStatus == "Rejected")
                    {
                        allowedStatuses.Add("Pending");
                    }
                    else if (currentStatus == "Closed")
                    {
                        allowedStatuses.Add("Executed");
                    }
                    break;
            }

            return allowedStatuses;
        }

        public bool CanChangeToStatus(User user, string currentStatus, string newStatus, string requestType)
        {
            if (user == null) return false;

            var allowedStatuses = GetAllowedStatusChanges(user, currentStatus, requestType);
            return allowedStatuses.Contains(newStatus);
        }

        public bool CanChangeDocumentRequestStatus(User user, DocumentRequest documentRequest)
        {
            switch (user.Role)
            {
                case "legalstaff":
                    return true;
                case "legalcounsel":
                    return documentRequest.CreatedBy == user.Id || documentRequest.RequesterEmail == user.Email;
                case "requester":
                    return CanRequesterChangeDocumentStatus(user, documentRequest);
                default:
                    return false;
            }
        }

        private bool CanRequesterChangeDocumentStatus(User user, DocumentRequest documentRequest)
        {
            bool isRequester = documentRequest.CreatedBy == user.Id || documentRequest.RequesterEmail == user.Email;
            if (!isRequester) return false;

            return (documentRequest.Status == "Rejected") || (documentRequest.Status == "Closed");
        }

        public bool CanDeleteDocumentRequest(User user, DocumentRequest documentRequest)
        {
            return user.Role == "legalstaff";
        }
    }
}