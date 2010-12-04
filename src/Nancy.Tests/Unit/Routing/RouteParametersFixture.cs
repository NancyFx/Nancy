using System;

namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Routing;
    using Xunit;

    public class RouteParametersFixture
    {
        [Fact]
        public void Should_support_dynamic_properties()
        {
            //Given
            dynamic parameters = new RouteParameters();
            parameters.test = 10;

            // When
            var value = (int)parameters.test;

            // Then
            value.ShouldEqual(10);
        }
		[Fact]
		public void Should_support_dynamic_casting_of_properties_to_ints()
		{
			//Given
			dynamic parameters = new RouteParameters();
			parameters.test = "10";

			// When
			var value = (int)parameters.test;

			// Then
			value.ShouldEqual(10);
		}

		[Fact]
		public void Should_support_dynamic_casting_of_properties_to_guids()
		{
			//Given
			dynamic parameters = new RouteParameters();
			var guid = Guid.NewGuid();
			parameters.test = guid.ToString();

			// When
			var value = (Guid)parameters.test;

			// Then
			value.ShouldEqual(guid);
		}


		[Fact]
		public void Should_support_dynamic_casting_of_properties_to_timespans()
		{
			//Given
			dynamic parameters = new RouteParameters();
			parameters.test = new TimeSpan(1, 2, 3, 4).ToString();

			// When
			var value = (TimeSpan)parameters.test;

			// Then
			value.ShouldEqual(new TimeSpan(1, 2, 3, 4));
		}

		[Fact]
		public void Should_support_dynamic_casting_of_properties_to_datetimes()
		{
			//Given
			dynamic parameters = new RouteParameters();

			parameters.test = new DateTime(2001, 3, 4);

			// When
			var value = (DateTime)parameters.test;

			// Then
			value.ShouldEqual(new DateTime(2001, 3, 4));
		}


		[Fact]
		public void Should_support_dynamic_casting_of_nullable_properties()
		{
			//Given
			dynamic parameters = new RouteParameters();
			var guid = Guid.NewGuid();
			parameters.test = guid.ToString();

			// When
			var value = (Guid?)parameters.test;

			// Then
			value.ShouldEqual(guid);
		}

	}
}