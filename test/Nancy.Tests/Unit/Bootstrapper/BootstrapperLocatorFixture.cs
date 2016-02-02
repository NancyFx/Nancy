namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;
    using System.Collections.Generic;

    using Nancy.Bootstrapper;

    using Xunit;

    public class BootstrapperLocatorFixture
    {
        /// <summary>
        /// Internal stuff
        /// </summary>
        public interface IBootstwapper
        {
            
        }

        /// <summary>
        /// Base
        /// </summary>
        public abstract class BootstwapperBase : IBootstwapper
        {
            
        }

        /// <summary>
        /// Default
        /// </summary>
        public class DefaultBootstwapper : BootstwapperBase
        {
            
        }

        /// <summary>
        /// Custom 
        /// </summary>
        public class MyOwnDefaultBootstwapper : BootstwapperBase
        {

        }

        /// <summary>
        /// Custom 
        /// </summary>
        public class MyFirstBootstwapper : DefaultBootstwapper
        {
            
        }

        /// <summary>
        /// Custom 
        /// </summary>
        public class AnotherFirstBootstwapper : DefaultBootstwapper
        {

        }

        /// <summary>
        /// Another custom one
        /// </summary>
        public class MySecondBootstwapper: MyFirstBootstwapper
        {
             
        }
        
        /// <summary>
        /// Another custom one
        /// </summary>
        public class MyThirdBootstwapper: MyFirstBootstwapper
        {
             
        }

        /// <summary>
        /// Another custom one
        /// </summary>
        public class MyFourthBootstwapper : MySecondBootstwapper
        {

        }

        [Fact]
        public void Should_automatically_resolve_the_most_derived_bootstrapper()
        {
            // Given
            var list = new List<Type> {typeof (MyFirstBootstwapper), typeof (MySecondBootstwapper), typeof(MyFourthBootstwapper)};
            Type found;

            // When
            var res = NancyBootstrapperLocator.TryFindMostDerivedType(list, out found);

            // Then
            res.ShouldEqual(true);
            found.ShouldEqual(typeof(MyFourthBootstwapper));
        }

        [Fact]
        public void Should_return_false_when_there_are_more_than_one_most_derived_types_and_their_base_is_not_included()
        {
            // Given
            var list = new List<Type> { typeof(MyFirstBootstwapper), typeof(AnotherFirstBootstwapper)};
            Type found;

            // When
            var res = NancyBootstrapperLocator.TryFindMostDerivedType(list, out found);

            // Then
            res.ShouldEqual(false);
            found.ShouldEqual(null);
        }

        [Fact]
        public void Should_return_false_when_there_are_more_than_one_most_derived_types()
        {
            // Given
            var list = new List<Type> { typeof(MyFirstBootstwapper), typeof(MySecondBootstwapper), typeof(MyThirdBootstwapper) };
            Type found;

            // When
            var res = NancyBootstrapperLocator.TryFindMostDerivedType(list, out found);

            // Then
            res.ShouldEqual(false);
            found.ShouldEqual(null);
        }

        [Fact]
        public void Should_be_able_to_handle_parent_and_grandparent_inheritance_of_internal_stuff()
        {
            // Given
            var list = new List<Type> { typeof(MyOwnDefaultBootstwapper), typeof(MyFirstBootstwapper) };
            Type found;

            // When
            var res = NancyBootstrapperLocator.TryFindMostDerivedType(list, out found);

            // Then
            res.ShouldEqual(false);
            found.ShouldEqual(null);
        }

        [Fact]
        public void Should_be_able_to_handle_a_complete_graph()
        {
            // Given
            var list = new List<Type> { typeof(BootstwapperBase), typeof(DefaultBootstwapper), typeof(MyFirstBootstwapper), typeof(MySecondBootstwapper) };
            Type found;

            // When
            var res = NancyBootstrapperLocator.TryFindMostDerivedType(list, out found);

            // Then
            res.ShouldEqual(true);
            found.ShouldEqual(typeof(MySecondBootstwapper));
        } 
    }
}
