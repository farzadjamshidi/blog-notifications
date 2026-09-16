# Blog.Notifications

A standalone worker service that reacts to events published by `Blog.API`
over RabbitMQ — the first real, independently-deployed service split out
of the `blog` monolith. Consumes `NewCommentNotification` messages; today
it only logs them (no live delivery to a client yet — see
`../learning-notes/` for where this is headed).

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A reachable RabbitMQ instance — normally provided by `../blog/docker-compose.yml`,
  which runs this service alongside `Blog.API`, SQL Server, Redis, and
  MongoDB. See `../blog/README.md`.

## Getting started

```bash
cd Blog.Notifications
dotnet run
```

Without `RabbitMQ:Host` configured, this falls back to MassTransit's
in-memory transport — the process starts fine, but nothing outside it
ever publishes a message for it to consume. Set `RabbitMQ__Host` (or run
via `../blog/docker-compose.yml`, which sets it for you) to actually
receive messages from `Blog.API`.

## Learn more

Part of the same running C#/.NET learning series as `blog` and
`blog-blazor` — see `../learning-notes/` (`./build.sh` there generates a
PDF of the whole thing).
