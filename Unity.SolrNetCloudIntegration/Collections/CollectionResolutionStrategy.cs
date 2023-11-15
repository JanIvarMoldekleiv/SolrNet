
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Strategies


namespace Unity.SolrNetCloudIntegration.Collections
{
    public class CollectionResolutionStrategy : BuilderStrategy
    {
        private static readonly MethodInfo genericResolveCollectionMethod = typeof(CollectionResolutionStrategy)
                .GetMethod("ResolveCollection", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        private delegate object CollectionResolver(BuilderContext context);

        public override void PreBuildUp(ref BuilderContext context)
        {

            Type typeToBuild = context.Type;

            if (typeToBuild.IsGenericType)
            {
                Type openGeneric = typeToBuild.GetGenericTypeDefinition();

                if (openGeneric == typeof(IEnumerable<>) ||
                    openGeneric == typeof(ICollection<>) ||
                    openGeneric == typeof(IList<>))
                {
                    Type elementType = typeToBuild.GetGenericArguments()[0];

                    MethodInfo resolverMethod = genericResolveCollectionMethod.MakeGenericMethod(elementType);

                    CollectionResolver resolver = (CollectionResolver)Delegate.CreateDelegate(typeof(CollectionResolver), resolverMethod);

                    context.Existing = resolver(context);
                    context.BuildComplete = true;
                }
            }            
        }


    }
}
