#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Reflection;
using System.Text;
using log4net.Util;
using log4net.Repository;

namespace log4net.Core;

/// <summary>
/// Static manager that controls the creation of repositories
/// </summary>
/// <remarks>
/// <para>
/// Static manager that controls the creation of repositories
/// </para>
/// <para>
/// This class is used by the wrapper managers (e.g. <see cref="LogManager"/>)
/// to provide access to the <see cref="ILogger"/> objects.
/// </para>
/// <para>
/// This manager also holds the <see cref="IRepositorySelector"/> that is used to
/// lookup and create repositories. The selector can be set either programmatically using
/// the <see cref="RepositorySelector"/> property, or by setting the <c>log4net.RepositorySelector</c>
/// AppSetting in the applications config file to the fully qualified type name of the
/// selector to use. 
/// </para>
/// </remarks>
/// <author>Nicko Cadell</author>
/// <author>Gert Driesen</author>
public static class LoggerManager
{
  /// <summary>
  /// Hook the shutdown event
  /// </summary>
  /// <remarks>
  /// <para>
  /// On the full .NET runtime, the static constructor hooks up the 
  /// <c>AppDomain.ProcessExit</c> and <c>AppDomain.DomainUnload</c>> events. 
  /// These are used to shut down the log4net system as the application exits.
  /// </para>
  /// </remarks>
  static LoggerManager()
  {
    try
    {
      // Register the AppDomain events, note we have to do this with a
      // method call rather than directly here because the AppDomain
      // makes a LinkDemand which throws the exception during the JIT phase.
      RegisterAppDomainEvents();
    }
    catch (System.Security.SecurityException)
    {
      LogLog.Debug(_declaringType, "Security Exception (ControlAppDomain LinkDemand) while trying " +
        "to register Shutdown handler with the AppDomain. LoggerManager.Shutdown() " +
        "will not be called automatically when the AppDomain exits. It must be called " +
        "programmatically.");
    }

    // Dump out our assembly version into the log if debug is enabled
    LogLog.Debug(_declaringType, GetVersionInfo());

    // Set the default repository selector
    // Look for the RepositorySelector type specified in the AppSettings 'log4net.RepositorySelector'
    string? appRepositorySelectorTypeName = SystemInfo.GetAppSetting("log4net.RepositorySelector");
    if (!string.IsNullOrEmpty(appRepositorySelectorTypeName))
    {
      // Resolve the config string into a Type
      Type? appRepositorySelectorType = null;
      try
      {
        appRepositorySelectorType = SystemInfo.GetTypeFromString(appRepositorySelectorTypeName!, false, true);
      }
      catch (Exception e) when (!e.IsFatal())
      {
        LogLog.Error(_declaringType, $"Exception while resolving RepositorySelector Type [{appRepositorySelectorTypeName}]", e);
      }

      if (appRepositorySelectorType is not null)
      {
        // Create an instance of the RepositorySelectorType
        object? appRepositorySelectorObj = null;
        try
        {
          appRepositorySelectorObj = Activator.CreateInstance(appRepositorySelectorType);
        }
        catch (Exception e) when (!e.IsFatal())
        {
          LogLog.Error(_declaringType, $"Exception while creating RepositorySelector [{appRepositorySelectorType.FullName}]", e);
        }

        if (appRepositorySelectorObj is IRepositorySelector sel)
        {
          RepositorySelector = sel;
        }
        else
        {
          LogLog.Error(_declaringType, $"RepositorySelector Type [{appRepositorySelectorType.FullName}] is not an IRepositorySelector");
        }
      }
    }
    // Create the DefaultRepositorySelector if not configured above 
    RepositorySelector ??= new DefaultRepositorySelector(typeof(Repository.Hierarchy.Hierarchy));
  }

  /// <summary>
  /// Register for ProcessExit and DomainUnload events on the AppDomain
  /// </summary>
  /// <remarks>
  /// <para>
  /// This needs to be in a separate method because the events make
  /// a LinkDemand for the ControlAppDomain SecurityPermission. Because
  /// this is a LinkDemand it is demanded at JIT time. Therefore we cannot
  /// catch the exception in the method itself, we have to catch it in the
  /// caller.
  /// </para>
  /// </remarks>
  private static void RegisterAppDomainEvents()
  {
    // ProcessExit seems to be fired if we are part of the default domain
    AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

    // Otherwise DomainUnload is fired
    AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
  }

