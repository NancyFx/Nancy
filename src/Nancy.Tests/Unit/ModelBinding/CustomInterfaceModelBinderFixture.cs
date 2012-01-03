using System;
using FakeItEasy;
using Nancy.ModelBinding;
using Nancy.Tests.Fakes;
using Xunit;

namespace Nancy.Tests.Unit.ModelBinding
{
	public interface IMyInterface
	{
	}

	public class MyImplementation : IMyInterface
	{
	}

	public class MyModelBinder : IModelBinder
	{
		public object Bind(NancyContext context, Type modelType, params string[] blackList)
		{
			return new MyImplementation();
		}

		public bool CanBind(Type modelType)
		{
			return typeof (IMyInterface).IsAssignableFrom(modelType);
		}
	}

	public class MyModule : NancyModule
	{
		public MyModule(IModelBinderLocator mbl, NancyContext context)
		{
			ModelBinderLocator = mbl;
			Context = context;
		}

		public IMyInterface UseGenericBinderMethod()
		{
			return this.Bind<IMyInterface>();
		}

		public IMyInterface UseBinderMethod()
		{
			return this.Bind() as IMyInterface;
		}

		public IMyInterface WorkAround()
		{
			var mb = new MyModelBinder();

			Assert.True(mb.CanBind(typeof(MyImplementation)));

			return mb.Bind(Context, typeof(IMyInterface), null) as IMyInterface;
		}
	}

	public class CustomInterfaceModelBinderFixture
	{
		private readonly FakeDefaultNancyBootstrapper _bs;
		private readonly MyModule _myModule;
		public CustomInterfaceModelBinderFixture()
		{
			_bs = new FakeDefaultNancyBootstrapper();
			
			_bs.Initialise();
			_bs.Container.Register(typeof(IModelBinderLocator), typeof(DefaultModelBinderLocator));
			_bs.Container.Register(typeof (MyModule), typeof (MyModule));

			_myModule = _bs.Container.Resolve<MyModule>();
		}

		[Fact]
		public void mymodelbinder_binds_myimplementation()
		{
			var context = _bs.Container.Resolve<NancyContext>();

			var mb = new MyModelBinder();

			Assert.True(mb.CanBind(typeof(MyImplementation)));

			var instance = mb.Bind(context, typeof (IMyInterface), null) as IMyInterface;
			object_is_myimplementation_type(instance);
		}

		[Fact]
		public void generic_bindmethod_for_interfaces_should_work()
		{
			// generic method
			var instance = _myModule.UseGenericBinderMethod();
			object_is_myimplementation_type(instance);
		}

		[Fact]
		public void bindmethod_for_interfaces_should_work()
		{
			var instance = _myModule.UseBinderMethod();
			object_is_myimplementation_type(instance);
		}

		[Fact]
		public void workaround()
		{
			var instance = _myModule.WorkAround();
			object_is_myimplementation_type(instance);
		}

		private void object_is_myimplementation_type(object instance)
		{
			Assert.NotNull(instance);
			Assert.True(typeof(IMyInterface).IsAssignableFrom(instance.GetType()));
			Assert.IsType(typeof(MyImplementation), instance);
		}
	}
}