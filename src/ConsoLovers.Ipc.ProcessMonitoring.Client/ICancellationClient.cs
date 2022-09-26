﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICancellationClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.Client;

public interface ICancellationClient : IConfigurableClient
{
   bool RequestCancel();
}