  /// <summary>
  /// Return the default <see cref="ILoggerRepository"/> instance.
  /// </summary>
  /// <param name="repository">the repository to lookup in</param>
  /// <returns>Return the default <see cref="ILoggerRepository"/> instance</returns>
  /// <remarks>
  /// <para>
  /// Gets the <see cref="ILoggerRepository"/> for the repository specified
  /// by the <paramref name="repository"/> argument.
  /// </para>
  /// </remarks>
  public static ILoggerRepository GetRepository(string repository)
    => RepositorySelector.GetRepository(repository.EnsureNotNull());

  /// <summary>
  /// Returns the default <see cref="ILoggerRepository"/> instance.
  /// </summary>
  /// <param name="repositoryAssembly">The assembly to use to look up the repository.</param>
  /// <returns>The default <see cref="ILoggerRepository"/> instance.</returns>
  /// <remarks>
  /// <para>
  /// Returns the default <see cref="ILoggerRepository"/> instance.
  /// </para>
  /// </remarks>
  public static ILoggerRepository GetRepository(Assembly repositoryAssembly)
    => RepositorySelector.GetRepository(repositoryAssembly.EnsureNotNull());

  /// <summary>
  /// Returns the named logger if it exists.
  /// </summary>
  /// <param name="repository">The repository to lookup in.</param>
  /// <param name="name">The fully qualified logger name to look for.</param>
  /// <returns>
  /// The logger found, or <c>null</c> if the named logger does not exist in the
  /// specified repository.
  /// </returns>
  /// <remarks>
  /// <para>
  /// If the named logger exists (in the specified repository) then it
  /// returns a reference to the logger, otherwise it returns
  /// <c>null</c>.
  /// </para>
  /// </remarks>
  public static ILogger? Exists(string repository, string name)
    => RepositorySelector.GetRepository(repository.EnsureNotNull()).Exists(name.EnsureNotNull());

  /// <summary>
  /// Returns the named logger if it exists.
  /// </summary>
  /// <param name="repositoryAssembly">The assembly to use to look up the repository.</param>
  /// <param name="name">The fully qualified logger name to look for.</param>
  /// <returns>
  /// The logger found, or <c>null</c> if the named logger does not exist in the
  /// specified assembly's repository.
  /// </returns>
  /// <remarks>
  /// <para>
  /// If the named logger exists (in the specified assembly's repository) then it
  /// returns a reference to the logger, otherwise it returns
  /// <c>null</c>.
  /// </para>
  /// </remarks>
  public static ILogger? Exists(Assembly repositoryAssembly, string name)
    => RepositorySelector.GetRepository(repositoryAssembly.EnsureNotNull()).Exists(name.EnsureNotNull());

  /// <summary>
  /// Returns all the currently defined loggers in the specified repository.
  /// </summary>
  /// <param name="repository">The repository to lookup in.</param>
  /// <returns>All the defined loggers.</returns>
  /// <remarks>
  /// <para>
  /// The root logger is <b>not</b> included in the returned array.
  /// </para>
  /// </remarks>
  public static ILogger[] GetCurrentLoggers(string repository)
    => RepositorySelector.GetRepository(repository.EnsureNotNull()).GetCurrentLoggers();

  /// <summary>
  /// Returns all the currently defined loggers in the specified assembly's repository.
  /// </summary>
  /// <param name="repositoryAssembly">The assembly to use to look up the repository.</param>
  /// <returns>All the defined loggers.</returns>
  /// <remarks>
  /// <para>
  /// The root logger is <b>not</b> included in the returned array.
  /// </para>
  /// </remarks>
  public static ILogger[] GetCurrentLoggers(Assembly repositoryAssembly)
    => RepositorySelector.GetRepository(repositoryAssembly.EnsureNotNull()).GetCurrentLoggers();

  /// <summary>
  /// Retrieves or creates a named logger.
  /// </summary>
  /// <param name="repository">The repository to lookup in.</param>
  /// <param name="name">The name of the logger to retrieve.</param>
  /// <returns>The logger with the name specified.</returns>
  /// <remarks>
  /// <para>
  /// Retrieves a logger named as the <paramref name="name"/>
  /// parameter. If the named logger already exists, then the
  /// existing instance will be returned. Otherwise, a new instance is
  /// created.
  /// </para>
  /// <para>
  /// By default, loggers do not have a set level but inherit
  /// it from the hierarchy. This is one of the central features of
  /// log4net.
  /// </para>
  /// </remarks>
  public static ILogger GetLogger(string repository, string name)
    => RepositorySelector.GetRepository(repository.EnsureNotNull()).GetLogger(name.EnsureNotNull());

