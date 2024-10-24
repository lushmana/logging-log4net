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

using System.IO;
using log4net.Util.PatternStringConverters;
using NUnit.Framework;

namespace log4net.Tests.Util;

/// <summary>
/// Used for internal unit testing the <see cref="RandomStringPatternConverter"/> class.
/// </summary>
/// <remarks>
/// Used for internal unit testing the <see cref="RandomStringPatternConverter"/> class.
/// </remarks>
[TestFixture]
public class RandomStringPatternConverterTest
{
  [Test]
  public void TestConvert()
  {
    RandomStringPatternConverter converter = new();

    // Check default string length
    StringWriter sw = new();
    converter.Convert(sw, null);

    Assert.That(sw.ToString(), Has.Length.EqualTo(4), "Default string length should be 4");

    // Set string length to 7
    converter.Option = "7";
    converter.ActivateOptions();

    sw = new StringWriter();
    converter.Convert(sw, null);

    string string1 = sw.ToString();
    Assert.That(string1, Has.Length.EqualTo(7), "string length should be 7");

    // Check for duplicate result
    sw = new StringWriter();
    converter.Convert(sw, null);

    string string2 = sw.ToString();
    Assert.That(string1 != string2, "strings should be different");
  }
}