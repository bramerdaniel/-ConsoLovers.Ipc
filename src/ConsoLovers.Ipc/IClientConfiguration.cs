﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientConfiguration.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using global::Grpc.Net.Client;

public interface IClientConfiguration
{
   #region Public Properties

   /// <summary>Gets the address the channel was created for.</summary>
   public string Address { get; }

   /// <summary>Gets the <see cref="GrpcChannel"/> the client should use.</summary>
   public GrpcChannel Channel { get; }

   #endregion
}