  /// <summary>
  /// Retrieves or creates a named logger.
  /// </summary>
  /// <param name="repositoryAssembly">The assembly to use to look up the repository.</param>
  /// <param name="name">The name of the logger to retrieve.</param>
  /// <returns>The logger with the name specified.</returns>
  /// <remarks>
  /// <para>
  /// Retrieves a logger named as the <paramref name="name"/>
  /// parameter. If the named logger already exists, then the
  /// existing instance will be returned. Otherwise, a new instance is
  /// created.
  /// </para>
  /// <para>
  /// By default, loggers do not have a set level but inherit
  /// it from the hierarchy. This is one of the central features of
  /// log4net.
  /// </para>
  /// </remarks>
  public static ILogger GetLogger(Assembly repositoryAssembly, string name)
    => RepositorySelector.GetRepository(repositoryAssembly.EnsureNotNull()).GetLogger(name.EnsureNotNull());

  /// <summary>
  /// Shorthand for <see cref="LogManager.GetLogger(string)"/>.
  /// </summary>
  /// <param name="repository">The repository to lookup in.</param>
  /// <param name="type">The <paramref name="type"/> of which the fullname will be used as the name of the logger to retrieve.</param>
  /// <returns>The logger with the name specified.</returns>
  /// <remarks>
  /// <para>
  /// Gets the logger for the fully qualified name of the type specified.
  /// </para>
  /// </remarks>
  public static ILogger GetLogger(string repository, Type type)
  {
    if (type.EnsureNotNull().FullName is not string name)
    {
      throw new ArgumentException($"Type {type} does not have a full name", nameof(type));
    }
    return RepositorySelector.GetRepository(repository.EnsureNotNull()).GetLogger(name);
  }

  /// <summary>
  /// Shorthand for <see cref="LogManager.GetLogger(string)"/>.
  /// </summary>
  /// <param name="repositoryAssembly">the assembly to use to look up the repository</param>
  /// <param name="type">The <paramref name="type"/> of which the fullname will be used as the name of the logger to retrieve.</param>
  /// <returns>The logger with the name specified.</returns>
  /// <remarks>
  /// <para>
  /// Gets the logger for the fully qualified name of the type specified.
  /// </para>
  /// </remarks>
  public static ILogger GetLogger(Assembly repositoryAssembly, Type type)
  {
    if (type.EnsureNotNull().FullName is not string name)
    {
      throw new ArgumentException($"Type {type} does not have a full name", nameof(type));
    }
    return RepositorySelector.GetRepository(repositoryAssembly.EnsureNotNull()).GetLogger(name);
  }

  /// <summary>
  /// Shuts down the log4net system.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Calling this method will <b>safely</b> close and remove all
  /// appenders in all the loggers including root contained in all the
  /// default repositories.
  /// </para>
  /// <para>
  /// Some appenders need to be closed before the application exists. 
  /// Otherwise, pending logging events might be lost.
  /// </para>
  /// <para>
  /// The <c>shutdown</c> method is careful to close nested
  /// appenders before closing regular appenders. This allows
  /// configurations where a regular appender is attached to a logger
  /// and again to a nested appender.
  /// </para>
  /// </remarks>
  public static void Shutdown()
  {
    // Cleanup event handlers since they only call this method anyway
    AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
    AppDomain.CurrentDomain.DomainUnload -= OnDomainUnload;
    
    foreach (ILoggerRepository repository in GetAllRepositories())
    {
      repository.Shutdown();
    }
  }

  /// <summary>
  /// Shuts down the repository for the repository specified.
  /// </summary>
  /// <param name="repository">The repository to shut down.</param>
  /// <remarks>
  /// <para>
  /// Calling this method will <b>safely</b> close and remove all
  /// appenders in all the loggers including root contained in the
  /// repository for the <paramref name="repository"/> specified.
  /// </para>
  /// <para>
  /// Some appenders need to be closed before the application exists. 
  /// Otherwise, pending logging events might be lost.
  /// </para>
  /// <para>
  /// The <c>shutdown</c> method is careful to close nested
  /// appenders before closing regular appenders. This allows
  /// configurations where a regular appender is attached to a logger
  /// and again to a nested appender.
  /// </para>
  /// </remarks>
  public static void ShutdownRepository(string repository)
    => RepositorySelector.GetRepository(repository.EnsureNotNull()).Shutdown();

