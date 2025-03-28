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

namespace log4net.Filter;

/// <summary>
/// The return result from <see cref="IFilter.Decide"/>
/// </summary>
/// <remarks>
/// <para>
/// The return result from <see cref="IFilter.Decide"/>
/// </para>
/// </remarks>
public enum FilterDecision : int
{
  /// <summary>
  /// The log event must be dropped immediately without 
  /// consulting with the remaining filters, if any, in the chain.
  /// </summary>
  Deny = -1,

  /// <summary>
  /// This filter is neutral with respect to the log event. 
  /// The remaining filters, if any, should be consulted for a final decision.
  /// </summary>
  Neutral = 0,

  /// <summary>
  /// The log event must be logged immediately without 
  /// consulting with the remaining filters, if any, in the chain.
  /// </summary>
  Accept = 1,
}
