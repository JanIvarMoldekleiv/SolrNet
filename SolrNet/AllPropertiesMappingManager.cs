﻿#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet {
	/// <summary>
	/// Maps all properties in the class, with the same field name as the property.
	/// Note that unique keys must be added manually.
	/// </summary>
	public class AllPropertiesMappingManager : IReadOnlyMappingManager {
		private readonly IDictionary<Type, PropertyInfo> uniqueKeys = new Dictionary<Type, PropertyInfo>();

		public ICollection<KeyValuePair<PropertyInfo, string>> GetFields(Type type) {
			var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var kvProps = Func.Select(props, prop => new KeyValuePair<PropertyInfo, string>(prop, prop.Name));
			return new List<KeyValuePair<PropertyInfo, string>>(kvProps);
		}

		public KeyValuePair<PropertyInfo, string> GetUniqueKey(Type type) {
		    try {
		        var key = uniqueKeys[type];
		        return new KeyValuePair<PropertyInfo, string>(key, key.Name);
		    } catch (KeyNotFoundException) {
		        throw new NoUniqueKeyException(type);
		    }
		}

		public void SetUniqueKey(PropertyInfo property) {
			if (property == null)
				throw new ArgumentNullException("property");
			var t = property.ReflectedType;
			uniqueKeys[t] = property;			
		}
	}
}