  /// <summary>
  /// Shuts down the repository for the repository specified.
  /// </summary>
  /// <param name="repositoryAssembly">The assembly to use to look up the repository.</param>
  /// <remarks>
  /// <para>
  /// Calling this method will <b>safely</b> close and remove all
  /// appenders in all the loggers including root contained in the
  /// repository for the repository. The repository is looked up using
  /// the <paramref name="repositoryAssembly"/> specified.
  /// </para>
  /// <para>
  /// Some appenders need to be closed before the application exists. 
  /// Otherwise, pending logging events might be lost.
  /// </para>
  /// <para>
  /// The <c>shutdown</c> method is careful to close nested
  /// appenders before closing regular appenders. This allows
  /// configurations where a regular appender is attached to a logger
  /// and again to a nested appender.
  /// </para>
  /// </remarks>
  public static void ShutdownRepository(Assembly repositoryAssembly)
    => RepositorySelector.GetRepository(repositoryAssembly.EnsureNotNull()).Shutdown();

  /// <summary>
  /// Resets all values contained in this repository instance to their defaults.
  /// </summary>
  /// <param name="repository">The repository to reset.</param>
  /// <remarks>
  /// <para>
  /// Resets all values contained in the repository instance to their
  /// defaults.  This removes all appenders from all loggers, sets
  /// the level of all non-root loggers to <c>null</c>,
  /// sets their additivity flag to <c>true</c> and sets the level
  /// of the root logger to <see cref="Level.Debug"/>. Moreover,
  /// message disabling is set its default "off" value.
  /// </para>    
  /// </remarks>
  public static void ResetConfiguration(string repository)
    => RepositorySelector.GetRepository(repository.EnsureNotNull()).ResetConfiguration();

  /// <summary>
  /// Resets all values contained in this repository instance to their defaults.
  /// </summary>
  /// <param name="repositoryAssembly">The assembly to use to look up the repository to reset.</param>
  /// <remarks>
  /// <para>
  /// Resets all values contained in the repository instance to their
  /// defaults.  This removes all appenders from all loggers, sets
  /// the level of all non-root loggers to <c>null</c>,
  /// sets their additivity flag to <c>true</c> and sets the level
  /// of the root logger to <see cref="Level.Debug"/>. Moreover,
  /// message disabling is set its default "off" value.
  /// </para>    
  /// </remarks>
  public static void ResetConfiguration(Assembly repositoryAssembly)
    => RepositorySelector.GetRepository(repositoryAssembly.EnsureNotNull()).ResetConfiguration();

  /// <summary>
  /// Creates a repository with the specified name.
  /// </summary>
  /// <param name="repository">The name of the repository, this must be unique amongst repositories.</param>
  /// <returns>The <see cref="ILoggerRepository"/> created for the repository.</returns>
  /// <remarks>
  /// <para>
  /// Creates the default type of <see cref="ILoggerRepository"/> which is a
  /// <see cref="log4net.Repository.Hierarchy.Hierarchy"/> object.
  /// </para>
  /// <para>
  /// The <paramref name="repository"/> name must be unique. Repositories cannot be redefined.
  /// An <see cref="Exception"/> will be thrown if the repository already exists.
  /// </para>
  /// </remarks>
  /// <exception cref="LogException">The specified repository already exists.</exception>
  public static ILoggerRepository CreateRepository(string repository)
    => RepositorySelector.CreateRepository(repository.EnsureNotNull(), null);

  /// <summary>
  /// Creates a repository with the specified name and repository type.
  /// </summary>
  /// <param name="repository">The name of the repository, this must be unique to the repository.</param>
  /// <param name="repositoryType">A <see cref="Type"/> that implements <see cref="ILoggerRepository"/>
  /// and has a no arg constructor. An instance of this type will be created to act
  /// as the <see cref="ILoggerRepository"/> for the repository specified.</param>
  /// <returns>The <see cref="ILoggerRepository"/> created for the repository.</returns>
  /// <remarks>
  /// <para>
  /// The <paramref name="repository"/> name must be unique. Repositories cannot be redefined.
  /// An Exception will be thrown if the repository already exists.
  /// </para>
  /// </remarks>
  /// <exception cref="LogException">The specified repository already exists.</exception>
  public static ILoggerRepository CreateRepository(string repository, Type repositoryType)
    => RepositorySelector.CreateRepository(repository.EnsureNotNull(), repositoryType.EnsureNotNull());

