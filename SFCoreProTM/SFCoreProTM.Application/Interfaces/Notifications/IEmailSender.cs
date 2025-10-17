using System.Threading;
using System.Threading.Tasks;

namespace SFCoreProTM.Application.Interfaces.Notifications;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
}

