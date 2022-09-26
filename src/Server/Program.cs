﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Server;

using ConsoLovers.ConsoleToolkit.Core;

internal class Program
{
   #region Public Methods and Operators

   public static async Task Main()
   {
      await ConsoleApplication.WithArguments<ServerArgs>()
         .RunAsync();
   }

   #endregion
}