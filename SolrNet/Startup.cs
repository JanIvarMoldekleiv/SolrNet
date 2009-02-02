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

using Microsoft.Practices.ServiceLocation;
using SolrNet.Utils;

namespace SolrNet {
    public static class Startup {
        static Startup() {
            Container = new Container();
            ServiceLocator.SetLocatorProvider(() => Container);
            var mapper = new MemoizingMappingManager(new AttributesMappingManager());
            Container.Register<IReadOnlyMappingManager>(c => mapper);

            var rng = new RNG();
            Container.Register<IRNG>(c => rng);

            var randomizer = new ListRandomizer();
            Container.Register<IListRandomizer>(c => randomizer);

        }

        public static readonly Container Container;

        public static void Init<T>(string serverURL) where T: new() {
            var connection = new SolrConnection(serverURL);
            var connectionKey = string.Format("{0}.{1}.{2}", typeof(SolrConnection), typeof(T), serverURL);
            Container.Register<ISolrConnection>(connectionKey, c => connection);

            var resultParser = new SolrQueryResultParser<T>(Container.GetInstance<IReadOnlyMappingManager>());
            Container.Register<ISolrQueryResultParser<T>>(c => resultParser);

            var queryExecuter = new SolrQueryExecuter<T>(connection, resultParser, Container.GetInstance<IReadOnlyMappingManager>());
            Container.Register<ISolrQueryExecuter<T>>(c => queryExecuter);

            var documentSerializer = new SolrDocumentSerializer<T>(Container.GetInstance<IReadOnlyMappingManager>());
            Container.Register<ISolrDocumentSerializer<T>>(c => documentSerializer);

            var basicServer = new SolrBasicServer<T>(connection, queryExecuter, documentSerializer);
            Container.Register<ISolrBasicOperations<T>>(c => basicServer);
            Container.Register<ISolrBasicReadOnlyOperations<T>>(c => basicServer);

            var server = new SolrServer<T>(basicServer, Container.GetInstance<IReadOnlyMappingManager>());
            Container.Register<ISolrOperations<T>>(c => server);
            Container.Register<ISolrReadOnlyOperations<T>>(c => server);
 
        }
    }
}