  /// <summary>
  /// Creates a repository for the specified assembly and repository type.
  /// </summary>
  /// <param name="repositoryAssembly">The assembly to use to get the name of the repository.</param>
  /// <param name="repositoryType">A <see cref="Type"/> that implements <see cref="ILoggerRepository"/>
  /// and has a no arg constructor. An instance of this type will be created to act
  /// as the <see cref="ILoggerRepository"/> for the repository specified.</param>
  /// <returns>The <see cref="ILoggerRepository"/> created for the repository.</returns>
  /// <remarks>
  /// <para>
  /// The <see cref="ILoggerRepository"/> created will be associated with the repository
  /// specified such that a call to <see cref="GetRepository(Assembly)"/> with the
  /// same assembly specified will return the same repository instance.
  /// </para>
  /// </remarks>
  public static ILoggerRepository CreateRepository(Assembly repositoryAssembly, Type repositoryType)
    => RepositorySelector.CreateRepository(repositoryAssembly.EnsureNotNull(), repositoryType.EnsureNotNull());

  /// <summary>
  /// Gets an array of all currently defined repositories.
  /// </summary>
  /// <returns>An array of all the known <see cref="ILoggerRepository"/> objects.</returns>
  /// <remarks>
  /// <para>
  /// Gets an array of all currently defined repositories.
  /// </para>
  /// </remarks>
  public static ILoggerRepository[] GetAllRepositories() => RepositorySelector.GetAllRepositories();

  /// <summary>
  /// Gets or sets the repository selector used by the <see cref="LogManager" />.
  /// </summary>
  /// <value>
  /// The repository selector used by the <see cref="LogManager" />.
  /// </value>
  /// <remarks>
  /// <para>
  /// The repository selector (<see cref="IRepositorySelector"/>) is used by 
  /// the <see cref="LogManager"/> to create and select repositories 
  /// (<see cref="ILoggerRepository"/>).
  /// </para>
  /// <para>
  /// The caller to <see cref="LogManager"/> supplies either a string name 
  /// or an assembly (if not supplied the assembly is inferred using 
  /// <see cref="Assembly.GetCallingAssembly()"/>).
  /// </para>
  /// <para>
  /// This context is used by the selector to look up a specific repository.
  /// </para>
  /// </remarks>
  public static IRepositorySelector RepositorySelector { get; set; }

  /// <summary>
  /// Internal method to get pertinent version info.
  /// </summary>
  /// <returns>A string of version info.</returns>
  private static string GetVersionInfo()
  {
    StringBuilder sb = new();

    Assembly myAssembly = Assembly.GetExecutingAssembly();
    sb.Append("log4net assembly [").Append(myAssembly.FullName).Append("]. ");
    sb.Append("Loaded from [").Append(SystemInfo.AssemblyLocationInfo(myAssembly)).Append("]. ");
    sb.Append("(.NET Runtime [").Append(Environment.Version).Append(']');
    sb.Append(" on ").Append(Environment.OSVersion);
    sb.Append(')');
    return sb.ToString();
  }

  /// <summary>
  /// Called when the <see cref="AppDomain.DomainUnload"/> event fires
  /// </summary>
  /// <param name="sender">the <see cref="AppDomain"/> that is exiting</param>
  /// <param name="e">null</param>
  /// <remarks>
  /// <para>
  /// Called when the <see cref="AppDomain.DomainUnload"/> event fires.
  /// </para>
  /// <para>
  /// When the event is triggered the log4net system is <see cref="Shutdown()"/>.
  /// </para>
  /// </remarks>
  private static void OnDomainUnload(object? sender, EventArgs e) => Shutdown();

  /// <summary>
  /// Called when the <see cref="AppDomain.ProcessExit"/> event fires
  /// </summary>
  /// <param name="sender">the <see cref="AppDomain"/> that is exiting</param>
  /// <param name="e">null</param>
  /// <remarks>
  /// <para>
  /// Called when the <see cref="AppDomain.ProcessExit"/> event fires.
  /// </para>
  /// <para>
  /// When the event is triggered the log4net system is <see cref="Shutdown()"/>.
  /// </para>
  /// </remarks>
  private static void OnProcessExit(object? sender, EventArgs e) => Shutdown();

  /// <summary>
  /// The fully qualified type of the LoggerManager class.
  /// </summary>
  /// <remarks>
  /// Used by the internal logger to record the Type of the
  /// log message.
  /// </remarks>
  private static readonly Type _declaringType = typeof(LoggerManager);
}
