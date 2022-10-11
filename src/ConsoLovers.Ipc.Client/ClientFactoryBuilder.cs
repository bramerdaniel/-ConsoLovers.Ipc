﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFactoryBuilder.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.DependencyInjection;

internal class ClientFactoryBuilder : IClientFactoryBuilder, IClientFactoryBuilderWithoutName
{
   #region Constants and Fields

   private readonly ServiceCollection serviceCollection;

   private ChannelFactory? channelFactory;

   #endregion

   #region Constructors and Destructors

   public ClientFactoryBuilder()
   {
      serviceCollection = new ServiceCollection();
      serviceCollection.AddSingleton<IClientFactoryBuilder>(this);
   }

   #endregion

   #region IClientFactoryBuilder Members

   public IClientFactoryBuilder AddService(Action<ServiceCollection> services)
   {
      if (services == null)
         throw new ArgumentNullException(nameof(services));

      services(serviceCollection);
      return this;
   }

   public IClientFactoryBuilder WithCulture(CultureInfo culture)
   {
      // TODO
      return this;
   }

   public IClientFactory Build()
   {
      if (channelFactory == null)
         throw new InvalidOperationException($"The {nameof(channelFactory)} is not specified yet");

      var providerFactory = new DefaultServiceProviderFactory(new ServiceProviderOptions { ValidateOnBuild = true });
      var serviceProvider = providerFactory.CreateServiceProvider(serviceCollection);

      return new ClientFactory(serviceProvider, channelFactory);
   }

   #endregion

   #region IClientFactoryBuilderWithoutName Members

   public IClientFactoryBuilder ForName(string name)
   {
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      EnsureValidFileName(name);
      return WithSocketFile(() => Path.Combine(Path.GetTempPath(), $"{name}.uds"));
   }

   public IClientFactoryBuilder ForProcess(Process process)
   {
      if (process == null)
         throw new ArgumentNullException(nameof(process));
      
      return ForName($"{process.ProcessName}.{process.Id}");
   }

   public IClientFactoryBuilder WithSocketFile(Func<string> computeSocketFile)
   {
      if (computeSocketFile == null)
         throw new ArgumentNullException(nameof(computeSocketFile));

      var socketFile = computeSocketFile();
      EnsureValidFilePath(socketFile);

      channelFactory = new ChannelFactory(socketFile);
      serviceCollection.AddSingleton(channelFactory);
      return this;
   }

   #endregion

   #region Methods

   private static void EnsureValidFileName(string fileName, [CallerArgumentExpression("fileName")] string? callerExpression = null)
   {
      if (fileName is null)
         throw new ArgumentNullException(callerExpression);

      if (string.IsNullOrWhiteSpace(fileName))
         throw new ArgumentException(callerExpression, $"{callerExpression} must not be empty.");

      if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
         throw new ArgumentNullException(callerExpression, $"{callerExpression} is not a valid file name.");
   }

   private static void EnsureValidFilePath(string fileName, [CallerArgumentExpression("fileName")] string? callerExpression = null)
   {
      if (fileName is null)
         throw new ArgumentNullException(callerExpression);

      if (string.IsNullOrWhiteSpace(fileName))
         throw new ArgumentException(callerExpression, $"{callerExpression} must not be empty.");

      if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
         throw new ArgumentNullException(callerExpression, $"{callerExpression} is not a valid file name.");
   }

   #endregion
}