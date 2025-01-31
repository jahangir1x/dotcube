﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using DeltaShare.Model;
using DeltaShare.Util;
using MimeKit;

namespace DeltaShare.Service
{
    public sealed class PoolCreatorServerService : IDisposable
    {
        private readonly string creatorIpAddress = "192.168.1.106";
        private CancellationTokenSource? cancellationTokenSource;
        private Task? listenTask;
        public ObservableCollection<User> PoolUsers { get; } = new();

        public PoolCreatorServerService()
        {
            PoolUsers.Add(new User("You", "", "You", creatorIpAddress, true));
        }

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
        }

        public void StartListening()
        {
            cancellationTokenSource = new CancellationTokenSource();
            listenTask = Task.Run(() => Listen(cancellationTokenSource.Token));
        }

        public void StopListening()
        {
            cancellationTokenSource?.Cancel();
            listenTask?.Wait();
        }

        private async Task Listen(CancellationToken cancellationToken)
        {
            string prefix = $"http://{creatorIpAddress}:9898/";
            int maxConcurrentRequests = 10;
            HttpListener listener = new();
            listener.Prefixes.Add(prefix);
            listener.Start();

            HashSet<Task> requests = new();
            for (int requestIdx = 0; requestIdx < maxConcurrentRequests; requestIdx++)
            {
                requests.Add(listener.GetContextAsync());
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                Task task = await Task.WhenAny(requests);
                requests.Remove(task);

                if (task is Task<HttpListenerContext> contextTask)
                {
                    HttpListenerContext ctx = await contextTask;
                    requests.Add(ProcessRequestAsync(ctx));
                    requests.Add(listener.GetContextAsync());
                }
            }

            listener.Stop();
            listener.Close();
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            try
            {
                Dictionary<string, MimePart> formParts = await MultipartParser.Parse(context, "/clients");

                User user = new(
                    await new StreamReader(formParts["Name"].Content.Stream).ReadToEndAsync(),
                    await new StreamReader(formParts["Email"].Content.Stream).ReadToEndAsync(),
                    await new StreamReader(formParts["Username"].Content.Stream).ReadToEndAsync(),
                    context.Request.RemoteEndPoint.Address.ToString(),
                    false
                );
                Debug.WriteLine($"Received new user: {user}");
                PoolUsers.Add(user);
                MultipartParser.SendResponse(context, "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing request: {ex.Message}");
            }
        }

    }
}
