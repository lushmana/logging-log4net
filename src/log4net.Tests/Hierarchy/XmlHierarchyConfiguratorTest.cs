/*
 *
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

using System.Xml;
using NUnit.Framework;

using log4net.Repository.Hierarchy;

namespace log4net.Tests.Hierarchy;

[TestFixture]
public class XmlHierarchyConfiguratorTest
{
  public string? TestProp { set; get; }

  [Test]
  [Platform(Include="Win")]
  public void EnvironmentOnWindowsIsCaseInsensitive()
  {
    SetTestPropWithPath();
    Assert.That(TestProp, Is.Not.EqualTo("Path="));
  }

  [Test]
  [Platform(Include="Unix")]
  public void EnvironmentOnUnixIsCaseSensitive()
  {
    SetTestPropWithPath();
    Assert.That(TestProp, Is.EqualTo("Path="));
  }

  private void SetTestPropWithPath()
  {
    XmlDocument doc = new();
    XmlElement el = doc.CreateElement("param");
    el.SetAttribute("name", "TestProp");
    el.SetAttribute("value", "Path=${path}");
    new TestConfigurator().PublicSetParameter(el, this);
  }

  // workaround for SetParameter being protected
  private sealed class TestConfigurator : XmlHierarchyConfigurator
  {
    public TestConfigurator() : base(null!)
    {
    }

    public void PublicSetParameter(XmlElement element, object target) 
    {
      SetParameter(element, target);
    }
  }
}
