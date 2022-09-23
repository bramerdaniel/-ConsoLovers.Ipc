﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowProgressLogic.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using Spectre.Console;

internal class ShowProgressLogic : IApplicationLogic<ClientArgs>
{
   #region Constants and Fields

   private readonly IClientFactory clientFactory;

   private IProgressClient? progressClient;

   private IResultClient? resultClient;

   #endregion

   #region Constructors and Destructors

   public ShowProgressLogic(IClientFactory clientFactory)
   {
      this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
   }

   #endregion

   #region IApplicationLogic<ClientArgs> Members

   public async Task ExecuteAsync(ClientArgs arguments, CancellationToken cancellationToken)
   {
      Console.Title = $"client [{clientFactory.ChannelFactory.ServerName}]";
      progressClient = clientFactory.CreateProgressClient();
      resultClient = clientFactory.CreateResultClient();

      await AnsiConsole.Progress()
         .Columns(new ProgressColumn[] { new SpinnerColumn(), new PercentageColumn(), new ProgressBarColumn(), new TaskDescriptionColumn() })
         .StartAsync(Update);

      Console.Clear();

      var resultInfo = await resultClient.WaitForResultAsync();
      Console.WriteLine($"{clientFactory.ChannelFactory.ServerName} exited with code {resultInfo.ExitCode}");
      foreach (var pair in resultInfo.Data)
         AnsiConsole.MarkupLine($"[blue]{pair.Key}[/] = [green]{pair.Value}[/]");

      Console.ReadLine();
   }

   #endregion

   #region Methods

   private async Task Update(ProgressContext progressContext)
   {
      var progress = progressContext.AddTask(clientFactory.ChannelFactory.ServerName);
      progressClient.ProgressChanged += OnProgressChanged;

      void OnProgressChanged(object? sender, ProgressEventArgs e)
      {
         progress.Value = e.Percentage;
         progress.Description = e.Message;
      }

      await resultClient.WaitForResultAsync();
   }

   #endregion
}