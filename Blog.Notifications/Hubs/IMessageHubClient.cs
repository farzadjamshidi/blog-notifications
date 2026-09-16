namespace Blog.Notifications.Hubs;

public interface IMessageHubClient
{
    Task NewComment(Guid postId, string commentText);
}
