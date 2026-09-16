using Blog.Contracts;
using Blog.Notifications.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Blog.Notifications.Consumers;

public class NewCommentNotificationConsumer(
    ILogger<NewCommentNotificationConsumer> logger,
    IHubContext<MessageHub, IMessageHubClient> hubContext)
    : IConsumer<NewCommentNotification>
{
    public Task Consume(ConsumeContext<NewCommentNotification> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "Received NewCommentNotification: recipient {RecipientUserProfileId}, post {PostId}, comment {CommentText}",
            message.RecipientUserProfileId,
            message.PostId,
            message.CommentText);

        return hubContext.Clients
            .User(message.RecipientUserProfileId.ToString())
            .NewComment(message.PostId, message.CommentText);
    }
}
