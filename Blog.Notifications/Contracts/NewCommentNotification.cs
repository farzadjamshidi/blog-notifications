namespace Blog.Contracts;

// Deliberately duplicated in Blog.API/Contracts (not shared via project
// reference or a published package — the two repos are genuinely
// separate, and a real contracts package is more overhead than this
// learning project needs yet). MassTransit derives its exchange/queue
// names from the message type's *fully-qualified* name by default — so
// the namespace here (Blog.Contracts, deliberately neither Blog.API's nor
// Blog.Notifications' own root namespace) must match byte-for-byte on
// both sides, or the publisher and consumer silently end up talking to
// two different exchanges and no message ever arrives.
public record NewCommentNotification(Guid RecipientUserProfileId, Guid PostId, string CommentText);
