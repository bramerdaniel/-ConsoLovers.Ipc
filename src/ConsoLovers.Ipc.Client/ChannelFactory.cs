﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

using global::Grpc.Core;
using global::Grpc.Net.Client;


/// <summary>Helper factory for creating <see cref="GrpcChannel"/>s</summary>
internal class ChannelFactory : IChannelFactory
{
   private readonly string socketPath;

   #region Constants and Fields

   private GrpcChannel? channel;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="ChannelFactory"/> class.</summary>
   /// <exception cref="System.ArgumentNullException">serverName</exception>
   internal ChannelFactory(string socketPath)
   {
      this.socketPath = socketPath ?? throw new ArgumentNullException(nameof(socketPath));
      ServerName = Path.GetFileNameWithoutExtension(socketPath);
   }

   #endregion

   #region IChannelFactory Members

   /// <summary>Gets the serverName.</summary>
   public string ServerName { get; }

   /// <summary>Gets the channel.</summary>
   public GrpcChannel Channel => channel ??= CreateChannelFromPath();

   #endregion

   #region Methods

   private GrpcChannel CreateChannelFromPath()
   {
      var credentials = CallCredentials.FromInterceptor((context, metadata) =>
      {
         metadata.Add("Language", "en-US");
         return Task.CompletedTask;
      });
      
      var channelCredentials = ChannelCredentials.Create(ChannelCredentials.SecureSsl, credentials);


      var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
      var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
      var socketsHttpHandler = new SocketsHttpHandler
      {
         ConnectCallback = connectionFactory.ConnectAsync,
         Proxy = new WebProxy(),
         
      };
      
      var grpcChannel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
      {
         HttpHandler = socketsHttpHandler,
         // Credentials = channelCredentials
      });

      return grpcChannel;
   }

   #endregion
}