/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
*/

#if NET462_OR_GREATER
using System;
using System.Diagnostics;
using System.Reflection;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using NUnit.Framework;

namespace log4net.Tests.Appender;

[TestFixture]
public class DebugAppenderTest
{
  [Test]
  public void NullCategoryTest()
  {
    CategoryTraceListener categoryTraceListener = new();
    Debug.Listeners.Add(categoryTraceListener);

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

    DebugAppender debugAppender = new()
    {
      Layout = new SimpleLayout()
    };
    debugAppender.ActivateOptions();

    debugAppender.Category = null;

    TestErrorHandler testErrHandler = new();
    debugAppender.ErrorHandler = testErrHandler;

    BasicConfigurator.Configure(rep, debugAppender);

    ILog log = LogManager.GetLogger(rep.Name, GetType());
    log.Debug("Message");

    Assert.That(categoryTraceListener.Category, Is.Null);

    Assert.That(testErrHandler.ErrorOccured, Is.False);

    Debug.Listeners.Remove(categoryTraceListener);
  }

  [Test]
  public void EmptyStringCategoryTest()
  {
    CategoryTraceListener categoryTraceListener = new();
    Debug.Listeners.Add(categoryTraceListener);

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

    DebugAppender debugAppender = new()
    {
      Layout = new SimpleLayout()
    };
    debugAppender.ActivateOptions();

    debugAppender.Category = new PatternLayout("");

    BasicConfigurator.Configure(rep, debugAppender);

    ILog log = LogManager.GetLogger(rep.Name, GetType());
    log.Debug("Message");

    Assert.That(categoryTraceListener.Category, Is.Null);

    Debug.Listeners.Remove(categoryTraceListener);
  }

  [Test]
  public void DefaultCategoryTest()
  {
    CategoryTraceListener categoryTraceListener = new();
    Debug.Listeners.Add(categoryTraceListener);

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

    DebugAppender debugAppender = new()
    {
      Layout = new SimpleLayout()
    };
    debugAppender.ActivateOptions();

    BasicConfigurator.Configure(rep, debugAppender);

    ILog log = LogManager.GetLogger(rep.Name, GetType());
    log.Debug("Message");

    Assert.That(
      categoryTraceListener.Category,
      Is.EqualTo(GetType().ToString()));

    Debug.Listeners.Remove(categoryTraceListener);
  }

  [Test]
  public void MethodNameCategoryTest()
  {
    CategoryTraceListener categoryTraceListener = new();
    Debug.Listeners.Add(categoryTraceListener);

    ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

    DebugAppender debugAppender = new();
    PatternLayout methodLayout = new("%method");
    methodLayout.ActivateOptions();
    debugAppender.Category = methodLayout;
    debugAppender.Layout = new SimpleLayout();
    debugAppender.ActivateOptions();

    BasicConfigurator.Configure(rep, debugAppender);

    ILog log = LogManager.GetLogger(rep.Name, GetType());
    log.Debug("Message");

    Assert.That(
        categoryTraceListener.Category,
        Is.EqualTo(MethodInfo.GetCurrentMethod().Name));

    Debug.Listeners.Remove(categoryTraceListener);
  }

  private sealed class TestErrorHandler : IErrorHandler
  {
    public bool ErrorOccured { get; private set; }

    public void Error(string message, Exception? e, ErrorCode errorCode)
      => ErrorOccured = true;

    public void Error(string message, Exception e)
      => Error(message, e, ErrorCode.GenericFailure);

    public void Error(string message)
      => Error(message, null, ErrorCode.GenericFailure);
  }
}
#endif